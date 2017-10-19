using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

using ILNET;
using ILNET.Elements;
using ILNET.Editor;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ILNET.Client.Windows
{
    public partial class GraphDoc : DockContent, IToolstripInterface, IPropertyGridInterface
    {
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, String lpszFile);
        [DllImport("gdi32.dll")]
        static extern int DeleteEnhMetaFile(IntPtr hemf);
        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int left, top, right, bottom;}
        [DllImport("gdi32")]
        static extern bool PlayEnhMetaFile(IntPtr hDC, IntPtr hMF, ref RECT lpRect);

        public delegate void Notify();

        [DefaultPropertyAttribute("Name")]
        public class ProcessProperties
        {
            GProcess m_gprocess;

            public event Notify OnRefresh;
            public event Notify OnNameChange;

            public ProcessProperties(GProcess gprocess)
            {
                m_gprocess = gprocess;
            }

            [CategoryAttribute("Design"), DisplayNameAttribute("(Name)"), DescriptionAttribute("Name of the process")]
            public string Name
            {
                get
                {
                    return m_gprocess.Name;
                }
                set
                {
                    m_gprocess.Name = value;
                    OnRefresh.Invoke();
                    OnNameChange.Invoke();
                }
            }

            [CategoryAttribute("Workflow"), DescriptionAttribute("Indicated the start node of the workflow")]
            public string Entry
            {
                get
                {
                    return (m_gprocess.Base as Process).EntryNode;
                }
                set
                {
                    (m_gprocess.Base as Process).EntryNode = value;
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Workflow"), DescriptionAttribute("Indicated the end node of the workflow")]
            public string Exit
            {
                get
                {
                    return (m_gprocess.Base as Process).ExitNode;
                }
                set
                {
                    (m_gprocess.Base as Process).ExitNode = value;
                    OnRefresh.Invoke();
                }
            }
        }

        [DefaultPropertyAttribute("Name")]
        public class NodeVariableProperties
        {
            GNode m_var;

            public event Notify OnRefresh;

            public NodeVariableProperties(GNodeVariable p_var)
            {
                m_var = p_var;
            }


            [CategoryAttribute("Design"), DisplayNameAttribute("(Name)"), DescriptionAttribute("Name of the variable")]
            public string Name
            {
                get
                {
                    return m_var.Name;
                }
                set
                {
                    m_var.Name = value;
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Design"), DescriptionAttribute("Indicated the scope of the variable")]
            public VariableLevel Scope
            {
                get
                {
                    return (m_var.Base as Variable).Level;
                }
                set
                {
                    (m_var.Base as Variable).Level = value;
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Data"), DescriptionAttribute("Type of the variable")]
            public string Type
            {
                get
                {
                    return (m_var.Base as Variable).Type;
                }
                set
                {
                    (m_var.Base as Variable).Type = value;
                    OnRefresh.Invoke();
                }
            }

            [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
            [CategoryAttribute("Data"), DescriptionAttribute("Initial value of the variable")]
            public string Value
            {
                get
                {
                    return (m_var.Base as Variable).Value;
                }
                set
                {
                    (m_var.Base as Variable).Value = value;
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Data"), DescriptionAttribute("Indicated whether the variable is an input parameter")]
            public bool Input
            {
                get
                {
                    return (m_var.Base as Variable).IsInput;
                }
                set
                {
                    (m_var.Base as Variable).IsInput = value;
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Data"), DescriptionAttribute("Indicated whether the variable is an output parameter")]
            public bool Output
            {
                get
                {
                    return (m_var.Base as Variable).IsOutput;
                }
                set
                {
                    (m_var.Base as Variable).IsOutput = value;
                    OnRefresh.Invoke();
                }
            }
        }

        public class NodeProperties
        {
            internal GNode m_gnode;

            public event Notify OnRefresh;

            public void Refresh() { OnRefresh.Invoke(); }

            [CategoryAttribute("Design"), DisplayNameAttribute("(Name)"), DescriptionAttribute("Name of the node")]
            public string Name
            {
                get
                {
                    return m_gnode.Name;
                }
                set
                {
                    m_gnode.Name = value;
                    Refresh();
                }
            }

            [CategoryAttribute("Workflow"), DescriptionAttribute("Indicated whether the node is the start of the workflow")]
            public bool Entry
            {
                get
                {
                    return m_gnode.Name == (m_gnode.Base as Node).ParentProcess.EntryNode;
                }
                set
                {
                    if (value)
                        (m_gnode.Base as Node).ParentProcess.EntryNode = m_gnode.Name;
                    else
                        (m_gnode.Base as Node).ParentProcess.EntryNode = "";
                    OnRefresh.Invoke();
                }
            }

            [CategoryAttribute("Workflow"), DescriptionAttribute("Indicated whether the node is the end of the workflow")]
            public bool Exit
            {
                get
                {
                    return m_gnode.Name == (m_gnode.Base as Node).ParentProcess.ExitNode;
                }
                set
                {
                    if (value)
                        (m_gnode.Base as Node).ParentProcess.ExitNode = m_gnode.Name;
                    else
                        (m_gnode.Base as Node).ParentProcess.ExitNode = "";
                    OnRefresh.Invoke();
                }
            }
        }

        [DefaultPropertyAttribute("Name")]
        public class NodeFunctionProperties : NodeProperties
        {
            public NodeFunctionProperties(GNodeFunction p_var)
            {
                m_gnode = p_var;
            }

            [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
            [CategoryAttribute("Function"), DescriptionAttribute("Initial value of the variable")]
            public string Code
            {
                get
                {
                    return (m_gnode.Base as Node).Function;
                }
                set
                {
                    (m_gnode.Base as Node).Function = value;
                    Refresh();
                }
            }

        }

        [DefaultPropertyAttribute("Value")]
        public class NodeSubprocessParameterProperties
        {
            internal GNodeSubprocess m_node;
            string m_param;

            public event Notify OnRefresh;

            public void Refresh() { OnRefresh.Invoke(); }

            public NodeSubprocessParameterProperties(GNodeSubprocess gnode, string parameter)
            {
                m_node = gnode;
                m_param = parameter;
            }

            [CategoryAttribute("Design"), DisplayNameAttribute("(Name)"), DescriptionAttribute("Name of the node")]
            public string Name
            {
                get
                {
                    return m_param;
                }
            }

            [CategoryAttribute("Data"), DescriptionAttribute("Type of the parameter")]
            public string Type
            {
                get
                {
                    Node node = m_node.Base as Node;
                    Process c = node.ParentProcess.ParentProject.FindProcess(node.Subprocess);
                    return c.FindVar(m_param).Type;
                }
            }

            [CategoryAttribute("Data"), DescriptionAttribute("Initial value of the parameter")]
            public string Value
            {
                get
                {
                    return (m_node.Base as Node).FindParameter(m_param).Value;
                }
                //    set
                //    {
                //        (m_node.Base as GNode).FindParameter(m_param).Value = value;
                //        Refresh();
                //    }
            }
        }

        [DefaultPropertyAttribute("Name")]
        public class NodeSubprocessProperties : NodeProperties
        {
            public NodeSubprocessProperties(GNodeSubprocess gnode)
            {
                m_gnode = gnode;
            }

            [CategoryAttribute("Function"), DescriptionAttribute("Initial value of the variable")]
            public string Subprocess
            {
                get
                {
                    return (m_gnode.Base as Node).Subprocess;
                }
                set
                {
                    (m_gnode.Base as Node).Subprocess = value;
                    Refresh();
                }
            }
        }

        Object m_tagIgnoreChangeEvents = new Object();

        GProcess m_gprocess;

        VisualTool m_selectedTool;

        Double m_viewZoom;
        int m_viewX, m_viewY;
        int m_viewW, m_viewH;

        bool m_selection;
        int m_mviewX, m_mviewY;
        bool m_mviewdown;

        ToolStripButton m_tool;

        bool m_selectionMoving;

        public GraphDoc()
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(MainForm_MouseWheel);

            m_tool = tsbToolSelect;

            editorAction.ConfigurationManager.Language = "cs";


        }

        #region IToolstripInterface Members

        public List<ToolStrip> ToolStirps
        {
            get
            {
                List<ToolStrip> list = new List<ToolStrip>();
                list.Add(this.tstripChildTools);
                list.Add(this.tstripFile);
                list.Add(this.tstripProject);
                return list;
            }
        }

        #endregion


        #region IPropertyGridInterface Members

        public event NotifyEvent OnSelectionChanged;

        public event NotifyEvent OnDocumentModified;

        public event NotifyEvent OnDocumentStructureModified;

        public event NotifyEvent OnDocumentSave;

        public event NotifyEvent OnDocumentRun;

        public List<string> ObjectNames
        {
            get { return new List<string>(); }
        }

        public Object SelectedObject
        {
            get
            {
                GProcess c = m_gprocess;
                if (c.SelectedElement == null)
                {
                    ProcessProperties cp = new ProcessProperties(c);
                    cp.OnRefresh += new Notify(this.RefreshAfterChange);
                    cp.OnNameChange += new Notify(this.RefreshAfterStructureChange);
                    return cp;
                }
                GElement e = c.SelectedElement;
                if (e is GNodeVariable)
                {
                    NodeVariableProperties vp = new NodeVariableProperties(e as GNodeVariable);
                    vp.OnRefresh += new Notify(this.RefreshAfterChange);
                    return vp;
                }
                if (e is GNodeFunction)
                {
                    NodeFunctionProperties cp = new NodeFunctionProperties(e as GNodeFunction);
                    cp.OnRefresh += new Notify(this.RefreshAfterChange);
                    return cp;
                }
                if (e is GNodeSubprocess)
                {
                    if ((e as GNodeSubprocess).SelectedParameter.StartsWith("in:"))
                    {
                        NodeSubprocessParameterProperties pp = new NodeSubprocessParameterProperties(e as GNodeSubprocess, (e as GNodeSubprocess).SelectedParameter.Substring(3));
                        pp.OnRefresh += new Notify(this.RefreshAfterChange);
                        return pp;
                    }

                    NodeSubprocessProperties cp = new NodeSubprocessProperties(e as GNodeSubprocess);
                    cp.OnRefresh += new Notify(this.RefreshAfterChange);
                    return cp;
                }

                return null;
            }
        }

        #endregion

        private string m_fileName = "";
        private bool m_changed = true;

        public string FileName
        {
            get { return m_fileName; }
            set
            {
                m_fileName = value;

                this.ToolTipText = value;

                m_changed = false;
            }
        }

        public GProject GProject
        {
            get
            {
                m_gprocess.GProject.BeforeSave();
                return m_gprocess.GProject;
            }
        }

        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            return GetType().ToString() + "," + FileName + "," + Text;
        }

        public void UpdateText()
        {
            this.Text = m_gprocess.Name + (Modified ? "*" : "");
        }

        private void DocumentChanged()
        {
            m_gprocess.ModelChanged();

            OnDocumentModified.Invoke(this);

            if (m_changed) return;

            m_changed = true;
            UpdateText();
        }

        private void DocumentStructureChanged()
        {
            OnDocumentStructureModified.Invoke(this);
        }

        public bool Modified { get { return m_changed; } set { m_changed = value; } }

        private void RefreshAfterChange()
        {
            if (m_selection && (SelectedObject is NodeFunctionProperties))
            {
                NodeFunctionProperties ncp = (NodeFunctionProperties)SelectedObject;

                if (editorAction.Tag != m_tagIgnoreChangeEvents)
                {
                    editorAction.Tag = m_tagIgnoreChangeEvents;
                    editorAction.Text = ncp.Code;
                    editorAction.Tag = null;
                }

                editorAction.Enabled = true;
            }

            DocumentChanged();

            RefreshAll();
        }

        private void RefreshAfterStructureChange()
        {
            DocumentStructureChanged();
        }

        private void RefreshAll()
        {
            pbDisplay.Refresh();
        }

        Metafile DrawToMetafile()
        {
            Metafile metafile;

            MemoryStream stream = new MemoryStream();
            //using (MemoryStream stream = new MemoryStream())
            {
                using (Graphics offScreenBufferGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
                {
                    IntPtr deviceContextHandle = offScreenBufferGraphics.GetHdc();
                    metafile = new Metafile(
                    stream,
                    deviceContextHandle,
                    new RectangleF(0, 0, pbDisplay.Width, pbDisplay.Height),
                    MetafileFrameUnit.Pixel,
                    EmfType.EmfPlusOnly);
                    offScreenBufferGraphics.ReleaseHdc();
                }
            }

            using (Graphics emfGraphics = Graphics.FromImage(metafile))
            {
                emfGraphics.SmoothingMode = SmoothingMode.HighQuality;
                emfGraphics.SetClip(new Rectangle(0, 0, pbDisplay.Width, pbDisplay.Height));
                m_gprocess.Draw(emfGraphics, m_viewZoom, m_viewX, m_viewY, pbDisplay.Width, pbDisplay.Height);
            }

            /*using (var fileStream = File.Create(@"c:\temp\metafile.emf"))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }*/
            
            return metafile;
        }

        private void pbDisplay_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            m_gprocess.Draw(e.Graphics, m_viewZoom, m_viewX, m_viewY, pbDisplay.Width, pbDisplay.Height);
            e.Graphics.ResetTransform();

            return;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            {
                Metafile metafile = DrawToMetafile();
                GraphicsUnit gu = GraphicsUnit.Pixel;
                RectangleF mr = metafile.GetBounds(ref gu);
                e.Graphics.DrawImage(metafile, (int)(mr.X - m_viewZoom), (int)(mr.Y - m_viewZoom));
            }

            e.Graphics.TranslateTransform(-m_viewX - (float)m_viewZoom, -m_viewY - (float)m_viewZoom);
            e.Graphics.ScaleTransform((float)m_viewZoom, (float)m_viewZoom);

            Rectangle r = m_gprocess.BoundingBox;

            Pen p = new Pen(Color.Gray);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            p.DashPattern = new float[] { 1f, 2f };

            int padding = 20;
            r.X -= padding; r.Y -= padding; r.Width += 2 * padding; r.Height += 2 * padding;
            e.Graphics.DrawRectangle(p, r);

            if (lastTick == 0) lastTick = System.Environment.TickCount;
            int currentTick = System.Environment.TickCount;
            if (lastTick + 1000 < currentTick)
            {
                double tfps = frames / (currentTick - lastTick);
                fps = (tfps + fps) / 2.0;
                frames = 0;
                lastTick = currentTick;
            }
            frames++;

            e.Graphics.DrawString((((int)(fps * 10000)) / 10.0).ToString(), this.Font, new SolidBrush(Color.Green), 100, 100);

            pbDisplay.Refresh();
        }

        int lastTick = 0;
        double frames = 0;
        double fps = 0;

        private void pbDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            this.btnFocus.Focus();

            m_mviewdown = false;

            if (e.Button == MouseButtons.Middle)
            {
                tsbToolAnchorAdd.PerformClick();
                m_mviewdown = true;
            }

            if (e.Button == MouseButtons.Left)
            {
                m_mviewdown = true;
            }

            m_mviewX = e.X;
            m_mviewY = e.Y;

            if (m_mviewdown)
            {
                switch (m_tool.Name.Substring(7).ToLower())
                {
                    case "select":
                        m_selectedTool = m_gprocess.GProject.GetTool("Select");
                        break;
                    case "connect":
                        m_selectedTool = m_gprocess.GProject.GetTool("Connect");
                        break;
                    case "anchoradd":
                        m_selectedTool = m_gprocess.GProject.GetTool("AnchorAdd");
                        break;
                    case "anchordelete":
                        m_selectedTool = m_gprocess.GProject.GetTool("AnchorDelete");
                        break;
                    case "anchorcollapse":
                        m_selectedTool = m_gprocess.GProject.GetTool("AnchorCollapse");
                        break;
                    case "variable":
                        m_gprocess.AddVariable(m_viewZoom, m_viewX + m_mviewX, m_viewY + m_mviewY);
                        tsbToolSelect.PerformClick();
                        m_selectedTool = m_gprocess.GProject.GetTool("Select");
                        break;
                    case "function":
                        m_gprocess.AddNodeCode(m_viewZoom, m_viewX + m_mviewX, m_viewY + m_mviewY);
                        tsbToolSelect.PerformClick();
                        m_selectedTool = m_gprocess.GProject.GetTool("Select");
                        break;
                    case "subprocess":
                        m_gprocess.AddNodeSubprocess(m_viewZoom, m_viewX + m_mviewX, m_viewY + m_mviewY);
                        tsbToolSelect.PerformClick();
                        m_selectedTool = m_gprocess.GProject.GetTool("Select");
                        break;
                }

                // check selection
                m_selection = m_gprocess.Clicked(m_selectedTool, m_viewZoom, m_viewX + m_mviewX, m_viewY + m_mviewY);
                OnSelectionChanged.Invoke(this);
                m_selectionMoving = false;

                if (m_selection && (SelectedObject is NodeFunctionProperties))
                {
                    NodeFunctionProperties ncp = (NodeFunctionProperties)SelectedObject;

                    editorAction.Tag = m_tagIgnoreChangeEvents;
                    editorAction.Text = ncp.Code;
                    editorAction.Tag = null;

                    editorAction.Enabled = true;
                }
                else
                {
                    editorAction.Enabled = false;
                    editorAction.Tag = m_tagIgnoreChangeEvents;
                    editorAction.Text = "";
                    editorAction.Tag = null;
                }

                RefreshAll();
            }
        }

        private void pbDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_mviewdown)
            {
                if (m_selection)
                {
                    if (Math.Sqrt(Math.Pow(e.X - m_mviewX, 2) + Math.Pow(e.Y - m_mviewY, 2)) > 3 || m_selectionMoving)
                    {
                        m_selectionMoving = true;

                        m_gprocess.MouseMoved(m_selectedTool, m_viewZoom, m_viewX + e.X, m_viewY + e.Y);

                        m_mviewX = e.X;
                        m_mviewY = e.Y;
                        DocumentChanged();
                    }
                }
                else
                {
                    m_viewX -= e.X - m_mviewX;
                    m_viewY -= e.Y - m_mviewY;
                    m_mviewX = e.X;
                    m_mviewY = e.Y;
                }

                RefreshAll();

            }
        }

        private void pbDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            m_mviewdown = false;

            m_gprocess.MouseUp(m_selectedTool);
            RefreshAll();

            if (e.Button == MouseButtons.Middle)
            {
                tsbToolSelect.PerformClick();
            }
        }

        private void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            double oldZoom = m_viewZoom;

            m_viewZoom += e.Delta / 120 * 0.1;

            if (m_viewZoom < 0.1) m_viewZoom = 0.1;

            m_viewX = (int)((m_viewX + e.X) * (m_viewZoom / oldZoom) - e.X);
            m_viewY = (int)((m_viewY + e.Y) * (m_viewZoom / oldZoom) - e.Y);

            RefreshAll();
        }

        public void LoadProcess(GProcess gprocess)
        {
            m_viewZoom = 1;

            m_gprocess = gprocess;

            UpdateText();
        }

        public void SaveProject()
        {
            m_gprocess.GProject.BeforeSave();

            if (OnDocumentSave != null)
                OnDocumentSave.Invoke(this);
        }

        public void RunProject()
        {
            if (OnDocumentRun != null)
                OnDocumentRun.Invoke(this);
        }

        private void tsbToolSelect_Click(object sender, EventArgs e)
        {
            if (m_tool != sender)
            {
                m_tool.Checked = false;
                m_tool = (ToolStripButton)sender;
                m_tool.Checked = true;
            }
        }

        private void tsbToolDelete_Click(object sender, EventArgs e)
        {
            m_gprocess.PerformDelete();
            DocumentChanged();
            RefreshAll();
        }

        private void tsbFileSave_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void GraphDoc_Shown(object sender, EventArgs e)
        {
            m_viewW = pbDisplay.Width;
            m_viewH = pbDisplay.Height;

            Rectangle r = m_gprocess.BoundingBox;
            m_viewX = (r.Width - m_viewW) / 2 + r.Left;
            m_viewY = (r.Height - m_viewH) / 2 + r.Top;
            RefreshAll();
        }

        private void btnFocus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) tsbToolDelete.PerformClick();
        }

        private void pbDisplay_Resize(object sender, EventArgs e)
        {
            m_viewX += (m_viewW - pbDisplay.Width) / 2;
            m_viewY += (m_viewH - pbDisplay.Height) / 2;

            m_viewW = pbDisplay.Width;
            m_viewH = pbDisplay.Height;

            RefreshAll();
        }

        public string ProcessName
        {
            get { return m_gprocess.Name; }
        }

        private void tsbProcessRun_Click(object sender, EventArgs e)
        {
            if (Modified) SaveProject();
            RunProject();
        }

        private void editorAction_TextChanged(object sender, EventArgs e)
        {
            if (editorAction.Tag == m_tagIgnoreChangeEvents) return;

            if (SelectedObject is NodeFunctionProperties)
            {
                NodeFunctionProperties ncp = (NodeFunctionProperties)SelectedObject;

                editorAction.Tag = m_tagIgnoreChangeEvents;

                ncp.Code = editorAction.Text;

                OnSelectionChanged.Invoke(this);

                editorAction.Tag = null;
            }
        }

        private void tsbToolFunctionEditor_Click(object sender, EventArgs e)
        {
            scView.Panel2Collapsed = !tsbToolFunctionEditor.Checked;
        }
    }
}