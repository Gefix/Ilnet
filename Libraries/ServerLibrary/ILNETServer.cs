using System;
using System.Data;
using System.Data.OleDb;

using ILNET;
using ILNET.Common;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using ILNET.Server.Properties;
using ILNET.Compiler;
using ILNET.Elements;

namespace ILNET.Server
{
    //Service classes
    public class ILNETServer : MarshalByRefObject
    {
        private string m_solutionPath = Settings.Default.ProjectsFolder;

        private string PackagePathname(string package)
        {
            return package.Replace('.', '/');
        }

        private int LoadProject(string name)
        {
            string pathname = PackagePathname(name);
            if (File.Exists(m_solutionPath + pathname + ".xml"))
            {
                string file = File.ReadAllText(m_solutionPath + pathname + ".xml");
                return Load(file);
            }
            return 0;
        }

        private void SaveProject(string name, string source)
        {
            string pathname = PackagePathname(name);
            File.WriteAllText(m_solutionPath + pathname + ".xml", source);
            Console.WriteLine("Project " + name + " saved");
        }

        private void DeleteProject(string name)
        {
            string pathname = PackagePathname(name);
            File.Delete(m_solutionPath + pathname + ".xml");
            Console.WriteLine("Project " + name + " removed");
        }

        public ILNETServer()
        {
            if (!Directory.Exists(m_solutionPath)) Directory.CreateDirectory(m_solutionPath);
            /*
            foreach (string pathname in Directory.GetFiles(m_solutionPath, "*.xml"))
            {
                string name = Path.GetFileNameWithoutExtension(pathname);
                LoadProject(name);
            }
            */ 
        }

        public override Object InitializeLifetimeService()
        {
            return null;
        }

        private static Random m_rand = new Random();

        private Dictionary<int, string> m_ProjectsByID = new Dictionary<int, string>();
        private Dictionary<string, int> m_ProjectsByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<int, ProjectRunner> m_Builds = new Dictionary<int, ProjectRunner>();

        private Dictionary<int, ProjectRunner> m_Runners = new Dictionary<int, ProjectRunner>();

        private Dictionary<int, int> m_ProjectsByPID = new Dictionary<int, int>();
        private Dictionary<int, Dictionary<int, bool>> m_RunnersByID = new Dictionary<int, Dictionary<int, bool>>();

        private struct EventPoint
        {
            public Int32 pid;
            public Int32 gid;
            public Hashtable data;
            public FunctionExit port;
        }

        private Dictionary<int, EventPoint> m_RaisedEvents = new Dictionary<int, EventPoint>();
        private Dictionary<int, EventPoint> m_EventListeners = new Dictionary<int, EventPoint>();

        private Dictionary<int, Queue> m_EventResults = new Dictionary<int, Queue>();

        private Dictionary<int, Queue> m_ProcessResults = new Dictionary<int, Queue>();

        void callback(Hashtable results)
        {
            int pid = (int)results["pid"];
            if (!m_ProcessResults.ContainsKey(pid)) return;
            m_ProcessResults[pid].Enqueue(results);

            lock (m_ProcessResults[pid])
            {
                Monitor.PulseAll(m_ProcessResults[pid]);
            }
            Console.WriteLine("Signal reached to end node");
        }

        int eventqueue(Hashtable data, FunctionExit port)
        {
            int returnValue = 0;

            EventPoint l = new EventPoint();
            l.data = data;
            l.port = port;

            string eventType = "raise";
            if (l.data.Contains("systemEvent"))
            {
                eventType = l.data["systemEvent"].ToString();
                l.data.Remove("systemEvent");
            }

            int pid = 0;
            if (l.data.Contains("pid"))
            {
                pid = (int)l.data["pid"];
                l.pid = pid;
                l.data.Remove("pid");
            }

            int id = 0;
            if (l.data.Contains("gid"))
            {
                id = (int)l.data["gid"];
                l.gid = id;
                l.data.Remove("gid");
            }

            lock (m_RaisedEvents)
            {
                if (eventType == "raise")
                {
                    int eid = m_rand.Next(1, Int32.MaxValue);
                    while (m_RaisedEvents.ContainsKey(eid)) eid = m_rand.Next(1, Int32.MaxValue);
                    m_RaisedEvents[eid] = l;
                    returnValue = eid;

                    if (l.port == null) m_EventResults[eid] = new Queue();

                    foreach (int lid in m_EventListeners.Keys)
                    {
                        if (IsRespectiveListener(l, m_EventListeners[lid].data))
                        {
                            Hashtable h = Utils.Clone(l.data);
                            h.Add("systemEventId", eid);
                            m_EventListeners[lid].port(h);
                        }
                    }

                    if (m_Runners.ContainsKey(pid))
                    {
                        lock (m_Runners[pid])
                        {
                            Monitor.PulseAll(m_Runners[pid]);
                        }
                    }
                }
                else if (eventType == "query")
                {
                    Hashtable response = new Hashtable();

                    int i = 0;
                    foreach (int eid in m_RaisedEvents.Keys)
                    {
                        if (m_RaisedEvents[eid].data.ContainsKey("system"))
                        {
                            if (m_RaisedEvents[eid].data["system"] as String == "EventTimeout")
                            {
                                if (DateTime.FromBinary(Convert.ToInt64(m_RaisedEvents[eid].data["value"])).CompareTo(DateTime.Now) <= 0)
                                {
                                    // TODO: remove key from m_RaisedEvents and m_EventResults tables
                                }
                            }
                        }
                        if (IsRespectiveListener(m_RaisedEvents[eid], l.data)) response.Add(i++, eid);
                    }

                    l.port(response);
                }
                else if (eventType == "fetch")
                {
                    Hashtable response = new Hashtable();
                    bool success = true;

                    if (l.data.Contains("systemEventId"))
                    {
                        int eventId = Convert.ToInt32(l.data["systemEventId"]);
                        if (!m_RaisedEvents.ContainsKey(eventId)) success = false;
                        else
                        {
                            response = Utils.Clone(m_RaisedEvents[eventId].data);
                        }
                    }
                    else success = false;

                    if (!success) response.Add("success", false);

                    l.port(response);
                }
                else if (eventType == "answer")
                {
                    Hashtable response = new Hashtable();
                    bool success = true;

                    if (l.data.Contains("systemEventId"))
                    {
                        int eventId = Convert.ToInt32(l.data["systemEventId"]);
                        l.data.Remove("systemEventId");

                        if (!m_RaisedEvents.ContainsKey(eventId)) success = false;
                        else
                        {
                            bool close = false;
                            if (l.data.Contains("systemEventClose")) close = Convert.ToBoolean(l.data["systemEventClose"]);

                            if (m_RaisedEvents[eventId].port != null)
                                m_RaisedEvents[eventId].port(l.data);
                            else
                            {
                                m_EventResults[eventId].Enqueue(l.data);

                                lock (m_EventResults[eventId])
                                {
                                    Monitor.PulseAll(m_EventResults[eventId]);
                                }
                            }

                            if (close)
                            {
                                if (m_RaisedEvents[eventId].port != null)
                                {
                                    m_RaisedEvents.Remove(eventId);
                                }
                                else
                                {
                                    m_RaisedEvents[eventId].data.Clear();
                                    m_RaisedEvents[eventId].data.Add("system", "EventTimeout");
                                    m_RaisedEvents[eventId].data.Add("value", DateTime.Now.AddMinutes(5).ToBinary());
                                }
                            }
                        }
                    }
                    else success = false;

                    response.Add("success", success);

                    l.port(response);
                }
                else if (eventType == "close")
                {
                    Hashtable response = new Hashtable();
                    bool success = true;

                    if (l.data.Contains("systemEventId"))
                    {
                        int eventId = Convert.ToInt32(l.data["systemEventId"]);
                        if (!m_RaisedEvents.ContainsKey(eventId)) success = false;
                        else
                        {
                            if (m_RaisedEvents[eventId].port != null)
                            {
                                m_RaisedEvents.Remove(eventId);
                            }
                            else
                            {
                                m_RaisedEvents[eventId].data.Clear();
                                m_RaisedEvents[eventId].data.Add("system", "EventTimeout");
                                m_RaisedEvents[eventId].data.Add("value", DateTime.Now.AddMinutes(5).ToBinary());
                            }
                        }
                    }
                    else if (l.data.Contains("systemListenerId"))
                    {
                        int listenerId = Convert.ToInt32(l.data["systemListenerId"]);
                        if (!m_EventListeners.ContainsKey(listenerId)) success = false;
                        else
                        {
                            m_EventListeners.Remove(listenerId);
                        }
                    }
                    else success = false;

                    response.Add("success", success);

                    l.port(response);
                }
                else if (eventType == "listen")
                {
                    int lid = m_rand.Next(1, Int32.MaxValue);
                    while (m_EventListeners.ContainsKey(lid)) lid = m_rand.Next(1, Int32.MaxValue);
                    m_EventListeners[lid] = l;
                    returnValue = lid; 

                    foreach (int eid in m_RaisedEvents.Keys)
                    {
                        if (IsRespectiveListener(m_RaisedEvents[eid], l.data))
                        {
                            Hashtable h = Utils.Clone(m_RaisedEvents[eid].data);
                            h.Add("systemEventId", eid);
                            l.port(h);
                        }
                    }
                }
                else if (eventType == "systemLoad") // internal, direct proccessing
                {
                    if (l.data.ContainsKey("assembly")) l.data.Remove("assembly");

                    if (l.data.Contains("projectName"))
                    {
                        string projectName = Convert.ToString(l.data["projectName"]);

                        int projectId;

                        if (!m_ProjectsByName.ContainsKey(projectName))
                            projectId = LoadProject(projectName);
                        else
                            projectId = m_ProjectsByName[projectName];

                        lock (m_ProjectsByID)
                        {
                            if (m_Builds.ContainsKey(projectId))
                                l.data.Add("assembly", m_Builds[projectId].Assembly);
                        }
                    }
                }
            }
            Console.WriteLine("Node raised an event");

            return returnValue;
        }

        private bool IsRespectiveListener(EventPoint epoint, Hashtable hlistener)
        {
            foreach (Object key in hlistener.Keys)
            {
                if (!epoint.data.ContainsKey(key)) return false;
                if (!(epoint.data[key].ToString() == hlistener[key].ToString())) return false;
            }
            return true;
        }

        private string[] HashtableToStringArray(Hashtable h)
        {
            string[] sresult = new string[h.Count * 2];

            int i = 0;
            foreach (Object o in h.Keys)
            {
                sresult[i++] = o.ToString();
                sresult[i++] = h[o].ToString();
            }

            return sresult;
        }

        private Hashtable StringArrayToHashtable(string[] s)
        {
            Hashtable h = new Hashtable();

            if (s == null) return h;

            for (int i = 0; i + 1 < s.Length; i += 2)
            {
                h[s[i]] = s[i + 1];
            }

            return h;
        }

        public int[] GetLoadedProjectIDs()
        {
            int[] result = new int[m_ProjectsByID.Count];
            int i = 0;
            foreach (int id in m_ProjectsByID.Keys) result[i++] = id;
            return result;
        }

        public string[] GetLoadedProjectNames()
        {
            string[] result = new string[m_ProjectsByName.Count];
            int i = 0;
            foreach (string id in m_ProjectsByName.Keys) result[i++] = id;
            return result;
        }

        public int GetLoadedProjectID(string projectName)
        {
            if (!m_ProjectsByName.ContainsKey(projectName)) return 0;

            return m_ProjectsByName[projectName];
        }

        public string GetLoadedProjectSource(int id)
        {
            if (!m_ProjectsByID.ContainsKey(id)) return "";

            return Project.Serialize(m_Builds[id].Project);
        }

        public string[] GetRepositoryProjectNames(string path)
        {
            path = PackagePathname(path);
            List<string> result = new List<string>();
            foreach (string pathname in Directory.GetFiles(m_solutionPath + path, "*.xml"))
            {
                string name = Path.GetFileNameWithoutExtension(pathname);
                result.Add(name);
            }
            return result.ToArray();
        }

        public string[] GetRepositoryFolderNames(string path)
        {
            path = PackagePathname(path);
            List<string> result = new List<string>();
            foreach (string pathname in Directory.GetDirectories(m_solutionPath + path))
            {
                string name = Path.GetFileName(pathname);
                result.Add(name);
            }
            return result.ToArray();
        }

        public bool CreateRepositoryFolder(string path)
        {
            path = PackagePathname(path);
            if (!Directory.Exists(Path.GetDirectoryName(path))) return false;
            if (Directory.Exists(path)) return false;
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteRepositoryFolder(string path)
        {
            path = PackagePathname(path);
            if (!Directory.Exists(path)) return false;
            try
            {
                Directory.Delete(path);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public string GetProjectSource(string projectName)
        {
            projectName = m_solutionPath + PackagePathname(projectName) + ".xml";
            if (!File.Exists(projectName)) return null;

            return File.ReadAllText(projectName);
        }

        public int GetProjectID(string projectName)
        {
            if (!m_ProjectsByName.ContainsKey(projectName))
                return LoadProject(projectName);
            else
                return m_ProjectsByName[projectName];
        }

        // Returns ID
        public int Load(string projectSource)
        {
            int id = m_rand.Next(1, Int32.MaxValue);

            int oldid = 0;

            lock (m_ProjectsByID)
            {
                while (m_ProjectsByID.ContainsKey(id)) id = m_rand.Next(1, Int32.MaxValue);

                ProjectRunner runner;

                try
                {
                    runner = new ProjectCompiler().Compile(projectSource);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }

                string projectName = runner.Project.Name;

                m_Builds[id] = runner;
                m_ProjectsByID[id] = projectName;

                if (m_ProjectsByName.ContainsKey(projectName))
                {
                    oldid = m_ProjectsByName[projectName];
                }

                m_ProjectsByName[projectName] = id;
            }

            if (oldid > 0)
            {
                Unload(oldid);
            }

            Console.WriteLine("Project " + m_ProjectsByID[id] + " loaded with ID: " + id.ToString());

            SaveProject(m_ProjectsByID[id], projectSource);

            return id;
        }

        public string LoadErrors(string projectSource)
        {
            ProjectRunner runner;

            try
            {
                runner = new ProjectCompiler().Compile(projectSource);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public bool Delete(string projectName)
        {
            if (m_ProjectsByName.ContainsKey(projectName)) return false;

            DeleteProject(projectName);

            return true;
        }

        public bool Unload(int id)
        {
            if (!m_ProjectsByID.ContainsKey(id)) return false;

            string name = m_ProjectsByID[id];

            lock (m_ProjectsByID)
            {
                // delete the project and stop its processes
                if (m_RunnersByID.ContainsKey(id))
                {
                    List<int> toStop = new List<int>();

                    foreach (int pid in m_RunnersByID[id].Keys)
                    {
                        toStop.Add(pid);
                    }

                    foreach (int pid in toStop)
                    {
                        if (!Stop(pid)) m_RunnersByID[id].Remove(pid);
                    }

                    m_RunnersByID.Remove(id);
                }

                if (m_ProjectsByName[m_ProjectsByID[id]] == id)
                    m_ProjectsByName.Remove(m_ProjectsByID[id]);

                m_ProjectsByID.Remove(id);

                m_Builds.Remove(id);
            }

            Console.WriteLine("Project " + name + " unloaded");

            return true;
        }

        // Returns PID
        public int Run(int id)
        {
            int pid = m_rand.Next(1, Int32.MaxValue);

            lock (m_Runners)
            {
                if (!m_ProjectsByID.ContainsKey(id)) return 0;

                ProjectRunner r = m_Builds[id].Copy();

                while (m_Runners.ContainsKey(pid) || m_Builds.ContainsKey(pid)) pid = m_rand.Next(1, Int32.MaxValue);

                m_Runners[pid] = r;

                m_ProjectsByPID[pid] = id;

                if (!m_RunnersByID.ContainsKey(id)) m_RunnersByID[id] = new Dictionary<int, bool>();
                m_RunnersByID[id].Add(pid, true);

                if (!m_ProcessResults.ContainsKey(pid)) m_ProcessResults[pid] = new Queue();

                r.SetPID(pid);
                r.SetGID(id);

                r.Run(callback, eventqueue);
            }

            Console.WriteLine("Executed project with id: " + id.ToString() + " and pid: " + pid.ToString());

            return pid;
        }

        public bool Wait(int pid)
        {
            try
            {
                if (!m_Runners.ContainsKey(pid)) return false;
                lock (m_Runners)
                {
                    Monitor.Wait(m_Runners[pid]);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool WaitResult(int pid)
        {
            try
            {
                if (!m_ProcessResults.ContainsKey(pid)) return false;
                lock (m_ProcessResults[pid])
                {
                    if (m_ProcessResults[pid].Count > 0) return true;
                    Monitor.Wait(m_ProcessResults[pid]);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool WaitEvent(int eid)
        {
            try
            {
                if (!m_RaisedEvents.ContainsKey(eid)) return false;
                if (!m_EventResults.ContainsKey(eid)) m_EventResults[eid] = new Queue();
                lock (m_EventResults[eid])
                {
                    Monitor.Wait(m_EventResults[eid]);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int RaiseEvent(string[] eventKeyValues)
        {
            Hashtable h = StringArrayToHashtable(eventKeyValues);

            h.Add("systemEvent", "raise");
            return eventqueue(h, null);
        }

        public int[] GetEventQueue(string[] eventKeyValues)
        {
            List<int> events = new List<int>();

            Hashtable h = StringArrayToHashtable(eventKeyValues);

            lock (m_RaisedEvents)
            {
                foreach (int eid in m_RaisedEvents.Keys)
                {
                    if (IsRespectiveListener(m_RaisedEvents[eid], h)) events.Add(eid);
                }
            }

            int[] result = new int[events.Count];

            for (int i = 0; i < events.Count; i++) result[i] = (int)events[i];

            return result;
        }

        public string[] GetEventData(int eid)
        {
            lock (m_RaisedEvents)
            {
                if (!m_RaisedEvents.ContainsKey(eid)) return null;

                return HashtableToStringArray(m_RaisedEvents[eid].data);
            }
        }

        public string GetEventDataBinary(int eid)
        {
            string result;

            lock (m_RaisedEvents)
            {
                if (!m_RaisedEvents.ContainsKey(eid)) return null;

                MemoryStream msbin = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(msbin, m_RaisedEvents[eid].data);

                MemoryStream mszip = new MemoryStream();
                GZipStream gz = new GZipStream(mszip, CompressionMode.Compress, false);
                gz.Write(msbin.ToArray(), 0, (int)msbin.Length);

                msbin = null;

                result = Convert.ToBase64String(mszip.ToArray());

                mszip = null;
            }

            return result;
        }

        public bool SetEventResponse(int eid, string[] responseKeyValues)
        {
            lock (m_RaisedEvents)
            {
                if (!m_RaisedEvents.ContainsKey(eid)) return false;

                m_RaisedEvents[eid].port(StringArrayToHashtable(responseKeyValues));
                m_RaisedEvents.Remove(eid);
            }

            return true;
        }

        public int GetResultsCount(int pid)
        {
            if (m_ProcessResults.ContainsKey(pid)) return m_ProcessResults[pid].Count;
            return 0;
        }

        public string[] GetNextResult(int pid)
        {
            if (!m_ProcessResults.ContainsKey(pid)) return null;
            if (m_ProcessResults[pid].Count < 1) return null;

            string[] sresult = HashtableToStringArray((Hashtable)m_ProcessResults[pid].Dequeue());

            return sresult;
        }

        public int GetEventResultsCount(int eid)
        {
            if (m_EventResults.ContainsKey(eid)) return m_EventResults[eid].Count;
            return 0;
        }

        public string[] GetEventNextResult(int eid)
        {
            if (!m_EventResults.ContainsKey(eid)) return null;
            if (m_EventResults[eid].Count < 1) return null;

            string[] sresult = HashtableToStringArray((Hashtable)m_EventResults[eid].Dequeue());

            return sresult;
        }

        public bool IsRunning(int pid)
        {
            try
            {
                if (!m_Runners.ContainsKey(pid)) return false;
                return m_Runners[pid].IsRunning();
            }
            catch
            {
                return false;
            }
        }

        public bool Stop(int pid)
        {
            lock (m_Runners)
            {
                if (!m_Runners.ContainsKey(pid)) return false;
                m_Runners[pid].Kill();

                m_RunnersByID[m_ProjectsByPID[pid]].Remove(pid);
                m_ProjectsByPID.Remove(pid);

                lock (m_Runners[pid])
                {
                    Monitor.PulseAll(m_Runners[pid]);
                }
                m_Runners.Remove(pid);

                lock (m_ProcessResults[pid])
                {
                    Monitor.PulseAll(m_ProcessResults[pid]);
                }
                m_ProcessResults.Remove(pid);
            }

            Console.WriteLine("Stopped running project instance with pid: " + pid.ToString());

            lock (m_RaisedEvents)
            {
                List<int> listenersToStop = new List<int>();

                foreach (int eid in m_EventListeners.Keys)
                {
                    if (m_EventListeners[eid].pid == pid)
                        listenersToStop.Add(eid);
                }

                foreach (int eid in listenersToStop)
                {
                    m_EventListeners.Remove(eid);
                }

                Console.WriteLine("Stopped " + listenersToStop.Count.ToString() + " listeners");

                List<int> eventsToClose = new List<int>();

                foreach (int eid in m_RaisedEvents.Keys)
                {
                    if (m_RaisedEvents[eid].pid == pid)
                        eventsToClose.Add(eid);
                }

                foreach (int eid in eventsToClose)
                {
                    m_RaisedEvents.Remove(eid);
                }

                Console.WriteLine("Closed " + eventsToClose.Count.ToString() + " raised events");
            }

            return true;
        }
    }
}
