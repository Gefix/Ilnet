using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

namespace ILNET.Elements
{
    [XmlRoot("project")]
    public class Project : Element
    {
        private string m_StartUpProcess;
        private List<Process> m_ProcessList;

        public Project()
        {
            m_StartUpProcess = "";
            m_ProcessList = new List<Process>();
        }

        public void NewProcess(string name)
        {
            Process c = new Process();
            c.ParentProject = this;
            c.Name = name;

            m_ProcessList.Add(c);
        }

        public void RemoveProcess(string name)
        {
            Process c = FindProcess(name);
            if (c == null) return;
            m_ProcessList.Remove(c);
        }

        public Process FindProcess(string name)
        {
            foreach (Process item in m_ProcessList)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        public String Compile()
        {
            String shell = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ILNET.Elements.Shell.cs")).ReadToEnd();

            shell = Utils.Replace(shell, "skip", "");

            shell = Utils.Replace(shell, "sclass", "c_" + m_StartUpProcess);

            String shellProcess = Utils.Extract(shell, "class");

            StringBuilder code = new StringBuilder();

            foreach (Process item in m_ProcessList)
            {
                code.Append(item.Compile(shellProcess));
            }

            shell = Utils.Replace(shell, "class", code.ToString());

            return shell;
        }

        [XmlAttribute("startUpProcess")]
        public string StartUpProcess
        {
            get
            {
                return m_StartUpProcess;
            }
            set
            {
                m_StartUpProcess = value;
            }
        }

        [XmlElement("process")]
        public Process[] ProcessList
        {
            get
            {
                Process[] items = new Process[m_ProcessList.Count];
                m_ProcessList.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Process[] items = (Process[])value;
                m_ProcessList.Clear();
                foreach (Process item in items)
                {
                    item.ParentProject = this;
                    m_ProcessList.Add(item);
                }
            }
        }

        static public Project Deserialize(String source)
        {
            return Deserialize(new StringReader(source));
        }

        static public Project Deserialize(TextReader source)
        {
            return (Project)new XmlSerializer(typeof(Project)).Deserialize(source);
        }

        static public String Serialize(Project project)
        {
            XmlSerializer s = new XmlSerializer(typeof(Project));
            StringWriter w = new StringWriter();
            s.Serialize(w, project);
            string result = w.ToString();
            w.Close();
            return result;
        }
    }
}
