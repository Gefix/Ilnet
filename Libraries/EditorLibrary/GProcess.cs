using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;

namespace ILNET.Editor
{
    public class ConnectionMap
    {
        Dictionary<GNode, Dictionary<GNode, List<GConnection>>> m_Connections;
        Dictionary<GNode, Dictionary<GNode, bool>> m_ConnectionsInverse;

        public ConnectionMap()
        {
            m_Connections = new Dictionary<GNode, Dictionary<GNode, List<GConnection>>>();
            m_ConnectionsInverse = new Dictionary<GNode, Dictionary<GNode, bool>>();
        }

        private List<GConnection> m_GConnections;

        public Dictionary<GNode, List<GConnection>> From(GNode gnode)
        {
            if (!m_Connections.ContainsKey(gnode)) return new Dictionary<GNode, List<GConnection>>();
            return m_Connections[gnode];
        }

        public Dictionary<GNode, bool> To(GNode gnode)
        {
            if (!m_ConnectionsInverse.ContainsKey(gnode)) return new Dictionary<GNode, bool>();
            return m_ConnectionsInverse[gnode];
        }

        public void Add(GConnection conn)
        {
            if (!m_Connections.ContainsKey(conn.Start)) 
            {
                m_Connections.Add(conn.Start, new Dictionary<GNode, List<GConnection>>());
            }
            if (!m_ConnectionsInverse.ContainsKey(conn.End))
            {
                m_ConnectionsInverse.Add(conn.End, new Dictionary<GNode, bool>());
            }
            if (!m_Connections[conn.Start].ContainsKey(conn.End))
            {
                m_Connections[conn.Start].Add(conn.End, new List<GConnection>());
            }
            bool unique = true;
            foreach (GConnection c in m_Connections[conn.Start][conn.End])
            {
                if (c.StartPort == conn.StartPort && c.EndPort == conn.EndPort)
                {
                    unique = false;
                    break;
                }
            }
            if (unique) m_Connections[conn.Start][conn.End].Add(conn);
            if (!m_ConnectionsInverse[conn.End].ContainsKey(conn.Start))
            {
                m_ConnectionsInverse[conn.End].Add(conn.Start, true);
            }
        }

        public void Remove(GConnection conn)
        {
            if (m_Connections.ContainsKey(conn.Start))
            {
                if (m_Connections[conn.Start].ContainsKey(conn.End))
                {
                    if (m_Connections[conn.Start][conn.End].Count <= 1)
                    {
                        m_Connections[conn.Start].Remove(conn.End);
                        if (m_ConnectionsInverse.ContainsKey(conn.End)) m_ConnectionsInverse[conn.End].Remove(conn.Start);
                    }
                    else
                        m_Connections[conn.Start][conn.End].Remove(conn);
                }
            }
        }

        public void Remove(GNode from, GNode to)
        {
            if (m_Connections.ContainsKey(from)) m_Connections[from].Remove(to);
            if (m_ConnectionsInverse.ContainsKey(to)) m_ConnectionsInverse[to].Remove(from);
        }

        public void RemoveNode(GNode gnode)
        {
            GNode[] todel = new GNode[From(gnode).Keys.Count];
            From(gnode).Keys.CopyTo(todel, 0);

            if (m_Connections.ContainsKey(gnode))
                m_Connections.Remove(gnode);

            foreach (GNode n in todel)
            {
                if (m_ConnectionsInverse.ContainsKey(n))
                {
                    m_ConnectionsInverse[n].Remove(gnode);
                }
            }

            todel = new GNode[To(gnode).Keys.Count];
            To(gnode).Keys.CopyTo(todel, 0);

            if (m_ConnectionsInverse.ContainsKey(gnode))
                m_ConnectionsInverse.Remove(gnode);

            foreach (GNode n in todel)
            {
                if (m_Connections.ContainsKey(n))
                {
                    m_Connections[n].Remove(gnode);
                }
            }

        }
    }

    public class GProcess
    {
        private GProject m_ParentProject;

        Dictionary<string, GNode> m_GNodes;
        ConnectionMap m_Connections;
        GElement m_selection;

        Rectangle m_boundingBox;

        public Rectangle BoundingBox { get { return m_boundingBox; } }

        public GConnection Connecting;
        public GNodeStub EndPoint;

        Process m_Process;

        public Process Base
        {
            get { return m_Process; }
        }

        public GProject GProject
        {
            get { return m_ParentProject; }
        }

        public GProcess(GProject gproject)
        {
            m_ParentProject = gproject;
            m_GNodes = new Dictionary<string, GNode>();
            m_Connections = new ConnectionMap();
        }

        public void ModelChanged()
        {
            FindBoundingBox();
        }

        public void Draw(Graphics gr, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height)
        {
            if (Zoom <= 0) return;

            gr.TranslateTransform(-Xpos, -Ypos);
            gr.ScaleTransform((float)Zoom, (float)Zoom);

            Xpos = (int)(Xpos / Zoom);
            Ypos = (int)(Ypos / Zoom);
            Width = (int)(Width / Zoom);
            Height = (int)(Height / Zoom);

            foreach (GNode gnode in m_GNodes.Values)
            {
                gnode.AfterChange();
            }
            
            foreach (GNode gnode in m_GNodes.Values)
            {
                Dictionary<GNode, List<GConnection>> conns = m_Connections.From(gnode);

                List<GConnection> toResolve = new List<GConnection>();
                
                foreach (GNode to in conns.Keys)
                {
                    foreach (GConnection c in conns[to])
                    {
                        try
                        {
                            if (c != m_selection)
                                c.Draw(gr, false, Zoom, Xpos, Ypos, Width, Height);
                        }catch
                        {
                            toResolve.Add(c);
                        }
                    }
                }

                foreach (GConnection c in toResolve)
                {
                    m_Connections.Remove(c);
                }
            }
            foreach (GNode gnode in m_GNodes.Values)
            {
                if (gnode != m_selection)
                    gnode.Draw(gr, false, Zoom, Xpos, Ypos, Width, Height);
            }

            if (m_selection != null)
                m_selection.Draw(gr, true, Zoom, Xpos, Ypos, Width, Height);

            if (Connecting != null)
                Connecting.Draw(gr, false, Zoom, Xpos, Ypos, Width, Height);
        }
        public bool Clicked(VisualTool Tool, double Zoom, Double Xpos, Double Ypos)
        {
            Xpos = Xpos / Zoom;
            Ypos = Ypos / Zoom;

            Object oldSelection = m_selection;
            m_selection = null;

            foreach (GNode gnode in m_GNodes.Values)
            {
                if (gnode.Clicked(Tool, oldSelection == gnode, Zoom, Xpos, Ypos))
                {
                    m_selection = gnode;
                }
            }

            foreach (GNode gnode in m_GNodes.Values)
            {
                Dictionary<GNode, List<GConnection>> conns = m_Connections.From(gnode);
                foreach (GNode to in conns.Keys)
                {
                    foreach (GConnection c in conns[to])
                    {
                        if (c.Clicked(Tool, oldSelection == c, Zoom, Xpos, Ypos))
                        {
                            m_selection = c;
                        }
                    }
                }
            }

            return m_selection != null;
        }

        public void MouseMoved(VisualTool Tool, Double Zoom, Double newX, Double newY)
        {
            newX = newX / Zoom;
            newY = newY / Zoom;

            if (m_selection != null)
            {
                m_selection.Move(Tool, (int)(newX - m_selection.MouseOffset.X), (int)(newY - m_selection.MouseOffset.Y));
            }
        }

        public void MouseUp(VisualTool Tool)
        {
            if (Tool == VisualTool.Connect && Connecting != null)
            {
                if (Connecting.Start != EndPoint && Connecting.End != EndPoint)
                {
                    m_Connections.Add(Connecting);
                }

                Connecting = null;
            }
        }

        private void AddConnections(Element gobj)
        {
            string[] connections = gobj.Meta["connections"].Split(';');
            foreach (string connection in connections)
            {
                string[] parameters = connection.Split(',');
                if (parameters.Length != 3) continue;
                string nodename = parameters[0];
                string fromPort = parameters[1];
                string toPort = parameters[2];

                if (m_GNodes.ContainsKey(nodename))
                {
                    GConnection c = new GConnection(m_GNodes[gobj.Name], m_GNodes[nodename], fromPort, toPort, this);
                    m_Connections.Add(c);
                }
            }
        }

        public void Load(Project project, Process process)
        {
            m_Process = process;

            foreach (Variable var in m_Process.VarsList)
            {
                m_GNodes.Add(var.Name, new GNodeVariable(var, this));
            }

            foreach (Node node in m_Process.NodesList)
            {
                GNode n;
                if (node.NodeIsSubprocess) n = new GNodeSubprocess(node, this);
                else n = new GNodeFunction(node, this);
                n.Name = node.Name;

                m_GNodes.Add(node.Name, n);
            }

            foreach (Node node in m_Process.NodesList)
            {
                AddConnections(node);

                foreach (string gnodename in node.NextNodesList)
                {
                    Node gto = m_Process.FindNode(gnodename);
                    if (gto != null)
                    {
                        GConnection c = new GConnection(m_GNodes[node.Name], m_GNodes[gto.Name], this);
                        m_Connections.Add(c);
                    }
                }
            }

            foreach (Variable gvar in m_Process.VarsList)
            {
                AddConnections(gvar);
            }

            FindBoundingBox();
        }

        public void BeforeSave()
        {
            foreach (GNode gnode in m_GNodes.Values)
            {
                gnode.Base.Meta.Clear();
                if (gnode is GNodeSubprocess) 
                {
                    Node node = (gnode.Base as Node);
                    node.Function = "";

                    foreach (ProcessParameter parameter in node.SubprocessParameters)
                    {
                        node.RemoveParameter(parameter.Name);
                    }
                }
            }

            foreach (GNode gnode in m_GNodes.Values)
            {
                gnode.BeforeSave();

                StringBuilder connections = new StringBuilder();

                Dictionary<GNode, List<GConnection>> conns = m_Connections.From(gnode);
                foreach (GNode to in conns.Keys)
                {
                    foreach (GConnection c in conns[to])
                    {
                        c.BeforeSave();

                        if (connections.Length > 0) connections.Append(';');
                        connections.Append(c.End.Name + "," + c.StartPort + "," + c.EndPort);
                    }
                }

                gnode.Base.Meta["connections"] = connections.ToString();
            }
        }

        public GElement SelectedElement
        {
            get
            {
                return m_selection;
            }
            set
            {
                m_selection = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Process.Name;
            }
            set
            {
                if (m_ParentProject.RenameProcess(m_Process.Name, value)) m_Process.Name = value;
            }
        }

        private void FindBoundingBox()
        {
            Rectangle r = new Rectangle();
            bool first = true;

            foreach (GNode gnode in m_GNodes.Values)
            {
                if (first)
                {
                    first = false;
                    r = gnode.BoundingBox();
                }
                else
                    r = Rectangle.Union(r, gnode.BoundingBox());

                Dictionary<GNode, List<GConnection>> conns = m_Connections.From(gnode);
                foreach (GNode to in conns.Keys)
                {
                    foreach (GConnection c in conns[to])
                    {
                        r = Rectangle.Union(r, c.BoundingBox());
                    }
                }
            }

            m_boundingBox = r;
        }

        public void PerformDelete()
        {
            if (m_selection == null) return;

            if (m_selection is GConnection)
            {
                GNode nS = (m_selection as GConnection).Start;
                GNode nE = (m_selection as GConnection).End;
                m_Connections.Remove(m_selection as GConnection);
                nS.RemoveConnection(nE.Name);
            }
            else if (m_selection is GNode)
            {
                GNode gnode = m_selection as GNode;

                m_Connections.RemoveNode(gnode);
                m_GNodes.Remove(gnode.Name);
                if (gnode is GNodeVariable) m_Process.RemoveVar(gnode.Name);
                else m_Process.RemoveNode(gnode.Name);
            }
            m_selection = null;
        }

        internal void RenameProcess(string oldValue, string newValue)
        {
            foreach (GNode gnode in m_GNodes.Values)
            {
                if (gnode is GNodeSubprocess && (gnode.Base as Node).Subprocess == oldValue)
                    (gnode.Base as Node).Subprocess = newValue;
            }
        }

        public void AddVariable(double Zoom, Double Xpos, Double Ypos)
        {
            Xpos = Xpos / Zoom;
            Ypos = Ypos / Zoom;

            string prefix = "v";
            int suffix = 1;
            while (m_GNodes.ContainsKey(prefix + suffix.ToString())) suffix++;
            m_Process.NewVar(prefix + suffix.ToString());
            Variable v = m_Process.FindVar(prefix + suffix.ToString());
            v.Meta["posX"] = Xpos.ToString();
            v.Meta["posY"] = Ypos.ToString();
            m_GNodes.Add(v.Name, new GNodeVariable(v, this));
        }

        public void AddNodeCode(double Zoom, Double Xpos, Double Ypos)
        {
            Xpos = Xpos / Zoom;
            Ypos = Ypos / Zoom;

            string prefix = "f";
            int suffix = 1;
            while (m_GNodes.ContainsKey(prefix + suffix.ToString())) suffix++;
            m_Process.NewNode(prefix + suffix.ToString());
            Node n = m_Process.FindNode(prefix + suffix.ToString());
            n.NodeIsSubprocess = false;
            n.Meta["posX"] = Xpos.ToString();
            n.Meta["posY"] = Ypos.ToString();
            m_GNodes.Add(n.Name, new GNodeFunction(n, this));
        }

        public void AddNodeSubprocess(double Zoom, Double Xpos, Double Ypos)
        {
            Xpos = Xpos / Zoom;
            Ypos = Ypos / Zoom;

            string prefix = "c";
            int suffix = 1;
            while (m_GNodes.ContainsKey(prefix + suffix.ToString())) suffix++;
            m_Process.NewNode(prefix + suffix.ToString());
            Node n = m_Process.FindNode(prefix + suffix.ToString());
            n.NodeIsSubprocess = true;
            n.Meta["posX"] = Xpos.ToString();
            n.Meta["posY"] = Ypos.ToString();
            m_GNodes.Add(n.Name, new GNodeSubprocess(n, this));
        }
    }
}
