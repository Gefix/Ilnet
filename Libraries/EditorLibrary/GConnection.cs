using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

using ILNET;
using ILNET.Elements;
using System.IO;
using System.IO.Compression;

namespace ILNET.Editor
{
    struct Anchor
    {
        public Anchor(Point p_origin, Double p_angle, Double p_inputLength, Double p_outputLength)
        {
            Origin = p_origin;
            Angle = p_angle;
            InputLength = p_inputLength;
            OutputLength = p_outputLength;
        }
        public Point Origin;
        public Double Angle;
        public Double InputLength;
        public Double OutputLength;
    }

    public class GConnection : GElement
    {
        const Int32 width = 5;
        const Int32 distance = 20;

        VisualTool m_tool = VisualTool.Select;

        GNode m_Start, m_End;
        string m_StartPort, m_EndPort;

        List<Anchor> m_Anchors;
        int m_SelectedAnchor;

        public GConnection(GNode start, GNode end, GProcess gprocess)
            : base(gprocess)
        {
            Start = start;
            StartPort = "";
            End = end;
            EndPort = "";

            m_Anchors = new List<Anchor>();

            m_SelectedAnchor = -1;

            AfterLoad();
        }

        public GConnection(GNode start, GNode end, string startPort, string endPort, GProcess gprocess)
            : base(gprocess)
        {
            Start = start;
            StartPort = startPort;
            End = end;
            EndPort = endPort;

            m_Anchors = new List<Anchor>();

            m_SelectedAnchor = -1;

            AfterLoad();
        }

        public override Element Base
        {
            get { return m_Start.Base; }
        }

        public override void AfterLoad()
        {
            string metaKey = "anchors," + m_End.Name + "," + m_StartPort + "," + m_EndPort;

            try
            {
                string[] anchors = m_Start.Base.Meta[metaKey].Split(';');
                foreach (string anchor in anchors)
                {
                    string[] values = anchor.Split(',');
                    m_Anchors.Add(new Anchor(
                            new Point(Convert.ToInt32(values[0]),
                                        Convert.ToInt32(values[1])),
                            Convert.ToDouble(values[2], GEF.CultureInfo),
                            Convert.ToDouble(values[3], GEF.CultureInfo),
                            Convert.ToDouble(values[4], GEF.CultureInfo)
                        ));
                }
            }
            catch
            {
                m_Anchors.Add(new Anchor(m_Start.PortPoint(NodePortDirection.Output, m_StartPort), 0, 0, 20));
                m_Anchors.Add(new Anchor(m_End.PortPoint(NodePortDirection.Input,m_EndPort), 0, 20, 0));
            }
        }

        public override void BeforeSave()
        {
            string metaKey = "anchors," + m_End.Name + "," + m_StartPort + "," + m_EndPort;

            StringBuilder anchors = new StringBuilder();
            for (int i = 0; i < m_Anchors.Count; i++)
            {
                Anchor a = m_Anchors[i];
                if (i > 0) anchors.Append(";");
                anchors.Append(a.Origin.X.ToString());
                anchors.Append("," + a.Origin.Y.ToString());
                anchors.Append("," + a.Angle.ToString("0.####", GEF.CultureInfo));
                anchors.Append("," + a.InputLength.ToString("0.####", GEF.CultureInfo));
                anchors.Append("," + a.OutputLength.ToString("0.####", GEF.CultureInfo));
            }

            string result = anchors.ToString();

            m_Start.Base.Meta[metaKey] = result;

            string value = "";

            if (m_Start.Base is Node)
            {
                value = "$" + m_Start.Name + "[\"" + m_StartPort + "\"]";
            }
            else if (m_Start.Base is Variable)
            {
                value = "$" + m_Start.Name;
            }


            if (m_Start.Base is Node && m_StartPort == "")
            {
                Node nStart = m_Start.Base as Node;
                if (!nStart.FindNextNode(m_End.Name)) nStart.NewNextNode(m_End.Name);
            }

            if (m_End.Base is Node && m_EndPort != "")
            {
                Node nEnd = m_End.Base as Node;
                nEnd.NewParameter(m_EndPort);
                ProcessParameter p = nEnd.FindParameter(m_EndPort);
                p.Value = value;
            }

            if (m_End.Base is Variable)
            {
                Node nStart = m_Start.Base as Node;
                nStart.Function += "$" + m_End.Name + " = " + value + ";\n";
            }
        }

        public int SelectedAnchor { get { return m_SelectedAnchor; } }
        public int AnchorCount { get { return m_Anchors.Count * 3 - 2; } }

        public PointF GetAnchorPoint(int anchor)
        {
            if (anchor < 0 || anchor >= m_Anchors.Count * 3 - 2) return new Point();

            anchor++;

            switch (anchor % 3)
            {
                case 0: // input
                    return GetWingPoint(m_Anchors[anchor / 3], true);
                case 1: // origin
                    return m_Anchors[anchor / 3].Origin;
                case 2: // output
                    return GetWingPoint(m_Anchors[anchor / 3], false);
                default:
                    return new Point();
            }
        }

        public void SetAnchorPoint(int anchor, PointF p)
        {
            if (anchor < 0 || anchor >= m_Anchors.Count * 3 - 2) return;

            anchor++;

            Anchor a = m_Anchors[anchor / 3];

            Double angle = 0, length = 0;

            switch (anchor % 3)
            {
                case 0: // input
                    GraphicUtils.CartesianToPolar(new PointF(p.X - a.Origin.X, p.Y - a.Origin.Y), ref angle, ref length);
                    a.InputLength = length;
                    a.Angle = angle - Math.PI;
                    break;
                case 1: // origin
                    a.Origin = new Point((int)p.X, (int)p.Y);
                    break;
                case 2: // output
                    GraphicUtils.CartesianToPolar(new PointF(p.X - a.Origin.X, p.Y - a.Origin.Y), ref angle, ref length);
                    a.OutputLength = length;
                    a.Angle = angle;
                    break;
            }
            m_Anchors[anchor / 3] = a;
        }

        public override int X 
        {
            get
            {
                if (m_SelectedAnchor < 0 || m_SelectedAnchor >= m_Anchors.Count * 3 - 2)
                    return Math.Min(m_Start.X, m_End.X);
                else
                {
                    return (int)GetAnchorPoint(m_SelectedAnchor).X;
                }
            }
            set 
            {
                SetAnchorPoint(m_SelectedAnchor, new PointF(value, GetAnchorPoint(m_SelectedAnchor).Y));
            } 
        }
        public override int Y 
        {
            get 
            {
                if (m_SelectedAnchor < 0)
                    return Math.Min(m_Start.Y, m_End.Y);
                else
                    return (int)GetAnchorPoint(m_SelectedAnchor).Y;
            } 
            set 
            {
                if (m_SelectedAnchor >= 0)
                {
                    SetAnchorPoint(m_SelectedAnchor, new PointF(GetAnchorPoint(m_SelectedAnchor).X, value));
                }
            } 
        }

        public GNode Start { get { return m_Start; } set { m_Start = value; } }
        public GNode End { get { return m_End; } set { m_End = value; } }

        public string StartPort { get { return m_StartPort; } set { m_StartPort = value; } }
        public string EndPort { get { return m_EndPort; } set { m_EndPort = value; } }

        private PointF GetWingPoint(Anchor anchor, bool isInput)
        {
            Double angle = anchor.Angle;
            Double length = (isInput ? anchor.InputLength : anchor.OutputLength);
            if (isInput) angle = angle + Math.PI;
            return new PointF((float)(anchor.Origin.X + length * Math.Cos(angle)), (float)(anchor.Origin.Y + length * Math.Sin(angle)));
        }

        public override void Draw(Graphics gr, bool selected, Double Zoom, Int32 Xpos, Int32 Ypos, Int32 Width, Int32 Height)
        {
            Anchor startAnchor = m_Anchors[0];
            startAnchor.Origin = Start.PortPoint(NodePortDirection.Output, m_StartPort);
            m_Anchors[0] = startAnchor;
            Anchor endAnchor = m_Anchors[m_Anchors.Count - 1];
            endAnchor.Origin = End.PortPoint(NodePortDirection.Input, m_EndPort);
            m_Anchors[m_Anchors.Count - 1] = endAnchor;

            List<PointF> points = new List<PointF>();

            for (int i = 0; i < m_Anchors.Count; i++)
            {
                Anchor p = m_Anchors[i];

                if (i > 0)
                    points.Add(GetWingPoint(p, true));
                
                points.Add(p.Origin);

                if (i < m_Anchors.Count - 1)
                    points.Add(GetWingPoint(p, false));
            }

            //gr.DrawBeziers(new Pen(selected ? Color.SkyBlue : Color.Black, 2), points.ToArray());
            gr.DrawBeziers(new Pen(selected ? Color.SkyBlue : Color.Black, 2), points.ToArray());

            try
            {
                if (selected)
                {
                    for (int i = 0; i < m_Anchors.Count; i++)
                    {
                        Anchor a = m_Anchors[i];
                        PointF p1 = GetWingPoint(a, true);
                        PointF p2 = GetWingPoint(a, false);

                        Pen pen = new Pen(Color.SkyBlue);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gr.DrawLine(pen, a.Origin, p1);
                        gr.DrawLine(pen, a.Origin, p2);

                        gr.FillRectangle(new SolidBrush(Color.SkyBlue), a.Origin.X - 2, a.Origin.Y - 2, 5, 5);
                        if (i > 0)
                            gr.FillRectangle(new SolidBrush(Color.SkyBlue), p1.X - 2, p1.Y - 2, 5, 5);
                        if (i < m_Anchors.Count - 1)
                            gr.FillRectangle(new SolidBrush(Color.SkyBlue), p2.X - 2, p2.Y - 2, 5, 5);
                    }
                    if (m_SelectedAnchor >= 0)
                    {
                        PointF p = GetAnchorPoint(m_SelectedAnchor);
                        gr.FillRectangle(new SolidBrush(Color.Red), p.X - 2, p.Y - 2, 5, 5);
                    }
                }
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        public override bool Clicked(VisualTool Tool, bool Selected, Double Zoom, Double Xpos, Double Ypos)
        {
            m_tool = Tool;

            int radius = 2;
            int zoneWidth = radius * 2 + 1;

            Bitmap bmp = new Bitmap(zoneWidth, zoneWidth);
            Graphics gr = Graphics.FromImage(bmp);

            gr.ScaleTransform((float)Zoom, (float)Zoom);
            gr.TranslateTransform((float)(-Xpos + radius / Zoom), (float)(-Ypos + radius / Zoom));

            gr.Clear(Color.FromArgb(0, Color.Black));

            Draw(gr, false, Zoom, (int)Xpos, (int)Ypos, 1, 1);

            int alphaSum = 0;
            for (int i = 0; i < zoneWidth; i++)
                for (int j = 0; j < zoneWidth; j++)
                    alphaSum += bmp.GetPixel(i, j).A;

            if (alphaSum > 0 || Selected)
            {
                m_SelectedAnchor = -1;

                // select anchor
                Rectangle region = new Rectangle((int)Xpos - radius, (int)Ypos - radius, zoneWidth, zoneWidth);

                if (Tool == VisualTool.Select)
                {
                    for (int i = 0; i < AnchorCount; i += 3)
                    {
                        PointF p = GetAnchorPoint(i);

                        if (region.Contains((int)p.X, (int)p.Y))
                        {
                            m_SelectedAnchor = i;
                        }
                    }
                }

                if (m_SelectedAnchor < 0)
                {
                    for (int i = 0; i < AnchorCount; i++)
                    {
                        if (i % 3 == 0) continue;

                        PointF p = GetAnchorPoint(i);

                        if (region.Contains((int)p.X, (int)p.Y))
                        {
                            m_SelectedAnchor = i;
                        }
                    }
                }

                if (Tool == VisualTool.AnchorAdd
                    || Tool == VisualTool.AnchorDelete
                    || Tool == VisualTool.AnchorCollapse)
                {
                    if (m_SelectedAnchor < 0)
                    {
                        for (int i = 0; i < AnchorCount; i += 3)
                        {
                            PointF p = GetAnchorPoint(i);

                            if (region.Contains((int)p.X, (int)p.Y))
                            {
                                m_SelectedAnchor = i;
                            }
                        }
                    }

                    if (m_SelectedAnchor < 0 && Tool == VisualTool.AnchorAdd)
                    {
                        // find insertion point

                        Anchor a = new Anchor();
                        a.Origin = new Point((int)Xpos, (int)Ypos);
                        a.InputLength = a.OutputLength = 0;

                        int insertAt = 0;

                        List<PointF> points = new List<PointF>();

                        for (int i = 0; i < m_Anchors.Count - 1; i++)
                        {
                            points.Clear();

                            points.Add(m_Anchors[i].Origin);
                            points.Add(GetWingPoint(m_Anchors[i], false));
                            points.Add(GetWingPoint(m_Anchors[i + 1], true));
                            points.Add(m_Anchors[i + 1].Origin);

                            gr.Clear(Color.FromArgb(0, Color.Black));

                            gr.DrawBeziers(new Pen(Color.Black, 2), points.ToArray());

                            int alphaSum2 = 0;
                            for (int k = 0; k < zoneWidth; k++)
                                for (int j = 0; j < zoneWidth; j++)
                                    alphaSum2 += bmp.GetPixel(k, j).A;

                            if (alphaSum2 > 0)
                            {
                                insertAt = i + 1;
                                break;
                            }
                        }

                        if (insertAt > 0)
                        {
                            m_Anchors.Insert(insertAt, a);
                            m_SelectedAnchor = insertAt * 3;
                        }
                    }

                    else if (m_SelectedAnchor >= 0 && Tool == VisualTool.AnchorDelete)
                    {
                        if (m_SelectedAnchor > 1 && m_SelectedAnchor < AnchorCount - 2)
                        {
                            m_Anchors.RemoveAt((m_SelectedAnchor + 1) / 3);
                            m_SelectedAnchor = -1;
                        }
                    }

                    else if (m_SelectedAnchor >= 0 && Tool == VisualTool.AnchorCollapse)
                    {
                        Anchor a = m_Anchors[(m_SelectedAnchor + 1) / 3];
                        if (a.InputLength == 0 && a.OutputLength == 0)
                        {
                            a.Angle = 0;
                            a.InputLength = a.OutputLength = 20;
                            if (m_SelectedAnchor <= 1) a.InputLength = 0;
                            if (m_SelectedAnchor >= AnchorCount - 2) a.OutputLength = 0;
                        }
                        else
                        {
                            a.InputLength = a.OutputLength = 0;
                        }
                        m_Anchors[(m_SelectedAnchor + 1) / 3] = a;
                        m_SelectedAnchor = ((m_SelectedAnchor + 1) / 3) * 3;
                    }
                }

                return Selected ? m_SelectedAnchor >= 0 || alphaSum > 0 : true;
            }

            return false;
        }

        public override Rectangle BoundingBox()
        {
            RectangleF r = new RectangleF();
            bool first = true;

            for (int i = 0; i < AnchorCount - 3; i += 3)
            {
                PointF p = GetAnchorPoint(i);

                if (first)
                {
                    first = false;
                    r = new RectangleF(p.X, p.Y, 0, 0);
                }
                else
                    r = RectangleF.Union(r, BezierOp.GetRect(GetAnchorPoint(i), GetAnchorPoint(i + 1), GetAnchorPoint(i + 2), GetAnchorPoint(i + 3)));
            }

            return Rectangle.Round(r);
        }

    }
}
