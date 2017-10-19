namespace ILNET.Client.Windows
{
    partial class GraphDoc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphDoc));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.contextMenuTabPage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tstripChildTools = new System.Windows.Forms.ToolStrip();
            this.tsbToolSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbToolConnect = new System.Windows.Forms.ToolStripButton();
            this.tsbToolVariable = new System.Windows.Forms.ToolStripButton();
            this.tsbToolFunction = new System.Windows.Forms.ToolStripButton();
            this.tsbToolSubprocess = new System.Windows.Forms.ToolStripButton();
            this.tsbToolS1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToolCut = new System.Windows.Forms.ToolStripButton();
            this.tsbToolCopy = new System.Windows.Forms.ToolStripButton();
            this.tsbToolPaste = new System.Windows.Forms.ToolStripButton();
            this.tsbToolDelete = new System.Windows.Forms.ToolStripButton();
            this.tsbToolS2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToolAnchorAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbToolAnchorDelete = new System.Windows.Forms.ToolStripButton();
            this.tsbToolAnchorCollapse = new System.Windows.Forms.ToolStripButton();
            this.tsbToolS3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToolFunctionEditor = new System.Windows.Forms.ToolStripButton();
            this.btnFocus = new System.Windows.Forms.Button();
            this.tstripFile = new System.Windows.Forms.ToolStrip();
            this.tsbFileSave = new System.Windows.Forms.ToolStripButton();
            this.pbDisplay = new System.Windows.Forms.PictureBox();
            this.tstripProject = new System.Windows.Forms.ToolStrip();
            this.tsbProjectRun = new System.Windows.Forms.ToolStripButton();
            this.editorAction = new ScintillaNET.Scintilla();
            this.scView = new System.Windows.Forms.SplitContainer();
            this.tstripChildTools.SuspendLayout();
            this.tstripFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDisplay)).BeginInit();
            this.tstripProject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scView)).BeginInit();
            this.scView.Panel1.SuspendLayout();
            this.scView.Panel2.SuspendLayout();
            this.scView.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(724, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Visible = false;
            // 
            // contextMenuTabPage
            // 
            this.contextMenuTabPage.Name = "contextMenuTabPage";
            this.contextMenuTabPage.Size = new System.Drawing.Size(61, 4);
            // 
            // tstripChildTools
            // 
            this.tstripChildTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbToolSelect,
            this.tsbToolConnect,
            this.tsbToolVariable,
            this.tsbToolFunction,
            this.tsbToolSubprocess,
            this.tsbToolS1,
            this.tsbToolCut,
            this.tsbToolCopy,
            this.tsbToolPaste,
            this.tsbToolDelete,
            this.tsbToolS2,
            this.tsbToolAnchorAdd,
            this.tsbToolAnchorDelete,
            this.tsbToolAnchorCollapse,
            this.tsbToolS3,
            this.tsbToolFunctionEditor});
            this.tstripChildTools.Location = new System.Drawing.Point(0, 0);
            this.tstripChildTools.Name = "tstripChildTools";
            this.tstripChildTools.Size = new System.Drawing.Size(724, 25);
            this.tstripChildTools.TabIndex = 3;
            this.tstripChildTools.Text = "toolStrip1";
            // 
            // tsbToolSelect
            // 
            this.tsbToolSelect.Checked = true;
            this.tsbToolSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbToolSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolSelect.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolSelect.Image")));
            this.tsbToolSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolSelect.Name = "tsbToolSelect";
            this.tsbToolSelect.Size = new System.Drawing.Size(23, 22);
            this.tsbToolSelect.Text = "Select";
            this.tsbToolSelect.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolConnect
            // 
            this.tsbToolConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolConnect.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolConnect.Image")));
            this.tsbToolConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolConnect.Name = "tsbToolConnect";
            this.tsbToolConnect.Size = new System.Drawing.Size(23, 22);
            this.tsbToolConnect.Text = "Connect";
            this.tsbToolConnect.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolVariable
            // 
            this.tsbToolVariable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolVariable.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolVariable.Image")));
            this.tsbToolVariable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolVariable.Name = "tsbToolVariable";
            this.tsbToolVariable.Size = new System.Drawing.Size(23, 22);
            this.tsbToolVariable.Text = "Variable";
            this.tsbToolVariable.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolFunction
            // 
            this.tsbToolFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolFunction.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolFunction.Image")));
            this.tsbToolFunction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolFunction.Name = "tsbToolFunction";
            this.tsbToolFunction.Size = new System.Drawing.Size(23, 22);
            this.tsbToolFunction.Text = "Function";
            this.tsbToolFunction.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolSubprocess
            // 
            this.tsbToolSubprocess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolSubprocess.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolSubprocess.Image")));
            this.tsbToolSubprocess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolSubprocess.Name = "tsbToolSubprocess";
            this.tsbToolSubprocess.Size = new System.Drawing.Size(23, 22);
            this.tsbToolSubprocess.Text = "Subprocess";
            this.tsbToolSubprocess.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolS1
            // 
            this.tsbToolS1.Name = "tsbToolS1";
            this.tsbToolS1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbToolCut
            // 
            this.tsbToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolCut.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolCut.Image")));
            this.tsbToolCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolCut.Name = "tsbToolCut";
            this.tsbToolCut.Size = new System.Drawing.Size(23, 22);
            this.tsbToolCut.Text = "Cut";
            // 
            // tsbToolCopy
            // 
            this.tsbToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolCopy.Image")));
            this.tsbToolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolCopy.Name = "tsbToolCopy";
            this.tsbToolCopy.Size = new System.Drawing.Size(23, 22);
            this.tsbToolCopy.Text = "Copy";
            // 
            // tsbToolPaste
            // 
            this.tsbToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolPaste.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolPaste.Image")));
            this.tsbToolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolPaste.Name = "tsbToolPaste";
            this.tsbToolPaste.Size = new System.Drawing.Size(23, 22);
            this.tsbToolPaste.Text = "Paste";
            // 
            // tsbToolDelete
            // 
            this.tsbToolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolDelete.Image")));
            this.tsbToolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolDelete.Name = "tsbToolDelete";
            this.tsbToolDelete.Size = new System.Drawing.Size(23, 22);
            this.tsbToolDelete.Text = "Delete";
            this.tsbToolDelete.Click += new System.EventHandler(this.tsbToolDelete_Click);
            // 
            // tsbToolS2
            // 
            this.tsbToolS2.Name = "tsbToolS2";
            this.tsbToolS2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbToolAnchorAdd
            // 
            this.tsbToolAnchorAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolAnchorAdd.Image")));
            this.tsbToolAnchorAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolAnchorAdd.Name = "tsbToolAnchorAdd";
            this.tsbToolAnchorAdd.Size = new System.Drawing.Size(91, 22);
            this.tsbToolAnchorAdd.Text = "Add Anchor";
            this.tsbToolAnchorAdd.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolAnchorDelete
            // 
            this.tsbToolAnchorDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolAnchorDelete.Image")));
            this.tsbToolAnchorDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolAnchorDelete.Name = "tsbToolAnchorDelete";
            this.tsbToolAnchorDelete.Size = new System.Drawing.Size(102, 22);
            this.tsbToolAnchorDelete.Text = "Delete Anchor";
            this.tsbToolAnchorDelete.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolAnchorCollapse
            // 
            this.tsbToolAnchorCollapse.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolAnchorCollapse.Image")));
            this.tsbToolAnchorCollapse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolAnchorCollapse.Name = "tsbToolAnchorCollapse";
            this.tsbToolAnchorCollapse.Size = new System.Drawing.Size(106, 22);
            this.tsbToolAnchorCollapse.Text = "Toggle Anchor";
            this.tsbToolAnchorCollapse.Click += new System.EventHandler(this.tsbToolSelect_Click);
            // 
            // tsbToolS3
            // 
            this.tsbToolS3.Name = "tsbToolS3";
            this.tsbToolS3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbToolFunctionEditor
            // 
            this.tsbToolFunctionEditor.CheckOnClick = true;
            this.tsbToolFunctionEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolFunctionEditor.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolFunctionEditor.Image")));
            this.tsbToolFunctionEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolFunctionEditor.Name = "tsbToolFunctionEditor";
            this.tsbToolFunctionEditor.Size = new System.Drawing.Size(23, 22);
            this.tsbToolFunctionEditor.Text = "Function Editor";
            this.tsbToolFunctionEditor.Click += new System.EventHandler(this.tsbToolFunctionEditor_Click);
            // 
            // btnFocus
            // 
            this.btnFocus.Location = new System.Drawing.Point(12, 90);
            this.btnFocus.Name = "btnFocus";
            this.btnFocus.Size = new System.Drawing.Size(75, 23);
            this.btnFocus.TabIndex = 4;
            this.btnFocus.Text = "button1";
            this.btnFocus.UseVisualStyleBackColor = true;
            this.btnFocus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnFocus_KeyDown);
            // 
            // tstripFile
            // 
            this.tstripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFileSave});
            this.tstripFile.Location = new System.Drawing.Point(0, 25);
            this.tstripFile.Name = "tstripFile";
            this.tstripFile.Size = new System.Drawing.Size(724, 25);
            this.tstripFile.TabIndex = 5;
            this.tstripFile.Text = "toolStrip1";
            // 
            // tsbFileSave
            // 
            this.tsbFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFileSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbFileSave.Image")));
            this.tsbFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFileSave.Name = "tsbFileSave";
            this.tsbFileSave.Size = new System.Drawing.Size(23, 22);
            this.tsbFileSave.Text = "Save";
            this.tsbFileSave.Click += new System.EventHandler(this.tsbFileSave_Click);
            // 
            // pbDisplay
            // 
            this.pbDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbDisplay.Location = new System.Drawing.Point(0, 0);
            this.pbDisplay.Name = "pbDisplay";
            this.pbDisplay.Size = new System.Drawing.Size(724, 575);
            this.pbDisplay.TabIndex = 2;
            this.pbDisplay.TabStop = false;
            this.pbDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.pbDisplay_Paint);
            this.pbDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbDisplay_MouseDown);
            this.pbDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbDisplay_MouseMove);
            this.pbDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbDisplay_MouseUp);
            this.pbDisplay.Resize += new System.EventHandler(this.pbDisplay_Resize);
            // 
            // tstripProject
            // 
            this.tstripProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbProjectRun});
            this.tstripProject.Location = new System.Drawing.Point(0, 50);
            this.tstripProject.Name = "tstripProject";
            this.tstripProject.Size = new System.Drawing.Size(724, 25);
            this.tstripProject.TabIndex = 6;
            // 
            // tsbProjectRun
            // 
            this.tsbProjectRun.Image = ((System.Drawing.Image)(resources.GetObject("tsbProjectRun.Image")));
            this.tsbProjectRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbProjectRun.Name = "tsbProjectRun";
            this.tsbProjectRun.Size = new System.Drawing.Size(48, 22);
            this.tsbProjectRun.Text = "Run";
            this.tsbProjectRun.Click += new System.EventHandler(this.tsbProcessRun_Click);
            // 
            // editorAction
            // 
            this.editorAction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editorAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorAction.Enabled = false;
            this.editorAction.Location = new System.Drawing.Point(0, 0);
            this.editorAction.Name = "editorAction";
            this.editorAction.Size = new System.Drawing.Size(150, 46);
            this.editorAction.TabIndex = 7;
            this.editorAction.TextChanged += new System.EventHandler(this.editorAction_TextChanged);
            // 
            // scView
            // 
            this.scView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scView.Location = new System.Drawing.Point(0, 75);
            this.scView.Name = "scView";
            this.scView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scView.Panel1
            // 
            this.scView.Panel1.Controls.Add(this.pbDisplay);
            this.scView.Panel1MinSize = 100;
            // 
            // scView.Panel2
            // 
            this.scView.Panel2.Controls.Add(this.editorAction);
            this.scView.Panel2Collapsed = true;
            this.scView.Size = new System.Drawing.Size(724, 575);
            this.scView.SplitterDistance = 330;
            this.scView.TabIndex = 8;
            // 
            // GraphDoc
            // 
            this.ClientSize = new System.Drawing.Size(724, 650);
            this.Controls.Add(this.scView);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.tstripProject);
            this.Controls.Add(this.tstripFile);
            this.Controls.Add(this.tstripChildTools);
            this.Controls.Add(this.btnFocus);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "GraphDoc";
            this.TabPageContextMenuStrip = this.contextMenuTabPage;
            this.Shown += new System.EventHandler(this.GraphDoc_Shown);
            this.tstripChildTools.ResumeLayout(false);
            this.tstripChildTools.PerformLayout();
            this.tstripFile.ResumeLayout(false);
            this.tstripFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDisplay)).EndInit();
            this.tstripProject.ResumeLayout(false);
            this.tstripProject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorAction)).EndInit();
            this.scView.Panel1.ResumeLayout(false);
            this.scView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scView)).EndInit();
            this.scView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuTabPage;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox pbDisplay;
        private System.Windows.Forms.ToolStrip tstripChildTools;
        private System.Windows.Forms.ToolStripButton tsbToolSelect;
        private System.Windows.Forms.ToolStripButton tsbToolConnect;
        private System.Windows.Forms.ToolStripSeparator tsbToolS1;
        private System.Windows.Forms.ToolStripButton tsbToolAnchorAdd;
        private System.Windows.Forms.ToolStripButton tsbToolAnchorDelete;
        private System.Windows.Forms.ToolStripButton tsbToolAnchorCollapse;
        private System.Windows.Forms.ToolStripButton tsbToolDelete;
        private System.Windows.Forms.ToolStripSeparator tsbToolS2;
        private System.Windows.Forms.ToolStripButton tsbToolCut;
        private System.Windows.Forms.ToolStripButton tsbToolCopy;
        private System.Windows.Forms.ToolStripButton tsbToolPaste;
        private System.Windows.Forms.Button btnFocus;
        private System.Windows.Forms.ToolStrip tstripFile;
        private System.Windows.Forms.ToolStripButton tsbFileSave;
        private System.Windows.Forms.ToolStripButton tsbToolVariable;
        private System.Windows.Forms.ToolStripButton tsbToolFunction;
        private System.Windows.Forms.ToolStripButton tsbToolSubprocess;
        private System.Windows.Forms.ToolStrip tstripProject;
        private System.Windows.Forms.ToolStripButton tsbProjectRun;
        private ScintillaNET.Scintilla editorAction;
        private System.Windows.Forms.SplitContainer scView;
        private System.Windows.Forms.ToolStripSeparator tsbToolS3;
        private System.Windows.Forms.ToolStripButton tsbToolFunctionEditor;
    }
}