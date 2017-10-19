using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using ILNET;

namespace ILNET.Server.WebService
{
    /// <summary>
    /// Summary description for ILNETService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        [WebMethod]
        public int[] GetLoadedProjectIDs()
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int[] result = server.GetLoadedProjectIDs();

            return result;
        }
        [WebMethod]
        public string[] GetLoadedProjectNames()
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetLoadedProjectNames();

            return result;
        }
        [WebMethod]
        public int GetLoadedProjectID(string projectName)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.GetLoadedProjectID(projectName);

            return result;
        }
        [WebMethod]
        public string GetLoadedProjectSource(int id)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string result = server.GetLoadedProjectSource(id);

            return result;
        }
        [WebMethod]
        public string[] GetRepositoryProjectNames(string path)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetRepositoryProjectNames(path);

            return result;
        }
        [WebMethod]
        public string[] GetRepositoryFolderNames(string path)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetRepositoryFolderNames(path);

            return result;
        }
        [WebMethod]
        public bool CreateRepositoryFolder(string path)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.CreateRepositoryFolder(path);

            return result;
        }
        [WebMethod]
        public bool DeleteRepositoryFolder(string path)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.DeleteRepositoryFolder(path);

            return result;
        }
        [WebMethod]
        public string GetProjectSource(string projectName)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string result = server.GetProjectSource(projectName);

            return result;
        }
        [WebMethod]
        public int GetProjectID(string projectName)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.GetProjectID(projectName);

            return result;
        }
        [WebMethod]
        public int Load(string project)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.Load(project);

            return result;
        }
        [WebMethod]
        public string LoadErrors(string project)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string result = server.LoadErrors(project);

            return result;
        }
        [WebMethod]
        public bool Unload(int id)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.Unload(id);

            return result;
        }
        [WebMethod]
        public int Run(int id)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.Run(id);

            return result;
        }
        [WebMethod]
        public int GetResultsCount(int pid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.GetResultsCount(pid);

            return result;
        }
        [WebMethod]
        public string[] GetNextResult(int pid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetNextResult(pid);

            return result;
        }
        [WebMethod]
        public int GetEventResultsCount(int eid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.GetEventResultsCount(eid);

            return result;
        }
        [WebMethod]
        public string[] GetEventNextResult(int eid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetEventNextResult(eid);

            return result;
        }
        [WebMethod]
        public int[] GetEventQueue(string[] eventKeyValues)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int[] result = server.GetEventQueue(eventKeyValues);

            return result;
        }
        [WebMethod]
        public string[] GetEventData(int eid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            string[] result = server.GetEventData(eid);

            return result;
        }
        [WebMethod]
        public bool SetEventResponse(int eid, string[] responseKeyValues)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.SetEventResponse(eid, responseKeyValues);

            return result;
        }
        [WebMethod]
        public bool WaitResult(int pid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.WaitResult(pid);

            return result;
        }
        [WebMethod]
        public int RaiseEvent(string[] eventKeyValues)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            int result = server.RaiseEvent(eventKeyValues);

            return result;
        }
        [WebMethod]
        public bool WaitEvent(int eid)
        {
            ILNETServer server = (ILNETServer)Activator.GetObject(typeof(ILNETServer), "http://localhost:8001/ILNET");

            bool result = server.WaitEvent(eid);

            return result;
        }
    }
}
