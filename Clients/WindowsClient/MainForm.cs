using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using ILNET;
using ILNET.Elements;
using ILNET.Editor;
using System.Reflection;
using ILNET.Server;
using ServiceStack;

namespace ILNET.Client.Windows
{
    public struct GProcessInfo
    {
        public string pathname;
    }

    public partial class MainForm : Form
    {
        //WebService m_WebService;
        //ILNETServer m_WebService;
        JsonServiceClient m_WebService;

        private RepositoryExplorer m_repositoryExplorer = new RepositoryExplorer();
        private PropertyWindow m_propertyWindow = new PropertyWindow();
        private Toolbox m_toolbox = new Toolbox();

        Dictionary<string, GProject> m_projectObjects;

        public MainForm()
        {
            InitializeComponent();

            m_toolbox.Show(dockPanel);
            m_repositoryExplorer.Show(dockPanel);
            m_propertyWindow.Show(m_repositoryExplorer.Pane, DockAlignment.Bottom, 0.50);

            m_projectObjects = new Dictionary<string, GProject>(StringComparer.OrdinalIgnoreCase);

            m_repositoryExplorer.OnDocumentOpen += new NotifyEvent(repositoryDocumentOpen);
            m_repositoryExplorer.OnDocumentRename += new RenameEvent(repositoryDocumentRename);
            m_repositoryExplorer.OnDocumentDelete += new NotifyEvent(repositoryDocumentDelete);
            m_repositoryExplorer.OnDocumentAddProcess += new NotifyEvent(repositoryDocumentAddProcess);
            m_repositoryExplorer.OnDocumentAddProject += new NotifyEvent(repositoryDocumentAddProject);
            m_repositoryExplorer.OnDocumentAddFolder += new NotifyEvent(repositoryDocumentAddFolder);
            m_repositoryExplorer.OnDocumentMark += new NotifyEvent(repositoryDocumentMark);
            m_repositoryExplorer.OnFolderExpand += new NotifyEvent(repositoryFolderExpand);
        }

        private void SetProjectObject(string name, GProject gproject)
        {
            if (m_projectObjects.ContainsKey(name)) m_projectObjects[name] = gproject;
            else m_projectObjects.Add(name, gproject);
        }

        private GProject GetProjectObject(string name)
        {
            if (name == null) throw new Exception();
            if (m_projectObjects.ContainsKey(name)) return m_projectObjects[name];

            //string source = m_WebService.GetProjectSource(name);
            string source = m_WebService.Get<String[]>("repository/source/" + name)[0];

            Project project = Project.Deserialize(source);
            GProject gproject = new GProject();
            gproject.Load(project);
            m_projectObjects.Add(name, gproject);
            return gproject;
        }

        private void FreeProjectObject(string name)
        {
            if (name == null) throw new Exception();
            if (m_projectObjects.ContainsKey(name)) m_projectObjects.Remove(name);
        }

        private void RenameProjectObject(string name, string newname)
        {
            if (name == null || newname == null) throw new Exception();
            if (m_projectObjects.ContainsKey(name))
            {
                m_projectObjects.Add(newname, m_projectObjects[name]);
                m_projectObjects.Remove(name);
            }
        }

        private void LoadRepository()
        {
            if (m_repositoryExplorer.Tree.Nodes.Count < 1)
            {
                TreeNode r = m_repositoryExplorer.Tree.Nodes.Add("root", "Repository");
                r.ImageIndex = 0; r.SelectedImageIndex = 0;
            }

            TreeNode root = m_repositoryExplorer.Tree.Nodes["root"];
            root.Tag = new RepositoryExplorer.FolderInfo() { FolderName = "", Loaded = false };

            LoadRepositoryFolder(root);

            root.Expand();
        }

        private void LoadRepositoryFolder(TreeNode parent)
        {
            m_repositoryExplorer.Tree.BeginUpdate();

            RepositoryExplorer.FolderInfo folderInfo = (RepositoryExplorer.FolderInfo)parent.Tag;

            string path = (folderInfo).FolderName;

            parent.Nodes.Clear();

            Dictionary<string, bool> isExpanded = new Dictionary<string, bool>();
            foreach (TreeNode n in parent.Nodes)
            {
                isExpanded.Add(n.Name, n.IsExpanded);
            }

            parent.Nodes.Clear();

            // retrieve server projects
            //string[] projectNames = m_WebService.GetRepositoryProjectNames(path);
            string[] projectNames = m_WebService.Get<String[]>("repository/projects?path=" + path);

            foreach (string projectName in projectNames)
            {
                string pathName = folderInfo.FolderName + projectName;

                Project project = GetProjectObject(pathName).BaseProject;

                TreeNode node = parent.Nodes.Add(projectName, projectName);
                node.Tag = pathName;

                node.ImageIndex = 3;
                node.SelectedImageIndex = 3;

                Dictionary<string, Process> processes = new Dictionary<string, Process>();
                foreach (Process process in project.ProcessList)
                {
                    processes.Add(process.Name, process);
                }
                foreach (string name in processes.Keys)
                {
                    TreeNode treeNodeProcess = node.Nodes.Add(name, name);
                    treeNodeProcess.Tag = processes[name];

                    if (project.StartUpProcess == name)
                    {
                        treeNodeProcess.ImageIndex = 7;
                        treeNodeProcess.SelectedImageIndex = 7;
                    }
                    else
                    {
                        treeNodeProcess.ImageIndex = 5;
                        treeNodeProcess.SelectedImageIndex = 5;
                    }
                }

                if (isExpanded.ContainsKey(node.Name) && isExpanded[node.Name]) node.Expand();
            }

            //string[] folderNames = m_WebService.GetRepositoryFolderNames(path);
            string[] folderNames = m_WebService.Get<String[]>("repository/folders?path=" + path);

            foreach (string pathname in folderNames)
            {
                string folder = pathname;

                TreeNode node = parent.Nodes.Add(folder, folder);
                node.Tag = new RepositoryExplorer.FolderInfo() { FolderName = folderInfo.FolderName + folder + '.', Loaded = false };

                node.ImageIndex = 2;
                node.SelectedImageIndex = 2;

                node.Nodes.Add("");

            }

            m_repositoryExplorer.Tree.Sort();

            folderInfo.Loaded = true;

            m_repositoryExplorer.Tree.EndUpdate();
        }

        private void repositoryFolderExpand(object sender)
        {
            TreeNode node = sender as TreeNode;

            if (node.Tag is RepositoryExplorer.FolderInfo)
            {
                RepositoryExplorer.FolderInfo folderInfo = (RepositoryExplorer.FolderInfo)node.Tag;

                if (!folderInfo.Loaded)
                {
                    LoadRepositoryFolder(node);
                }
            }
        }

        private void RefreshRepository(string projectName)
        {
            string[] path = projectName.Split('.');

            TreeNode parent = m_repositoryExplorer.Tree.Nodes["root"];

            for (int i = 0; i < path.Length - 1; i++)
            {
                if (parent.Nodes.ContainsKey(path[i])) parent = parent.Nodes[path[i]];
                else break;
            }

            if (parent.Nodes.ContainsKey(path[path.Length - 1]))
            {
                TreeNode nproject = parent.Nodes[path[path.Length - 1]];

                foreach (TreeNode nprocess in nproject.Nodes)
                {
                    if (nprocess.Text != (nprocess.Tag as Process).Name)
                    {
                        nprocess.Text = (nprocess.Tag as Process).Name;
                    }
                }
            }
        }

        private void RefreshDocuments()
        {
            foreach (object o in dockPanel.Documents)
            {
                if (o is GraphDoc)
                {
                    GraphDoc gd = o as GraphDoc;
                    gd.Refresh();
                    gd.UpdateText();
                }
            }
        }

        private void tsbFileNew_Click(object sender, EventArgs e)
        {
            Project g = new Project();
            g.NewProcess("main");
            g.StartUpProcess = "main";
            Process cmain = g.FindProcess("main");
            cmain.NewNode("start");
            Node nmain = cmain.FindNode("start");
            cmain.EntryNode = "start";
            cmain.ExitNode = "start";

            GraphDoc gd = new GraphDoc();

            // ask for filename

            string prefix = "project";
            string suffix = "";
            int i = 1;
            //! while (m_WebService.GetProjectID(prefix + i.ToString() + suffix) > 0) i++;
            string FileName = prefix + i.ToString() + suffix;

            g.Name = prefix + i.ToString();

            GProject gproject = new GProject();
            gproject.Load(g);

            gd.FileName = FileName;
            gd.LoadProcess(gproject.GProcess("main"));
            SetProjectObject(gd.FileName, gproject);
            gd.OnDocumentModified += new NotifyEvent(childDocumentModified);
            gd.OnDocumentStructureModified += new NotifyEvent(childDocumentStructureModified);
            gd.OnDocumentSave += new NotifyEvent(childDocumentSave);
            gd.OnDocumentRun += new NotifyEvent(childDocumentRun);
            gd.OnSelectionChanged += new NotifyEvent(childSelectionChanged);

            gd.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            gd.Show(dockPanel);

            gd.SaveProject();
            gd.LoadProcess(GetProjectObject(gd.FileName).GProcess(gd.ProcessName));

            LoadRepository();
        }

        private void childDocumentModified(object sender)
        {
        }

        private void childDocumentStructureModified(object sender)
        {
            RefreshRepository(((GraphDoc)sender).GProject.BaseProject.Name);
            RefreshDocuments();
        }

        private void SaveDocument(GProject gproject, string pathname)
        {
            Project project = gproject.BaseProject;
            string s = Project.Serialize(project);
            try
            {
                int id = 0;//! m_WebService.Load(s);
                if (id == 0)
                {
                    string error = "";//! m_WebService.LoadErrors(s);
                    MessageBox.Show("Error in function: " + error);
                }
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("debug.cs");
                w.Write(ex.Message);
                w.Close();

                MessageBox.Show(ex.Message);
            }
        }

        private void RunProject(string pathname)
        {
            try
            {
                int id = 0;//! m_WebService.GetProjectID(pathname);
                int pid = 0;//! m_WebService.Run(id);
                MessageBox.Show(pid.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void childDocumentSave(object sender)
        {
            GProject g = (sender as GraphDoc).GProject;

            SaveDocument(g, (sender as GraphDoc).FileName);

            foreach (object o in dockPanel.Documents)
            {
                if (o is GraphDoc)
                {
                    GraphDoc gd = o as GraphDoc;
                    gd.Modified = false;
                    gd.UpdateText();
                }
            }
        }

        private void childDocumentRun(object sender)
        {
            GProject g = (sender as GraphDoc).GProject;

            RunProject((sender as GraphDoc).FileName);
        }

        private void childSelectionChanged(object sender)
        {
            if (sender == dockPanel.ActiveDocument && sender is IPropertyGridInterface)
            {
                m_propertyWindow.SelectedObject = (sender as IPropertyGridInterface).SelectedObject;
            }
        }

        private void repositoryDocumentOpen(object sender)
        {
            TreeNode node = sender as TreeNode;

            string pathname = node.Parent.Tag as string;

            GraphDoc doc = null;
            foreach (object o in dockPanel.Documents)
            {
                if (o is GraphDoc)
                {
                    GraphDoc d = o as GraphDoc;
                    if (d.FileName == pathname && d.ProcessName == node.Text) { doc = d; break; }
                }
            }

            if (doc == null)
            {
                doc = OpenDocument(pathname, node.Text);
            }
            else
            {
                doc.Activate();
            }
        }

        private bool repositoryDocumentRename(object sender, string newName)
        {
            TreeNode node = sender as TreeNode;

            if (node.Tag is Process)
            {
                string pathname = node.Parent.Tag as string;

                GProject g = GetProjectObject(pathname);

                if (g.GProcess(newName)!=null) return false;
                g.GProcess(node.Text).Name = newName;
                if (g.GProcess(newName) == null) return false;

                GraphDoc doc = null;
                foreach (object o in dockPanel.Documents)
                {
                    if (o is GraphDoc)
                    {
                        GraphDoc d = o as GraphDoc;
                        if (d.FileName == pathname) { doc = d; doc.Modified = true; }
                    }
                }

                if (doc == null)
                {
                    SaveDocument(g, pathname);
                }
                else
                {
                    RefreshDocuments();
                }
            }
            else if (node.Tag is string)
            {
                return false;
            }
            
            return true;
        }

        private void repositoryDocumentDelete(object sender)
        {
            TreeNode node = sender as TreeNode;

            if (node.Tag is Process)
            {
                string pathname = node.Parent.Tag as string;

                GraphDoc doc = null, todel = null;
                foreach (object o in dockPanel.Documents)
                {
                    if (o is GraphDoc)
                    {
                        GraphDoc d = o as GraphDoc;
                        if (d.FileName == pathname)
                        {
                            if (d.ProcessName != node.Text) { doc = d; doc.Modified = true; }
                            else todel = d;
                        }
                    }
                }

                if (todel != null) { todel.Modified = false; todel.Close(); }

                GProject g = GetProjectObject(pathname);
                g.RemoveProcess(node.Text);

                if (g.BaseProject.StartUpProcess == node.Text)
                {
                    if (g.BaseProject.ProcessList.Count() > 0)
                    {
                        string entry = g.BaseProject.ProcessList[0].Name;
                        g.BaseProject.StartUpProcess = entry;
                        node.Parent.Nodes[entry].ImageIndex = 7;
                        node.Parent.Nodes[entry].SelectedImageIndex = 7;
                    }
                }

                if (doc == null)
                {
                    SaveDocument(g, pathname);
                }
                else
                {
                    RefreshDocuments();
                }

                node.Remove();
            }
            else if (node.Tag is string)
            {
                string pathname = node.Tag as string;

                List<GraphDoc> todel = new List<GraphDoc>();
                foreach (object o in dockPanel.Documents)
                {
                    if (o is GraphDoc)
                    {
                        GraphDoc d = o as GraphDoc;
                        if (d.FileName == pathname)
                        {
                            todel.Add(d);
                        }
                    }
                }
                foreach (GraphDoc d in todel) { d.Modified = false; d.Close(); }

                FreeProjectObject(pathname);

                //! m_WebService.Unload(m_WebService.GetProjectID(pathname));

                node.Remove();
            }
        }

        private void repositoryDocumentMark(object sender)
        {
            TreeNode node = sender as TreeNode;

            Process process = node.Tag as Process;
            Project project = process.ParentProject;
            string old = project.StartUpProcess;
            project.StartUpProcess = process.Name;
            node.ImageIndex = 7;
            node.SelectedImageIndex = 7;

            if (node.Parent.Nodes.ContainsKey(old))
            {
                node.Parent.Nodes[old].ImageIndex = 5;
                node.Parent.Nodes[old].SelectedImageIndex = 5;
            }

            string pathname = node.Parent.Tag as string;
            
            GraphDoc doc = null;
            foreach (object o in dockPanel.Documents)
            {
                if (o is GraphDoc)
                {
                    GraphDoc d = o as GraphDoc;
                    if (d.FileName == pathname)
                    {
                        if (d.ProcessName != node.Text) { doc = d; doc.Modified = true; }
                    }
                }
            }

            GProject g = GetProjectObject(pathname);

            if (doc == null)
            {
                SaveDocument(g, pathname);
            }
            else
            {
                RefreshDocuments();
            }
        }

        private void repositoryDocumentAddFolder(object sender)
        {
            TreeNode node = sender as TreeNode;
        }

        private void repositoryDocumentAddProject(object sender)
        {
            TreeNode node = sender as TreeNode;

            tsbFileNew.PerformClick();
        }

        private void repositoryDocumentAddProcess(object sender)
        {
            TreeNode node = sender as TreeNode;

            GProject g = GetProjectObject(node.Tag as string);

            int c = 1;
            string prefix = "c";
            while (g.GProcess(prefix + c.ToString()) != null) c++;
            g.AddProcess(prefix + c.ToString());
            GProcess cmain = g.GProcess(prefix + c.ToString());
            cmain.AddNodeCode(1, 0, 0);
            cmain.Base.EntryNode = cmain.Base.NodesList[0].Name;

            if (g.BaseProject.ProcessList.Count() == 1)
                g.BaseProject.StartUpProcess = prefix + c.ToString();

            LoadRepository();

            string pathname = node.Tag as string;

            GraphDoc doc = null;
            foreach (object o in dockPanel.Documents)
            {
                if (o is GraphDoc)
                {
                    GraphDoc d = o as GraphDoc;
                    if (d.FileName == pathname)
                    {
                        if (d.ProcessName != node.Text) { doc = d; doc.Modified = true; }
                    }
                }
            }

            if (doc == null)
            {
                SaveDocument(g, pathname);
            }
            else
            {
                RefreshDocuments();
            }
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument != null)
            {
                if (dockPanel.ActiveDocument is IToolstripInterface)
                {
                    ToolStripPanel[] tspanels = new ToolStripPanel[] { tspTop, tspBottom, tspLeft, tspRight };

                    foreach (ToolStripPanel tp in tspanels)
                        foreach (ToolStrip ts in tp.Controls)
                        {
                            ts.AutoSize = false;
                            ToolStripManager.RevertMerge(ts);
                        }

                    foreach (ToolStrip tstrip in (dockPanel.ActiveDocument as IToolstripInterface).ToolStirps)
                    {
                        tstrip.Visible = false;

                        ToolStrip parentStrip = null;
                        foreach (ToolStripPanel tp in tspanels)
                        {
                            parentStrip = tp.Controls[tstrip.Name] as ToolStrip;
                            if (parentStrip != null) break;
                        }

                        if (parentStrip == null)
                        {
                            parentStrip = new ToolStrip();
                            parentStrip.SuspendLayout();

                            parentStrip.Name = tstrip.Name;
                            parentStrip.Dock = System.Windows.Forms.DockStyle.None;
                            int left = 0;
                            foreach (ToolStrip ts in tspTop.Controls)
                            {
                                if (ts.Top == 0 && ts.Right > left) left = ts.Right;
                            }
                            parentStrip.Location = new System.Drawing.Point(left, 0);
                            parentStrip.TabIndex = 0;

                            this.tspTop.Controls.Add(parentStrip);

                            parentStrip.ResumeLayout(false);
                            parentStrip.PerformLayout();
                        }

                        ToolStripManager.Merge(tstrip, parentStrip);

                        parentStrip.Refresh();
                    }

                    foreach (ToolStripPanel tp in tspanels)
                        foreach (ToolStrip ts in tp.Controls)
                        {
                            if (ts.Items.Count <= 0)
                            {
                                //Settings
                                tp.Controls.Remove(ts);
                            }
                            else ts.AutoSize = true;
                        }
                }
                if (dockPanel.ActiveDocument is IPropertyGridInterface)
                {
                    childSelectionChanged(dockPanel.ActiveDocument as IPropertyGridInterface);
                }
            }
        }

        private void tsbServer_Click(object sender, EventArgs e)
        {
            foreach (Form f in  this.MdiChildren)
            {
                if (f as ServerStatusDoc != null)
                {
                    f.Activate();
                    return;
                }
            }

            ServerStatusDoc gd = new ServerStatusDoc();
            gd.Text = "Server";
            gd.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            gd.Show(dockPanel);
        }

        private GraphDoc OpenDocument(string filename, string processName)
        {
            GraphDoc gd = new GraphDoc();
            gd.FileName = filename;
            gd.LoadProcess(GetProjectObject(filename).GProcess(processName));
            gd.OnDocumentModified += new NotifyEvent(childDocumentModified);
            gd.OnDocumentStructureModified += new NotifyEvent(childDocumentStructureModified);
            gd.OnDocumentSave += new NotifyEvent(childDocumentSave);
            gd.OnDocumentRun += new NotifyEvent(childDocumentRun);
            gd.OnSelectionChanged += new NotifyEvent(childSelectionChanged);

            gd.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            gd.Show(dockPanel);
            return gd;
        }

        private void tsbFileOpen_Click(object sender, EventArgs e)
        {
            if (!(openFileDialog.ShowDialog() == DialogResult.OK)) return;
            // add to library
        }

        private void tsmConnect_Click(object sender, EventArgs e)
        {
            // connect with localhost

            //m_WebService = new WebService("http://rdesc.uni-plovdiv.bg/ILNET/ILNET.asmx?wsdl");
            //m_WebService = new WebService("http://localhost/ILNET/ILNET.asmx?wsdl");
            m_WebService = new JsonServiceClient("http://localhost:8080/api/v1/");
            //m_WebService = new ILNETServer();

            LoadRepository();

            tsmFile.Enabled = true;
            tstripFile.Enabled = true;
            tsmConnect.Visible = false;
        }

        private void tsmFileWrapLibrary_Click(object sender, EventArgs e)
        {
            if (openDLLDialog.ShowDialog() != DialogResult.OK) return;

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetFullPath(openDLLDialog.FileName);

            AppDomain domain = AppDomain.CreateDomain("ILNET", null, setup);
            Assembly asm = domain.Load(AssemblyName.GetAssemblyName(openDLLDialog.FileName));

            StringBuilder types = new StringBuilder();
 
            foreach (Type type in asm.GetTypes())
            {
                types.Append(type.ToString());

                types.Append(" (");
                bool first = true;
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    if (!first) types.Append(", ");
                    types.Append(methodInfo.Name);
                    first = false;
                }

                types.AppendLine(")");
            }

            MessageBox.Show(types.ToString());
        }
    }
}
