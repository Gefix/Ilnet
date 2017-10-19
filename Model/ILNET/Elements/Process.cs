using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ILNET.Elements
{
    public class Process : Element
    {
        private Project m_ParentProject;

        private List<Variable> m_Vars;
        private List<Node> m_Nodes;
        private string m_EntryNode;
        private string m_ExitNode;

        public Process()
        {
            m_Vars = new List<Variable>();
            m_Nodes = new List<Node>();
            m_EntryNode = "";
            m_ExitNode = "";
        }

        [XmlIgnoreAttribute]
        public Project ParentProject { get { return m_ParentProject; } set { m_ParentProject = value; } }

        public void NewVar(string name)
        {
            Variable var = new Variable();
            var.ParentProcess = this;
            var.Name = name;
            m_Vars.Add(var);
        }

        public Variable FindVar(string name)
        {
            foreach (Variable item in m_Vars)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        public void RemoveVar(string name)
        {
            Variable v = FindVar(name);
            if (v == null) return;
            m_Vars.Remove(v);
        }

        public void RemoveNode(string name)
        {
            Node n = FindNode(name);
            if (n == null) return;
            m_Nodes.Remove(n);

            foreach (Node nn in m_Nodes)
            {
                if (nn.FindNextNode(name)) nn.RemoveNextNode(name);
            }
        }

        public void NewNode(string name)
        {
            Node node = new Node();
            node.ParentProcess = this;
            node.Name = name;
            m_Nodes.Add(node);
        }

        public Node FindNode(string name)
        {
            foreach (Node item in m_Nodes)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        public String Compile(String shell)
        {
            Project project = ParentProject;

            shell = Utils.Replace(shell, "cname", "c_" + m_Name);
            shell = Utils.Replace(shell, "centry", "fn_" + m_EntryNode);

            // Function calls:
            {
                String shell_function = Utils.Extract(shell, "cnodefn");

                StringBuilder code = new StringBuilder();

                foreach (Node item in m_Nodes)
                {
                    String node_code = item.Compile(shell_function);

                    if (item.Name != m_ExitNode)
                    {
                        node_code = Utils.Replace(node_code, "cnodeisfinal", "");
                    }

                    code.Append(node_code);
                }

                shell = Utils.Replace(shell, "cnodefn", code.ToString());
            }

            // Variable init:
            {
                String shell_vars = Utils.Extract(shell, "varinit");

                StringBuilder code = new StringBuilder();

                foreach (Variable item in m_Vars)
                {
                    if (item.Level == VariableLevel.Global) continue;
                    code.Append(item.Compile(shell_vars));
                }

                shell = Utils.Replace(shell, "varinit", code.ToString());

                code = new StringBuilder();

                String shell_gvars = Utils.Extract(shell, "gvarinit");

                foreach (Variable item in m_Vars)
                {
                    if (item.Level != VariableLevel.Global) continue;
                    code.Append(item.Compile(shell_gvars));
                }

                shell = Utils.Replace(shell, "gvarinit", code.ToString());
            }

            // Transition tables:
            {
                String shell_function = Utils.Extract(shell, "transition");

                StringBuilder code = new StringBuilder();

                foreach (Node startNode in m_Nodes)
                {
                    foreach (string endNodeName in startNode.NextNodesList)
                    {
                        String transition_code = Utils.Replace(shell_function, "cnodestartname", startNode.Name);
                        transition_code = Utils.Replace(transition_code, "cnodeendname", endNodeName);

                        code.Append(transition_code);
                    }
                }

                shell = Utils.Replace(shell, "transition", code.ToString());
            }

            // Node names:
            {
                String shell_function = Utils.Extract(shell, "nodename");

                StringBuilder code = new StringBuilder();

                foreach (Node item in m_Nodes)
                {
                    {
                        String nodename_code = Utils.Replace(shell_function, "cnodecodename", "fn_" + item.Name);
                        nodename_code = Utils.Replace(nodename_code, "cnoderealname", item.Name);
                        code.Append(nodename_code);
                    }
                    {
                        String nodename_code = Utils.Replace(shell_function, "cnodecodename", "fn_" + item.Name + "_cb");
                        nodename_code = Utils.Replace(nodename_code, "cnoderealname", item.Name);
                        code.Append(nodename_code);
                    }
                }

                shell = Utils.Replace(shell, "nodename", code.ToString());
            }

            // Result passing:
            {
                String shell_results = Utils.Extract(shell, "cresult");

                StringBuilder code = new StringBuilder();

                String shell_varresult = Utils.Extract(shell_results, "cvarresult");

                foreach (Variable item in m_Vars)
                {
                    code.Append(item.CompileResult(shell_varresult));
                }

                shell_results = Utils.Replace(shell_results, "cvarresult", code.ToString());

                code = new StringBuilder();

                String shell_noderesult = Utils.Extract(shell_results, "cnoderesult");

                foreach (Node item in m_Nodes)
                {
                    code.Append(item.CompileResult(shell_noderesult));
                }

                shell_results = Utils.Replace(shell_results, "cnoderesult", code.ToString());

                shell = Utils.Replace(shell, "cresult", shell_results);
            }

            return shell;
        }

        [XmlAttribute("entryNode")]
        public string EntryNode
        {
            get
            {
                return m_EntryNode;
            }
            set
            {
                m_EntryNode = value;
            }
        }

        [XmlAttribute("exitNode")]
        public string ExitNode
        {
            get
            {
                return m_ExitNode;
            }
            set
            {
                m_ExitNode = value;
            }
        }

        [XmlElement("node")]
        public Node[] NodesList
        {
            get
            {
                Node[] items = new Node[m_Nodes.Count];
                m_Nodes.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Node[] items = (Node[])value;
                m_Nodes.Clear();
                foreach (Node item in items)
                {
                    item.ParentProcess = this;
                    m_Nodes.Add(item);
                }
            }
        }

        [XmlElement("var")]
        public Variable[] VarsList
        {
            get
            {
                Variable[] items = new Variable[m_Vars.Count];
                m_Vars.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Variable[] items = (Variable[])value;
                m_Vars.Clear();
                foreach (Variable item in items)
                {
                    item.ParentProcess = this;
                    m_Vars.Add(item);
                }
            }
        }
    }
}
