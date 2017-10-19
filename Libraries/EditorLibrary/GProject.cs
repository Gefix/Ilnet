using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;
using System.Drawing.Drawing2D;

namespace ILNET.Editor
{
    public class GProject
    {
        Dictionary<string, GProcess> m_GProcesses;

        Project m_Project;

        public GProject()
        {
            m_GProcesses = new Dictionary<string, GProcess>();
        }

        public VisualTool GetTool(string tool)
        {
            try
            {
                return (VisualTool)Enum.Parse(typeof(VisualTool), tool, true);
            }
            catch 
            {
                return 0;
            }
        }

        public GProcess GProcess(string processName)
        {
            if (m_GProcesses.ContainsKey(processName)) return m_GProcesses[processName];
            else return null;
        }

        public void Load(Project project)
        {
            m_Project = project;

            foreach (Process process in m_Project.ProcessList)
            {
                GProcess c = new GProcess(this);
                c.Load(project, process);
                m_GProcesses.Add(process.Name, c);
            }
        }

        public void BeforeSave()
        {
            foreach (GProcess c in m_GProcesses.Values)
            {
                c.BeforeSave();
            }
        }

        public Project BaseProject
        {
            get
            {
                return m_Project;
            }
        }

        internal bool RenameProcess(string oldValue, string newValue)
        {
            if (m_GProcesses.ContainsKey(newValue)) return false;
            if (!m_GProcesses.ContainsKey(oldValue)) return false;
            foreach (GProcess c in m_GProcesses.Values)
            {
                c.RenameProcess(oldValue, newValue);
            }
            m_GProcesses.Add(newValue, m_GProcesses[oldValue]);
            m_GProcesses.Remove(oldValue);
            
            return true;
        }

        public void RemoveProcess(string name)
        {
            if (m_GProcesses.ContainsKey(name))
            {
                m_GProcesses.Remove(name);
                m_Project.RemoveProcess(name);
            }
        }

        public void AddProcess(string name)
        {
            if (!m_GProcesses.ContainsKey(name))
            {
                m_Project.NewProcess(name);
                Process process = m_Project.FindProcess(name);
                GProcess c = new GProcess(this);
                c.Load(m_Project, process);
                m_GProcesses.Add(process.Name, c);
            }
        }
    }
}
