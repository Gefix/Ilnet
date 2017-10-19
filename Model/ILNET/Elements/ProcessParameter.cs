using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ILNET.Elements
{
    public class ProcessParameter : Element
    {
        private Node m_ParentNode;

        private string m_Value;

        public ProcessParameter()
        {
            m_Value = "";
        }

        [XmlIgnoreAttribute]
        public Node ParentNode { get { return m_ParentNode; } set { m_ParentNode = value; } }

        public String Compile(String shell)
        {
            Node node = ParentNode;
            Process process = node.ParentProcess;
            Project project = process.ParentProject;

            Process sub = project.FindProcess(node.Subprocess);
            
            if (sub == null) return "";

            Variable var = sub.FindVar(m_Name);

            if (var == null) return "";

            if (m_Value == "") return "";

            shell = Utils.Replace(shell, "cparamname", m_Name);
            String scope;
            switch (var.Level)
            {
                case VariableLevel.Global: scope = "g"; break;
                case VariableLevel.Intermediate: scope = "i"; break;
                case VariableLevel.Local: scope = "l"; break;
                default: return "";
            }
            shell = Utils.Replace(shell, "cparamscope", scope);
            shell = Utils.Replace(shell, "cparamvalue", node.ParseFunction(project, process, m_Value));
            return shell;
        }

        [XmlElement("value")]
        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }
    }
}
