using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;

namespace ILNET.Editor
{
    public class GNodeVariable : GNode
    {
        Variable m_var;

        public GNodeVariable(Variable var, GProcess gprocess) : base(gprocess)
        {
            m_var = var;
            AfterLoad();
        }

        public override Element Base
        {
            get { return m_var; }
        }

        public override String Name { get { return m_var.Name; } set { m_var.Name = value; } }

        public override void Draw(Graphics gr, bool selected, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height)
        {
            int W = 30;
            int H = 30;

            gr.FillEllipse(new SolidBrush(selected ? Color.SkyBlue : Color.White), X - W / 2, Y - H / 2, W, H);
            gr.DrawEllipse(new Pen(Color.Black), X - W / 2, Y - H / 2, W, H);

            Font f = new Font("Arial", 10);
            SizeF m = gr.MeasureString(Name, f);
            gr.DrawString(Name, f, new SolidBrush(Color.Black), X - m.Width / 2, Y - H / 2 - m.Height - 2);
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
            
        }
    }
}
