using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ILNET.Elements;

// Osnoven abstrakten klas, koito se nasledqva ot vsi4ki obekti v modela
// Toi sudurja koda za iz4islenie na formuli

namespace ILNET.Editor
{
    public enum NodePortDirection { Input, Output };

    public abstract class GNode : GElement
    {
        public abstract String Name { get; set; }
        public abstract Point PortPoint(NodePortDirection direction, string port);
        public abstract void RemoveConnection(String destination);

        public GNode(GProcess gprocess) : base(gprocess) { }

        internal void DrawArrows(Graphics gr, bool input, bool output)
        {
            Pen penBlack = new Pen(Color.Black);

            if (input)
            {
                Point arrowEnd = this.PortPoint(NodePortDirection.Input, "");

                penBlack.Width = 2;
                gr.DrawLine(penBlack, arrowEnd.X - 20, arrowEnd.Y, arrowEnd.X, arrowEnd.Y);
                gr.DrawLine(penBlack, arrowEnd.X - 10, arrowEnd.Y - 5, arrowEnd.X - 5, arrowEnd.Y);
                gr.DrawLine(penBlack, arrowEnd.X - 10, arrowEnd.Y + 5, arrowEnd.X - 5, arrowEnd.Y);
                penBlack.Width = 1;
            }

            if (output)
            {
                Point arrowStart = this.PortPoint(NodePortDirection.Output, "");

                penBlack.Width = 2;
                gr.DrawLine(penBlack, arrowStart.X, arrowStart.Y, arrowStart.X + 20, arrowStart.Y);
                gr.DrawLine(penBlack, arrowStart.X + 15, arrowStart.Y - 5, arrowStart.X + 20, arrowStart.Y);
                gr.DrawLine(penBlack, arrowStart.X + 15, arrowStart.Y + 5, arrowStart.X + 20, arrowStart.Y);
                penBlack.Width = 1;
            }
        }
        internal void RenameNode(string newValue)
        {
            string oldValue = Name;
            Process c = (Base as Node).ParentProcess;
            foreach (Node node in c.NodesList)
            {
                if (node.FindNextNode(oldValue))
                {
                    node.RemoveNextNode(oldValue);
                    node.NewNextNode(newValue);
                }
            }
            if (c.EntryNode == oldValue) c.EntryNode = newValue;
            if (c.ExitNode == oldValue) c.ExitNode = newValue;
        }
    }
}
