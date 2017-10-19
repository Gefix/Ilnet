using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace ILNET.Elements
{
    public class Node : Element
    {
        private Process m_ParentProcess;

        private bool m_NodeIsSubprocess;
        private string m_Function;
        private string m_Subprocess;
        private List<ProcessParameter> m_SubprocessParams;

        private List<string> m_NextNodes;

        public Node()
        {
            m_NodeIsSubprocess = false;
            m_Function = "";
            m_Subprocess = "";
            m_NextNodes = new List<string>();
            m_SubprocessParams = new List<ProcessParameter>();
        }

        [XmlIgnoreAttribute]
        public Process ParentProcess { get { return m_ParentProcess; } set { m_ParentProcess = value; } }

        public void NewNextNode(string name)
        {
            m_NextNodes.Add(name);
        }

        public void RemoveNextNode(string name)
        {
            m_NextNodes.Remove(name);
        }

        public void NewParameter(string varname)
        {
            ProcessParameter p = new ProcessParameter();
            p.ParentNode = this;
            p.Name = varname;
            m_SubprocessParams.Add(p);
        }

        public ProcessParameter FindParameter(string name)
        {
            foreach (ProcessParameter item in m_SubprocessParams)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        public bool FindNextNode(string name)
        {
            foreach (string item in m_NextNodes)
            {
                if (item == name) return true;
            }
            return false;
        }

        public void RemoveParameter(string name)
        {
            ProcessParameter p = FindParameter(name);
            if (p == null) return;
            m_SubprocessParams.Remove(p);
        }

        public String Compile(String shell)
        {
            Process process = ParentProcess;
            Project project = process.ParentProject;

            shell = Utils.Replace(shell, "cnodename", m_Name);
            shell = Utils.Replace(shell, "cnodefnname", "fn_" + m_Name);
            shell = Utils.Replace(shell, "cnodefnnamecb", "fn_" + m_Name + "_cb");

            if (NodeIsSubprocess)
            {
                shell = Utils.Replace(shell, "cnodefncode", "");
                shell = Utils.Replace(shell, "cnodefncodecb", ParseFunction(project, process, m_Function));
            }
            else
            {
                shell = Utils.Replace(shell, "cnodefncode", ParseFunction(project, process, m_Function));
                shell = Utils.Replace(shell, "cnodefncodecb", "");
            }

            // Parameters
            if (m_NodeIsSubprocess)
            {
                shell = Utils.Replace(shell, "cnodeisfn", "");

                String shell_param = Utils.Extract(shell, "csubparam");

                StringBuilder code = new StringBuilder();

                foreach (ProcessParameter item in m_SubprocessParams)
                {
                    code.Append(item.Compile(shell_param));
                }

                shell = Utils.Replace(shell, "csubparam", code.ToString());

                if (m_Subprocess.Contains("."))
                {
                    shell = Utils.Replace(shell, "csubname", "dynamic");
                    shell = Utils.Replace(shell, "cnodeisinternal", "");

                    string processName = m_Subprocess.Substring(m_Subprocess.LastIndexOf(".") + 1);
                    string projectName = m_Subprocess.Substring(0, m_Subprocess.Length - processName.Length - 1);

                    shell = Utils.Replace(shell, "csubprojectname", projectName);
                    shell = Utils.Replace(shell, "csubprocessname", processName);
                    shell = Utils.Replace(shell, "cnodefnproxy", "fn_" + m_Name + "_proxy");
                }
                else
                {
                    shell = Utils.Replace(shell, "csubname", "c_" + m_Subprocess);
                    shell = Utils.Replace(shell, "cnodeisexternal", "");
                }
            }
            else
            {
                shell = Utils.Replace(shell, "cnodeissubprocess", "");
                shell = Utils.Replace(shell, "cnodeisexternal", "");
            }

            // Connections
            {
                String shell_goto = Utils.Extract(shell, "cnodefngoto");

                StringBuilder code = new StringBuilder();

                foreach (String item in m_NextNodes)
                {
                    code.Append(Utils.Replace(shell_goto, "cnodefngotoname", "fn_" + item));
                }

                shell = Utils.Replace(shell, "cnodefngoto", code.ToString());
            }

            return shell;
        }

        public String ParseFunction(Project project, Process process, String function)
        {
            String varRX = "\\$(?:[a-z0-9_]+|\\[[^\\]]+\\])(?:\\.(?:[a-z0-9_]+|\\[[^\\]]+\\]))*";

            Regex var = new Regex("(?<lsl>(?:\\+\\+|\\-\\-)[ ]*)?" + varRX + "(?<lsr>[ ]*(?:\\+\\+|\\-\\-|(?:[\\+\\-\\*/%&\\|\\^]?=(?=[^=]))|>>=|<<=))?", RegexOptions.IgnoreCase);

            return var.Replace(function, new FunctionVarMatchEval(project, process).EvaluateMatch);
        }

        public String CompileResult(String shell)
        {
            if (!m_NodeIsSubprocess) return "";

            shell = Utils.Replace(shell, "cnodename", m_Name);

            return shell;
        }

        [XmlAttribute("nodeIsSubprocess")]
        public bool NodeIsSubprocess
        {
            get
            {
                return m_NodeIsSubprocess;
            }
            set
            {
                m_NodeIsSubprocess = value;
            }
        }

        [XmlElement("function")]
        public string Function
        {
            get
            {
                return m_Function.Replace("\n", "\r\n");
            }
            set
            {
                m_Function = value.Replace("\r\n", "\n");
            }
        }

        [XmlAttribute("subprocess")]
        public string Subprocess
        {
            get
            {
                return m_Subprocess;
            }
            set
            {
                m_Subprocess = value;
            }
        }

        [XmlElement("nextNode")]
        public String[] NextNodesList
        {
            get
            {
                String[] items = new String[m_NextNodes.Count];
                m_NextNodes.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                String[] items = (String[])value;
                m_NextNodes.Clear();
                foreach (String item in items)
                    m_NextNodes.Add(item);
            }
        }

        [XmlElement("processParameter")]
        public ProcessParameter[] SubprocessParameters
        {
            get
            {
                ProcessParameter[] items = new ProcessParameter[m_SubprocessParams.Count];
                m_SubprocessParams.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                ProcessParameter[] items = (ProcessParameter[])value;
                m_SubprocessParams.Clear();
                foreach (ProcessParameter item in items)
                {
                    item.ParentNode = this;
                    m_SubprocessParams.Add(item);
                }
            }
        }
    }
    class FunctionVarMatchEval
    {
        private Project m_project;
        private Process m_process;

        public FunctionVarMatchEval(Project project, Process process)
        {
            m_project = project;
            m_process = process;
        }

        public String EvaluateMatch(Match match)
        {
            Regex vars = new Regex("(?:\\$|.)(?<svar>(?<var>[a-z0-9_]+)|\\[(?<var>[^\\]]+)\\])", RegexOptions.IgnoreCase);

            String s = "";
            String lastType = "";

            Process c = m_process;
            bool subVars = false;

            foreach (Match m in vars.Matches(match.ToString()))
            {
                string var = m.Groups["var"].Value;

                if (!subVars)
                {
                    if (c != null)
                    {
                        Node n = c.FindNode(var);
                        if (n != null)
                        {
                            c = m_project.FindProcess(n.Subprocess);

                            if (s == "")
                            {
                                lastType = "Hashtable";
                                s = "l[\"" + var + "\"]";
                            }
                            else
                            {
                                s = "((" + lastType + ")" + s + ")[\"" + var + "\"]";
                                lastType = "Hashtable";
                            }
                            continue;
                        }

                        Variable v = c.FindVar(var);
                        if (v != null)
                        {
                            if (s == "")
                            {
                                lastType = v.Type;
                                s = v.ScopeVar() + "[\"" + var + "\"]";
                            }
                            else
                            {
                                s = "((" + lastType + ")" + s + ")[\"" + var + "\"]";
                                lastType = v.Type;
                            }
                            c = null;
                            continue;
                        }
                    }

                    s = "((" + lastType + ")" + s + ")";
                    subVars = true;
                }
                s = s + "." + m.Groups["svar"].Value;
            }

            if (!subVars && !match.Groups["lsl"].Success && !match.Groups["lsr"].Success) s = "((" + lastType + ")" + s + ")";

            return match.Groups["lsl"].Value + s + match.Groups["lsr"].Value;
        }
    }
}
