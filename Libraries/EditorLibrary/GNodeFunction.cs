using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;

namespace ILNET.Editor
{
    public class GNodeFunction : GNode
    {
        Node m_node;

        public GNodeFunction(Node node, GProcess gprocess)
            : base(gprocess)
        {
            m_node = node;
            AfterLoad();
        }

        public override Element Base
        {
            get { return m_node; }
        }

        public override String Name { get { return m_node.Name; } set { base.RenameNode(value); m_node.Name = value; } }

        public override void Draw(Graphics gr, bool selected, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height)
        {
            int W = 30;
            int H = 30;

            Pen penBlack = new Pen(Color.Black);
            SolidBrush brushSelected = new SolidBrush(selected ? Color.SkyBlue : Color.White);
            SolidBrush brushBlack = new SolidBrush(Color.Black);

            gr.FillRectangle(brushSelected, X - W / 2, Y - H / 2, W, H);
            gr.DrawRectangle(penBlack, X - W / 2, Y - H / 2, W, H);
            //gr.FillEllipse(brushSelected, X - W / 2, Y - H / 2, W, H);
            //gr.DrawEllipse(penBlack, X - W / 2, Y - H / 2, W, H);

            Font f = new Font("Arial", 10);
            SizeF m = gr.MeasureString(Name, f);
            gr.DrawString(Name, f, brushBlack, X - m.Width / 2, Y - H / 2 - m.Height - 2);

            DrawArrows(gr, this.m_node.ParentProcess.EntryNode == this.Name, this.m_node.ParentProcess.ExitNode == this.Name);
        }

        public override Rectangle BoundingBox()
        {
            Graphics gr = GraphicUtils.gr();

            int W = 30;
            int H = 30;

            Rectangle r = new Rectangle(X - W / 2, Y - H / 2, W, H);

            Font f = new Font("Arial", 10);
            SizeF m = gr.MeasureString(Name, f);

            r = Rectangle.Union(r, new Rectangle((int)(X - m.Width / 2), (int)(Y - H / 2 - m.Height - 2), (int)m.Width, (int)m.Height));

            return r;
        }

        public override Point PortPoint(NodePortDirection direction, string port)
        {
            if (direction == NodePortDirection.Input)
            {
                return new Point(X - 15, Y);
            }
            else
            {
                return new Point(X + 15, Y);
            }
        }

        public override void RemoveConnection(string destination)
        {
            m_node.RemoveNextNode(destination);
        }
    }
}
