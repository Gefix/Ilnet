using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ILNET.Elements;

namespace ILNET.Editor
{
    public class GNodeStub : GNode
    {
        public GNodeStub(GProcess gprocess) : base(gprocess) { }

        string x;

        public override string Name
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public override Point PortPoint(NodePortDirection direction, string port)
        {
            return new Point(X, Y);
        }

        public override void RemoveConnection(string destination)
        {
            return;
        }

        public override Element Base
        {
            get { return null; }
        }

        public override void Draw(System.Drawing.Graphics gr, bool selected, double Zoom, int Xpos, int Ypos, int Width, int Height)
        {
            return;
        }

        public override Rectangle BoundingBox()
        {
            return new Rectangle();
        }
    }
}
