namespace ILNET.Client.Windows
{
    partial class RepositoryExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryExplorer));
            this.treeView = new System.Windows.Forms.TreeView();
            this.cmsTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmTreeViewAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewFolderSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsmTreeViewAddProject = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewAddProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTreeViewEntrySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsmTreeViewEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmsTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.ContextMenuStrip = this.cmsTreeView;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Indent = 19;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(256, 399);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCollapse);
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            // 
            // cmsTreeView
            // 
            this.cmsTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmTreeViewAdd,
            this.tsmTreeViewDelete,
            this.tsmTreeViewRename,
            this.tsmTreeViewEntrySeparator,
            this.tsmTreeViewEntry});
            this.cmsTreeView.Name = "cmsTreeView";
            this.cmsTreeView.Size = new System.Drawing.Size(118, 98);
            this.cmsTreeView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsTreeView_Opening);
            // 
            // tsmTreeViewAdd
            // 
            this.tsmTreeViewAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmTreeViewAddFolder,
            this.tsmTreeViewFolderSeparator,
            this.tsmTreeViewAddProject,
            this.tsmTreeViewAddProcess});
            this.tsmTreeViewAdd.Name = "tsmTreeViewAdd";
            this.tsmTreeViewAdd.Size = new System.Drawing.Size(117, 22);
            this.tsmTreeViewAdd.Text = "Add";
            // 
            // tsmTreeViewAddFolder
            // 
            this.tsmTreeViewAddFolder.Name = "tsmTreeViewAddFolder";
            this.tsmTreeViewAddFolder.Size = new System.Drawing.Size(141, 22);
            this.tsmTreeViewAddFolder.Text = "New Folder";
            this.tsmTreeViewAddFolder.Click += new System.EventHandler(this.tsmTreeViewAddFolder_Click);
            // 
            // tsmTreeViewFolderSeparator
            // 
            this.tsmTreeViewFolderSeparator.Name = "tsmTreeViewFolderSeparator";
            this.tsmTreeViewFolderSeparator.Size = new System.Drawing.Size(138, 6);
            // 
            // tsmTreeViewAddProject
            // 
            this.tsmTreeViewAddProject.Name = "tsmTreeViewAddProject";
            this.tsmTreeViewAddProject.Size = new System.Drawing.Size(141, 22);
            this.tsmTreeViewAddProject.Text = "New Project";
            this.tsmTreeViewAddProject.Click += new System.EventHandler(this.tsmTreeViewAddProject_Click);
            // 
            // tsmTreeViewAddProcess
            // 
            this.tsmTreeViewAddProcess.Name = "tsmTreeViewAddProcess";
            this.tsmTreeViewAddProcess.Size = new System.Drawing.Size(141, 22);
            this.tsmTreeViewAddProcess.Text = "New Process";
            this.tsmTreeViewAddProcess.Click += new System.EventHandler(this.tsmTreeViewAddProcess_Click);
            // 
            // tsmTreeViewDelete
            // 
            this.tsmTreeViewDelete.Name = "tsmTreeViewDelete";
            this.tsmTreeViewDelete.Size = new System.Drawing.Size(117, 22);
            this.tsmTreeViewDelete.Text = "Delete";
            this.tsmTreeViewDelete.Click += new System.EventHandler(this.tsmTreeViewDelete_Click);
            // 
            // tsmTreeViewRename
            // 
            this.tsmTreeViewRename.Name = "tsmTreeViewRename";
            this.tsmTreeViewRename.Size = new System.Drawing.Size(117, 22);
            this.tsmTreeViewRename.Text = "Rename";
            this.tsmTreeViewRename.Click += new System.EventHandler(this.tsmTreeViewRename_Click);
            // 
            // tsmTreeViewEntrySeparator
            // 
            this.tsmTreeViewEntrySeparator.Name = "tsmTreeViewEntrySeparator";
            this.tsmTreeViewEntrySeparator.Size = new System.Drawing.Size(114, 6);
            // 
            // tsmTreeViewEntry
            // 
            this.tsmTreeViewEntry.Name = "tsmTreeViewEntry";
            this.tsmTreeViewEntry.Size = new System.Drawing.Size(117, 22);
            this.tsmTreeViewEntry.Text = "Entry";
            this.tsmTreeViewEntry.Click += new System.EventHandler(this.tsmTreeViewEntry_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            // 
            // RepositoryExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(256, 399);
            this.Controls.Add(this.treeView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RepositoryExplorer";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.TabText = "Repository Explorer";
            this.Text = "Repository Explorer - WinFormsUI";
            this.cmsTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip cmsTreeView;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewRename;
        private System.Windows.Forms.ToolStripSeparator tsmTreeViewEntrySeparator;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewEntry;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewAddFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewAddProject;
        private System.Windows.Forms.ToolStripMenuItem tsmTreeViewAddProcess;
        private System.Windows.Forms.ToolStripSeparator tsmTreeViewFolderSeparator;
    }
}