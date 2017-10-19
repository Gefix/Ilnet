using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;

namespace ILNET.Editor
{
    public class GNodeSubprocess : GNode
    {
        Node m_node;

        Dictionary<string, RectangleF> m_ports;
        string m_selectedPort;

        public GNodeSubprocess(Node node, GProcess gprocess)
            : base(gprocess)
        {
            m_node = node;
            m_selectedPort = "";
            m_ports = new Dictionary<string, RectangleF>();
            AfterLoad();
        }

        public override Element Base
        {
            get { return m_node; }
        }

        public override String Name { get { return m_node.Name; } set { base.RenameNode(value); m_node.Name = value; } }

        private void PortsRefresh(Graphics gr)
        {
            Font f = new Font("Arial", 10);

            Process c = m_node.ParentProcess.ParentProject.FindProcess(m_node.Subprocess);
            if (c == null) return;

            float W = 0;
            float H = 0;
            int row_height = 0, ins = 0, outs = 0;
            GetBodySize(ref W, ref H, ref row_height, ref ins, ref outs);

            m_ports = new Dictionary<string, RectangleF>();

            RectangleF r;
            float w;

            string arrow = "\x25B6";
            
            w = Math.Max(10, gr.MeasureString(arrow, f).Width);
            r = new RectangleF((int)(X - W / 2), (int)(Y - H / 2 + 10), w, row_height);
            m_ports.Add("in", r);

            w = Math.Max(10, gr.MeasureString(arrow, f).Width);
            r = new RectangleF((int)(X + W / 2) - w, (int)(Y - H / 2 + 10), w, row_height);
            m_ports.Add("out", r);

            ins = outs = 1;

            foreach (Variable v in c.VarsList)
            {
                if (v.IsInput)
                {
                    w = Math.Max(10, gr.MeasureString(v.Name, f).Width);
                    r = new RectangleF((int)(X - W / 2), (int)(Y - H / 2 + 10 + (10 + row_height) * ins), w, row_height);
                    m_ports.Add("in:" + v.Name, r);
                    ins++;
                }
                if (v.IsOutput)
                {
                    w = Math.Max(10, gr.MeasureString(v.Name, f).Width);
                    r = new RectangleF((int)(X + W / 2) - w, (int)(Y - H / 2 + 10 + (10 + row_height) * outs), w, row_height);
                    m_ports.Add("out:" + v.Name, r);
                    outs++;
                }
            }
        }

        public override void AfterChange()
        {
            PortsRefresh(GraphicUtils.gr());
        }

        public override void Draw(Graphics gr, bool selected, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height)
        {
            float W;
            float H;

            Process c = m_node.ParentProcess.ParentProject.FindProcess(m_node.Subprocess);

            Font f = new Font("Arial", 10);
            Pen penBlack = new Pen(Color.Black);
            SolidBrush brushSelected = new SolidBrush(selected ? Color.SkyBlue : Color.White);
            SolidBrush brushBlack = new SolidBrush(Color.Black);

            if (c == null)
            {
                W = 30;
                H = 30;

                gr.FillRectangle(brushSelected, X - W / 2, Y - H / 2, W, H);
                gr.DrawRectangle(penBlack, X - W / 2, Y - H / 2, W, H);

                gr.DrawString(Name, f, brushBlack, X - gr.MeasureString(Name, f).Width / 2, Y + H / 2 + 2);

                return;
            }

            W = H = 0;
            int row_height = 0, ins = 0, outs = 0;
            GetBodySize(ref W, ref H, ref row_height, ref ins, ref outs);

            gr.FillRectangle(brushSelected, (int)(X - W / 2 + 10), (int)(Y - H / 2), (int)W - 20, (int)H);
            gr.DrawRectangle(penBlack, (int)(X - W / 2 + 10), (int)(Y - H / 2), (int)W - 20, (int)H);

            ins = outs = 1;

            foreach (string port in m_ports.Keys)
            {
                if (port.StartsWith("in:")) ins++;
                else if (port.StartsWith("out:")) outs++;

                string portname = "\x25B6";

                if (port.IndexOf(':') > 0) portname = port.Substring(port.IndexOf(':') + 1);

                RectangleF r = m_ports[port];
                gr.FillRectangle(brushSelected, r);
                gr.DrawRectangle(penBlack, r.Left, r.Top, r.Width, r.Height);
                gr.DrawString(portname, f, brushBlack, r);
            }

            float nameWidth = gr.MeasureString(Name, f).Width;
            gr.DrawString(Name, f, brushBlack, X - nameWidth / 2, Y + H / 2 + 2);

            float processNameWidth = gr.MeasureString(c.Name, f).Width;
            gr.DrawString(c.Name, f, brushBlack, X - processNameWidth / 2, Y - H / 2 + 10);

            DrawArrows(gr, this.m_node.ParentProcess.EntryNode == this.Name, this.m_node.ParentProcess.ExitNode == this.Name);

            if (m_selectedPort != "")
            {
                Pen pen = new Pen(Color.Red, 2);
                gr.DrawRectangle(pen, m_ports[m_selectedPort].Left, m_ports[m_selectedPort].Top, m_ports[m_selectedPort].Width, m_ports[m_selectedPort].Height);
            }
        }
        private void GetBodySize(ref float W, ref float H, ref int RH, ref int ins, ref int outs)
        {
            Graphics gr = GraphicUtils.gr();

            Process c = m_node.ParentProcess.ParentProject.FindProcess(m_node.Subprocess);

            Font f = new Font("Arial", 10);

            if (c == null)
            {
                W = 30;
                H = 30;
                return;
            }

            ins = outs = 0;

            float inMaxW = 10, outMaxW = 10;

            foreach (Variable v in c.VarsList)
            {
                if (v.IsInput)
                {
                    ins++;
                    float w = gr.MeasureString(v.Name, f).Width;
                    if (w > inMaxW) inMaxW = w;
                }
                if (v.IsOutput)
                {
                    outs++;
                    float w = gr.MeasureString(v.Name, f).Width;
                    if (w > outMaxW) outMaxW = w;
                }
            }

            RH = (int)Math.Round(gr.MeasureString("qyZ", f).Height);

            int rows = Math.Max(ins, outs) + 1;

            H = 10 + rows * (RH + 10);

            float namew = gr.MeasureString(c.Name, f).Width;

            W = Math.Max(inMaxW + outMaxW + 10, namew + 30);
        }
        public override Rectangle BoundingBox()
        {
            Graphics gr = GraphicUtils.gr();

            Font f = new Font("Arial", 10);

            float W = 0, H = 0;
            int row_height = 0, ins = 0, outs = 0;
            GetBodySize(ref W, ref H, ref row_height, ref ins, ref outs);

            Rectangle r = new Rectangle((int)(X - W / 2), (int)(Y - H / 2), (int)W, (int)H);

            float nameWidth = gr.MeasureString(Name, f).Width;
            r = Rectangle.Union(r, new Rectangle((int)(X - nameWidth / 2), (int)(Y + H / 2 + 2), (int)nameWidth, (int)row_height));

            return r;
        }

        public override Point PortPoint(NodePortDirection direction, string port)
        {
            float W = 0, H = 0;
            int row_height = 0, ins = 0, outs = 0;
            GetBodySize(ref W, ref H, ref row_height, ref ins, ref outs);

            if (direction == NodePortDirection.Input)
            {
                if (port == "")
                {
                    return new Point((int)(X - W / 2), (int)(Y - H / 2 + 10 + row_height / 2));
                }
                else
                {
                    return new Point((int)m_ports["in:" + port].X, (int)(m_ports["in:" + port].Y + m_ports["in:" + port].Height / 2));
                }
            }
            else
            {
                if (port == "")
                {
                    return new Point((int)(X + W / 2), (int)(Y - H / 2 + 10 + row_height / 2));
                }
                else
                {
                    return new Point((int)m_ports["out:" + port].Right, (int)(m_ports["out:" + port].Y + m_ports["out:" + port].Height / 2));
                }
            }
        }

        public override bool Clicked(VisualTool Tool, bool selected, Double Zoom, Double Xpos, Double Ypos)
        {
            bool clicked = base.Clicked(Tool, selected, Zoom, Xpos, Ypos);

            m_selectedPort = "";
            if (clicked)
            {
                foreach (KeyValuePair<string, RectangleF> kvp in m_ports)
                {
                    if (kvp.Value.Contains((float)Xpos, (float)Ypos))
                    {
                        m_selectedPort = kvp.Key;
                        break;
                    }
                }
            }

            return clicked;
        }

        public override void RemoveConnection(string destination)
        {
            m_node.RemoveNextNode(destination);
        }

        public string SelectedParameter
        {
            get
            {
                return m_selectedPort;
            }
            set
            {
                m_selectedPort = value;
            }
        }
    }
}
