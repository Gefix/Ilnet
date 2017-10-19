using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ILNET.Elements;

namespace ILNET.Editor
{
    public abstract class GElement
    {
        protected GProcess m_gprocess;

        public GElement(GProcess gprocess)
        {
            m_gprocess = gprocess;
            m_mouseOffset = new Point(0, 0);
        }

        public virtual void AfterLoad()
        {
            try { m_X = Convert.ToInt32(Base.Meta["posX"]); }
            catch { m_X = 0; }
            try { m_Y = Convert.ToInt32(Base.Meta["posY"]); }
            catch { m_Y = 0; }
        }

        public virtual void BeforeSave()
        {
            Base.Meta["posX"] = m_X.ToString();
            Base.Meta["posY"] = m_Y.ToString();
        }

        public virtual void AfterChange() { }

        public abstract Element Base { get; }

        private int m_X, m_Y;

        public virtual int X { get { return m_X; } set { m_X = value; } }
        public virtual int Y { get { return m_Y; } set { m_Y = value; } }

        protected PointF m_mouseOffset;
        public PointF MouseOffset { get { return m_mouseOffset; } }

        public abstract void Draw(Graphics gr, bool selected, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height); // abstraktna funkciq, koqto iz4ertava obekta na ekrana. Definira se ot vseki obekt koito nasledqva Element
        public virtual bool Clicked(VisualTool Tool, bool selected, Double Zoom, Double Xpos, Double Ypos) // proverqva dali to4ka s koortinadi x,y leji vurhu obekta
        {
            bool clicked = BoundingBox().Contains((int)Xpos, (int)Ypos);
            if (clicked)
            {
                m_mouseOffset.X = (float)Xpos - X;
                m_mouseOffset.Y = (float)Ypos - Y;
            }
            return clicked;
        }
        public void Move(VisualTool Tool, Int32 newX, Int32 newY)
        {
            if (Tool == VisualTool.Select)
            {
                X = newX;
                Y = newY;
                return;
            }
            if (Tool == VisualTool.Connect && this is GNode)
            {
                int X = newX + (int)m_mouseOffset.X;
                int Y = newY + (int)m_mouseOffset.Y;

                if (m_gprocess.EndPoint == null)
                    m_gprocess.EndPoint = new GNodeStub(m_gprocess);
                m_gprocess.EndPoint.X = X;
                m_gprocess.EndPoint.Y = Y;
                if (m_gprocess.Connecting == null)
                {
                    if (this is GNodeSubprocess)
                    {
                        string p = (this as GNodeSubprocess).SelectedParameter;
                        if (p.StartsWith("in"))
                        {
                            m_gprocess.Connecting = new GConnection(m_gprocess.EndPoint, this as GNode, m_gprocess);
                            if (p.StartsWith("in:"))
                            {
                                m_gprocess.Connecting.EndPort = p.Substring(3);
                                m_gprocess.EndPoint.Name = "VO";
                            }
                            else
                                m_gprocess.EndPoint.Name = "SO";
                        }
                        else
                        {
                            m_gprocess.Connecting = new GConnection(this as GNode, m_gprocess.EndPoint, m_gprocess);
                            if (p.StartsWith("out:"))
                            {
                                m_gprocess.Connecting.StartPort = p.Substring(4);
                                m_gprocess.EndPoint.Name = "VI";
                            }
                            else
                                m_gprocess.EndPoint.Name = "SI";
                        }
                    }
                    else if (this is GNodeFunction)
                    {
                        m_gprocess.Connecting = new GConnection(this as GNode, m_gprocess.EndPoint, m_gprocess);
                        m_gprocess.EndPoint.Name = "SI";
                    }
                    else if (this is GNodeVariable)
                    {
                        m_gprocess.Connecting = new GConnection(this as GNode, m_gprocess.EndPoint, m_gprocess);
                        m_gprocess.EndPoint.Name = "CI";
                    }
                    else
                        return;
                }

                m_gprocess.Clicked(VisualTool.Select, 1, X, Y);

                bool hasPoint = false;

                if (m_gprocess.SelectedElement != null && m_gprocess.SelectedElement is GNode)
                {
                    if (m_gprocess.EndPoint.Name[0] == 'S')
                    {
                        if (m_gprocess.SelectedElement is GNodeFunction)
                        {
                            if (m_gprocess.EndPoint.Name[1] == 'O')
                            {
                                m_gprocess.Connecting.Start = m_gprocess.SelectedElement as GNode;
                                m_gprocess.Connecting.StartPort = "";
                                hasPoint = true;
                            }
                            else
                            {
                                m_gprocess.Connecting.End = m_gprocess.SelectedElement as GNode;
                                m_gprocess.Connecting.EndPort = "";
                                hasPoint = true;
                            }
                        }
                        else if (m_gprocess.SelectedElement is GNodeSubprocess)
                        {
                            string p = (m_gprocess.SelectedElement as GNodeSubprocess).SelectedParameter;

                            if (m_gprocess.EndPoint.Name[1] == 'O')
                            {
                                if (p == "out" || p == "")
                                {
                                    m_gprocess.Connecting.Start = m_gprocess.SelectedElement as GNode;
                                    m_gprocess.Connecting.StartPort = "";
                                    (m_gprocess.SelectedElement as GNodeSubprocess).SelectedParameter = "out";
                                    hasPoint = true;
                                }
                            }
                            else
                            {
                                if (p == "in" || p == "")
                                {
                                    m_gprocess.Connecting.End = m_gprocess.SelectedElement as GNode;
                                    m_gprocess.Connecting.EndPort = "";
                                    (m_gprocess.SelectedElement as GNodeSubprocess).SelectedParameter = "in";
                                    hasPoint = true;
                                }
                            }
                        }
                    }
                    if (m_gprocess.EndPoint.Name[0] == 'V')
                    {
                        if (m_gprocess.SelectedElement is GNodeVariable)
                        {
                            if (m_gprocess.EndPoint.Name[1] == 'O')
                            {
                                m_gprocess.Connecting.Start = m_gprocess.SelectedElement as GNode;
                                m_gprocess.Connecting.StartPort = "";
                                hasPoint = true;
                            }
                            else
                            {
                                m_gprocess.Connecting.End = m_gprocess.SelectedElement as GNode;
                                m_gprocess.Connecting.EndPort = "";
                                hasPoint = true;
                            }
                        }
                    }
                    if (m_gprocess.EndPoint.Name[0] == 'V' ||m_gprocess.EndPoint.Name[0] == 'C')
                    {
                        if (m_gprocess.SelectedElement is GNodeSubprocess)
                        {
                            string p = (m_gprocess.SelectedElement as GNodeSubprocess).SelectedParameter;

                            if (m_gprocess.EndPoint.Name[1] == 'O')
                            {
                                if (p.StartsWith("out:"))
                                {
                                    m_gprocess.Connecting.Start = m_gprocess.SelectedElement as GNode;
                                    m_gprocess.Connecting.StartPort = p.Substring(4);
                                    hasPoint = true;
                                }
                            }
                            else
                            {
                                if (p.StartsWith("in:"))
                                {
                                    m_gprocess.Connecting.End = m_gprocess.SelectedElement as GNode;
                                    m_gprocess.Connecting.EndPort = p.Substring(3);
                                    hasPoint = true;
                                }
                            }
                        }
                    }
                }
                if (hasPoint == false)
                {
                    m_gprocess.Clicked(VisualTool.Select, 1, Double.MaxValue, Double.MaxValue);

                    if (m_gprocess.EndPoint.Name[1] == 'O')
                        m_gprocess.Connecting.Start = m_gprocess.EndPoint;
                    else
                        m_gprocess.Connecting.End = m_gprocess.EndPoint;
                }

                m_gprocess.SelectedElement = this;
            }
        }
        public abstract Rectangle BoundingBox();
    }
}
