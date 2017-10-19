using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ILNET.Elements
{
    public enum VariableLevel { Local, Intermediate, Global };

    public class Variable : Element
    {
        private Process m_ParentProcess;

        private VariableLevel m_Level;
        private string m_Type;
        private string m_Value;
        private bool m_IsInput;
        private bool m_IsOutput;

        public Variable()
        {
            m_Level = VariableLevel.Local;
            m_Type = "";
            m_Value = "";
            m_IsInput = false;
            m_IsOutput = false;
        }

        [XmlIgnoreAttribute]
        public Process ParentProcess { get { return m_ParentProcess; } set { m_ParentProcess = value; } }

        public String ScopeVar()
        {
            switch (m_Level)
            {
                case VariableLevel.Global: return "g";
                case VariableLevel.Intermediate: return "i";
                case VariableLevel.Local: return "l";
                default: return "";
            }
        }

        public String Compile(String shell)
        {
            String value = m_Value;
            if (value == "")
            {
                if (m_Type == "") return "";

                if (m_Type == "string")
                    value = "\"\"";
                else
                    if (!m_Type.Contains("[") && !m_Type.Contains("]"))
                        value = "new " + m_Type + "()";
                    else return "";
            }

            shell = Utils.Replace(shell, "cvarname", m_Name);
            shell = Utils.Replace(shell, "cvarscope", ScopeVar());
            shell = Utils.Replace(shell, "cvarvalue", value);
            return shell;
        }

        public String CompileResult(String shell)
        {
            if (!m_IsOutput) return "";

            shell = Utils.Replace(shell, "cvarname", m_Name);
            String scope;
            switch (m_Level)
            {
                case VariableLevel.Global: scope = "g"; break;
                case VariableLevel.Intermediate: scope = "i"; break;
                case VariableLevel.Local: scope = "l"; break;
                default: return "";
            }
            shell = Utils.Replace(shell, "cvarscope", scope);
            return shell;
        }

        [XmlAttribute("level")]
        public VariableLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }

        [XmlElement("type")]
        public string Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
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

        [XmlAttribute("isInput")]
        public bool IsInput
        {
            get
            {
                return m_IsInput;
            }
            set
            {
                m_IsInput = value;
            }
        }

        [XmlAttribute("isOutput")]
        public bool IsOutput
        {
            get
            {
                return m_IsOutput;
            }
            set
            {
                m_IsOutput = value;
            }
        }
    }
}
