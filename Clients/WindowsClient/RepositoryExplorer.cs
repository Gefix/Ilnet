using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ILNET.Elements;

namespace ILNET.Client.Windows
{
    public partial class RepositoryExplorer : ToolWindow
    {
        public class FolderInfo
        {
            public string FolderName;
            public bool Loaded;
        }
        
        public RepositoryExplorer()
        {
            InitializeComponent();
        }

        public event NotifyEvent OnDocumentOpen;
        public event RenameEvent OnDocumentRename;
        public event NotifyEvent OnDocumentDelete;
        public event NotifyEvent OnDocumentAddFolder;
        public event NotifyEvent OnDocumentAddProject;
        public event NotifyEvent OnDocumentAddProcess;
        public event NotifyEvent OnDocumentMark;
        public event NotifyEvent OnFolderExpand;

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            treeView.RightToLeftLayout = RightToLeftLayout;
        }

        public TreeView Tree
        {
            get { return this.treeView; }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is Process)
            {
                OnDocumentOpen.Invoke(e.Node);
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null || e.Label == "") { e.CancelEdit = true; return; }

            if (!OnDocumentRename.Invoke(e.Node, e.Label)) e.CancelEdit = true;
        }

        private void tsmTreeViewRename_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
                treeView.SelectedNode.BeginEdit();
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (treeView.GetNodeAt(e.Location) != null) treeView.SelectedNode = treeView.GetNodeAt(e.Location);
            else treeView.SelectedNode = null;
        }

        private void tsmTreeViewDelete_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
                OnDocumentDelete.Invoke(treeView.SelectedNode);
        }

        private void cmsTreeView_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;

            if (node == null)
            {
                e.Cancel = true;
                return;
            }

            if (!(node.Tag is Process))
            {
                tsmTreeViewEntrySeparator.Visible = false;
                tsmTreeViewEntry.Visible = false;
            }
            else
            {
                tsmTreeViewEntrySeparator.Visible = true;
                tsmTreeViewEntry.Visible = true;

                Process process = treeView.SelectedNode.Tag as Process;
                Project project = process.ParentProject;

                if (project.StartUpProcess == process.Name)
                {
                    tsmTreeViewEntry.Enabled = false;
                    tsmTreeViewEntry.Checked = true;
                }
                else
                {
                    tsmTreeViewEntry.Enabled = true;
                    tsmTreeViewEntry.Checked = false;
                }
            }

            if (node.Tag is Process) tsmTreeViewAdd.Visible = false;
            else tsmTreeViewAdd.Visible = true;

            if (node.Tag is string)
            {
                tsmTreeViewAddFolder.Visible = true;
                tsmTreeViewFolderSeparator.Visible = true;
                tsmTreeViewAddProject.Visible = false;
                tsmTreeViewAddProcess.Visible = true;
            }

            if (node.Tag == null)
            {
                tsmTreeViewAddFolder.Visible = false;
                tsmTreeViewFolderSeparator.Visible = false;
                tsmTreeViewAddProject.Visible = true;
                tsmTreeViewAddProcess.Visible = false;
            }
        }

        private void tsmTreeViewEntry_Click(object sender, EventArgs e)
        {
            OnDocumentMark.Invoke(treeView.SelectedNode);
        }

        private void tsmTreeViewAddProcess_Click(object sender, EventArgs e)
        {
            OnDocumentAddProcess.Invoke(treeView.SelectedNode);
        }

        private void tsmTreeViewAddProject_Click(object sender, EventArgs e)
        {
            OnDocumentAddProject.Invoke(treeView.SelectedNode);
        }

        private void tsmTreeViewAddFolder_Click(object sender, EventArgs e)
        {
            OnDocumentAddFolder.Invoke(treeView.SelectedNode);
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            OnFolderExpand.Invoke(e.Node);
            if (e.Node.Tag is FolderInfo && e.Node.Nodes.Count > 0)
            {
                e.Node.Expand();
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is FolderInfo)
            {
                e.Node.ImageIndex = 2;
                e.Node.SelectedImageIndex = 2;
                ((FolderInfo)e.Node.Tag).Loaded = false;
            }
        }
    }
}