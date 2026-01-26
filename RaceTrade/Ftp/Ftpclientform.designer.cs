namespace RaceTrader
{
    partial class FtpClientForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewLeft = new System.Windows.Forms.ListView();
            this.columnHeaderLeftName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLeftSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLeftModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripLeft = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fxpToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLeftRefresh = new System.Windows.Forms.Button();
            this.btnLeftUp = new System.Windows.Forms.Button();
            this.txtLeftPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboLeftSite = new System.Windows.Forms.ComboBox();
            this.listViewRight = new System.Windows.Forms.ListView();
            this.columnHeaderRightName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRightSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderRightModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fxpToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRightRefresh = new System.Windows.Forms.Button();
            this.btnRightUp = new System.Windows.Forms.Button();
            this.txtRightPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboRightSite = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tabLogControl = new System.Windows.Forms.TabControl();
            this.tabPageConsole = new System.Windows.Forms.TabPage();
            this.txtConsoleLog = new System.Windows.Forms.RichTextBox();
            this.tabPageTransfer = new System.Windows.Forms.TabPage();
            this.txtTransferLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripLeft.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStripRight.SuspendLayout();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabLogControl.SuspendLayout();
            this.tabPageConsole.SuspendLayout();
            this.tabPageTransfer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewLeft);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewRight);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(1685, 605);
            this.splitContainer1.SplitterDistance = 839;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewLeft
            // 
            this.listViewLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.listViewLeft.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderLeftName,
            this.columnHeaderLeftSize,
            this.columnHeaderLeftModified});
            this.listViewLeft.ContextMenuStrip = this.contextMenuStripLeft;
            this.listViewLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLeft.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewLeft.ForeColor = System.Drawing.Color.White;
            this.listViewLeft.FullRowSelect = true;
            this.listViewLeft.HideSelection = false;
            this.listViewLeft.Location = new System.Drawing.Point(0, 86);
            this.listViewLeft.Margin = new System.Windows.Forms.Padding(4);
            this.listViewLeft.Name = "listViewLeft";
            this.listViewLeft.Size = new System.Drawing.Size(839, 519);
            this.listViewLeft.TabIndex = 1;
            this.listViewLeft.UseCompatibleStateImageBehavior = false;
            this.listViewLeft.View = System.Windows.Forms.View.Details;
            this.listViewLeft.DoubleClick += new System.EventHandler(this.listViewLeft_DoubleClick);
            // 
            // columnHeaderLeftName
            // 
            this.columnHeaderLeftName.Text = "Name";
            this.columnHeaderLeftName.Width = 350;
            // 
            // columnHeaderLeftSize
            // 
            this.columnHeaderLeftSize.Text = "Size";
            this.columnHeaderLeftSize.Width = 120;
            // 
            // columnHeaderLeftModified
            // 
            this.columnHeaderLeftModified.Text = "Modified";
            this.columnHeaderLeftModified.Width = 150;
            // 
            // contextMenuStripLeft
            // 
            this.contextMenuStripLeft.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripLeft.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fxpToRightToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.contextMenuStripLeft.Name = "contextMenuStripLeft";
            this.contextMenuStripLeft.Size = new System.Drawing.Size(177, 76);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // fxpToRightToolStripMenuItem
            // 
            this.fxpToRightToolStripMenuItem.Name = "fxpToRightToolStripMenuItem";
            this.fxpToRightToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.fxpToRightToolStripMenuItem.Text = "FXP to Right →";
            this.fxpToRightToolStripMenuItem.Click += new System.EventHandler(this.fxpToRightToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel1.Controls.Add(this.btnLeftRefresh);
            this.panel1.Controls.Add(this.btnLeftUp);
            this.panel1.Controls.Add(this.txtLeftPath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cboLeftSite);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(839, 86);
            this.panel1.TabIndex = 0;
            // 
            // btnLeftRefresh
            // 
            this.btnLeftRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeftRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnLeftRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftRefresh.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.btnLeftRefresh.ForeColor = System.Drawing.Color.White;
            this.btnLeftRefresh.Location = new System.Drawing.Point(726, 46);
            this.btnLeftRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnLeftRefresh.Name = "btnLeftRefresh";
            this.btnLeftRefresh.Size = new System.Drawing.Size(100, 27);
            this.btnLeftRefresh.TabIndex = 4;
            this.btnLeftRefresh.Text = "Refresh";
            this.btnLeftRefresh.UseVisualStyleBackColor = false;
            this.btnLeftRefresh.Click += new System.EventHandler(this.btnLeftRefresh_Click);
            // 
            // btnLeftUp
            // 
            this.btnLeftUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeftUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnLeftUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeftUp.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.btnLeftUp.ForeColor = System.Drawing.Color.White;
            this.btnLeftUp.Location = new System.Drawing.Point(618, 46);
            this.btnLeftUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnLeftUp.Name = "btnLeftUp";
            this.btnLeftUp.Size = new System.Drawing.Size(100, 27);
            this.btnLeftUp.TabIndex = 3;
            this.btnLeftUp.Text = "Up";
            this.btnLeftUp.UseVisualStyleBackColor = false;
            this.btnLeftUp.Click += new System.EventHandler(this.btnLeftUp_Click);
            // 
            // txtLeftPath
            // 
            this.txtLeftPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLeftPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtLeftPath.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLeftPath.ForeColor = System.Drawing.Color.White;
            this.txtLeftPath.Location = new System.Drawing.Point(13, 48);
            this.txtLeftPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtLeftPath.Name = "txtLeftPath";
            this.txtLeftPath.Size = new System.Drawing.Size(595, 25);
            this.txtLeftPath.TabIndex = 2;
            this.txtLeftPath.Text = "/";
            this.txtLeftPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLeftPath_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Left Site";
            // 
            // cboLeftSite
            // 
            this.cboLeftSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLeftSite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cboLeftSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLeftSite.Font = new System.Drawing.Font("Consolas", 9F);
            this.cboLeftSite.ForeColor = System.Drawing.Color.White;
            this.cboLeftSite.FormattingEnabled = true;
            this.cboLeftSite.Location = new System.Drawing.Point(124, 7);
            this.cboLeftSite.Margin = new System.Windows.Forms.Padding(4);
            this.cboLeftSite.Name = "cboLeftSite";
            this.cboLeftSite.Size = new System.Drawing.Size(700, 26);
            this.cboLeftSite.TabIndex = 0;
            this.cboLeftSite.SelectedIndexChanged += new System.EventHandler(this.cboLeftSite_SelectedIndexChanged);
            // 
            // listViewRight
            // 
            this.listViewRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.listViewRight.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderRightName,
            this.columnHeaderRightSize,
            this.columnHeaderRightModified});
            this.listViewRight.ContextMenuStrip = this.contextMenuStripRight;
            this.listViewRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRight.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewRight.ForeColor = System.Drawing.Color.White;
            this.listViewRight.FullRowSelect = true;
            this.listViewRight.HideSelection = false;
            this.listViewRight.Location = new System.Drawing.Point(0, 86);
            this.listViewRight.Margin = new System.Windows.Forms.Padding(4);
            this.listViewRight.Name = "listViewRight";
            this.listViewRight.Size = new System.Drawing.Size(841, 519);
            this.listViewRight.TabIndex = 1;
            this.listViewRight.UseCompatibleStateImageBehavior = false;
            this.listViewRight.View = System.Windows.Forms.View.Details;
            this.listViewRight.DoubleClick += new System.EventHandler(this.listViewRight_DoubleClick);
            // 
            // columnHeaderRightName
            // 
            this.columnHeaderRightName.Text = "Name";
            this.columnHeaderRightName.Width = 350;
            // 
            // columnHeaderRightSize
            // 
            this.columnHeaderRightSize.Text = "Size";
            this.columnHeaderRightSize.Width = 120;
            // 
            // columnHeaderRightModified
            // 
            this.columnHeaderRightModified.Text = "Modified";
            this.columnHeaderRightModified.Width = 150;
            // 
            // contextMenuStripRight
            // 
            this.contextMenuStripRight.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fxpToLeftToolStripMenuItem,
            this.deleteToolStripMenuItem1,
            this.viewToolStripMenuItem1,
            this.refreshToolStripMenuItem1});
            this.contextMenuStripRight.Name = "contextMenuStripRight";
            this.contextMenuStripRight.Size = new System.Drawing.Size(167, 76);
            // 
            // viewToolStripMenuItem1
            // 
            this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
            this.viewToolStripMenuItem1.Size = new System.Drawing.Size(166, 24);
            this.viewToolStripMenuItem1.Text = "View";
            this.viewToolStripMenuItem1.Click += new System.EventHandler(this.viewToolStripMenuItem1_Click);
            // 
            // fxpToLeftToolStripMenuItem
            // 
            this.fxpToLeftToolStripMenuItem.Name = "fxpToLeftToolStripMenuItem";
            this.fxpToLeftToolStripMenuItem.Size = new System.Drawing.Size(166, 24);
            this.fxpToLeftToolStripMenuItem.Text = "← FXP to Left";
            this.fxpToLeftToolStripMenuItem.Click += new System.EventHandler(this.fxpToLeftToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(166, 24);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // refreshToolStripMenuItem1
            // 
            this.refreshToolStripMenuItem1.Name = "refreshToolStripMenuItem1";
            this.refreshToolStripMenuItem1.Size = new System.Drawing.Size(166, 24);
            this.refreshToolStripMenuItem1.Text = "Refresh";
            this.refreshToolStripMenuItem1.Click += new System.EventHandler(this.refreshToolStripMenuItem1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel2.Controls.Add(this.btnRightRefresh);
            this.panel2.Controls.Add(this.btnRightUp);
            this.panel2.Controls.Add(this.txtRightPath);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cboRightSite);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(841, 86);
            this.panel2.TabIndex = 0;
            // 
            // btnRightRefresh
            // 
            this.btnRightRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRightRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnRightRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRightRefresh.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.btnRightRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRightRefresh.Location = new System.Drawing.Point(728, 46);
            this.btnRightRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRightRefresh.Name = "btnRightRefresh";
            this.btnRightRefresh.Size = new System.Drawing.Size(100, 27);
            this.btnRightRefresh.TabIndex = 4;
            this.btnRightRefresh.Text = "Refresh";
            this.btnRightRefresh.UseVisualStyleBackColor = false;
            this.btnRightRefresh.Click += new System.EventHandler(this.btnRightRefresh_Click);
            // 
            // btnRightUp
            // 
            this.btnRightUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRightUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnRightUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRightUp.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.btnRightUp.ForeColor = System.Drawing.Color.White;
            this.btnRightUp.Location = new System.Drawing.Point(620, 46);
            this.btnRightUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnRightUp.Name = "btnRightUp";
            this.btnRightUp.Size = new System.Drawing.Size(100, 27);
            this.btnRightUp.TabIndex = 3;
            this.btnRightUp.Text = "Up";
            this.btnRightUp.UseVisualStyleBackColor = false;
            this.btnRightUp.Click += new System.EventHandler(this.btnRightUp_Click);
            // 
            // txtRightPath
            // 
            this.txtRightPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRightPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtRightPath.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtRightPath.ForeColor = System.Drawing.Color.White;
            this.txtRightPath.Location = new System.Drawing.Point(13, 48);
            this.txtRightPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtRightPath.Name = "txtRightPath";
            this.txtRightPath.Size = new System.Drawing.Size(597, 25);
            this.txtRightPath.TabIndex = 2;
            this.txtRightPath.Text = "/";
            this.txtRightPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRightPath_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right Site";
            // 
            // cboRightSite
            // 
            this.cboRightSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRightSite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cboRightSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRightSite.Font = new System.Drawing.Font("Consolas", 9F);
            this.cboRightSite.ForeColor = System.Drawing.Color.White;
            this.cboRightSite.FormattingEnabled = true;
            this.cboRightSite.Location = new System.Drawing.Point(135, 7);
            this.cboRightSite.Margin = new System.Windows.Forms.Padding(4);
            this.cboRightSite.Name = "cboRightSite";
            this.cboRightSite.Size = new System.Drawing.Size(692, 26);
            this.cboRightSite.TabIndex = 0;
            this.cboRightSite.SelectedIndexChanged += new System.EventHandler(this.cboRightSite_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.statusStrip1.Font = new System.Drawing.Font("Consolas", 9F);
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 851);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1685, 24);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Font = new System.Drawing.Font("Consolas", 9F);
            this.toolStripStatusLabel.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(1665, 18);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.Text = "Ready";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(267, 18);
            this.toolStripProgressBar.Visible = false;
            // 
            // tabLogControl
            // 
            this.tabLogControl.Controls.Add(this.tabPageConsole);
            this.tabLogControl.Controls.Add(this.tabPageTransfer);
            this.tabLogControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabLogControl.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabLogControl.ItemSize = new System.Drawing.Size(100, 25);
            this.tabLogControl.Location = new System.Drawing.Point(0, 605);
            this.tabLogControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabLogControl.Name = "tabLogControl";
            this.tabLogControl.SelectedIndex = 0;
            this.tabLogControl.Size = new System.Drawing.Size(1685, 246);
            this.tabLogControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabLogControl.TabIndex = 2;
            // 
            // tabPageConsole
            // 
            this.tabPageConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabPageConsole.Controls.Add(this.txtConsoleLog);
            this.tabPageConsole.Location = new System.Drawing.Point(4, 29);
            this.tabPageConsole.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageConsole.Name = "tabPageConsole";
            this.tabPageConsole.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageConsole.Size = new System.Drawing.Size(1677, 213);
            this.tabPageConsole.TabIndex = 0;
            this.tabPageConsole.Text = "Console";
            // 
            // txtConsoleLog
            // 
            this.txtConsoleLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtConsoleLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsoleLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsoleLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsoleLog.ForeColor = System.Drawing.Color.White;
            this.txtConsoleLog.Location = new System.Drawing.Point(4, 4);
            this.txtConsoleLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtConsoleLog.Name = "txtConsoleLog";
            this.txtConsoleLog.ReadOnly = true;
            this.txtConsoleLog.Size = new System.Drawing.Size(1669, 205);
            this.txtConsoleLog.TabIndex = 0;
            this.txtConsoleLog.Text = "";
            // 
            // tabPageTransfer
            // 
            this.tabPageTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabPageTransfer.Controls.Add(this.txtTransferLog);
            this.tabPageTransfer.Location = new System.Drawing.Point(4, 29);
            this.tabPageTransfer.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageTransfer.Name = "tabPageTransfer";
            this.tabPageTransfer.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageTransfer.Size = new System.Drawing.Size(1677, 213);
            this.tabPageTransfer.TabIndex = 1;
            this.tabPageTransfer.Text = "Transfer Log";
            // 
            // txtTransferLog
            // 
            this.txtTransferLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtTransferLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTransferLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTransferLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTransferLog.ForeColor = System.Drawing.Color.White;
            this.txtTransferLog.Location = new System.Drawing.Point(4, 4);
            this.txtTransferLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtTransferLog.Name = "txtTransferLog";
            this.txtTransferLog.ReadOnly = true;
            this.txtTransferLog.Size = new System.Drawing.Size(1669, 205);
            this.txtTransferLog.TabIndex = 0;
            this.txtTransferLog.Text = "";
            // 
            // FtpClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1685, 875);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tabLogControl);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1061, 728);
            this.Name = "FtpClientForm";
            this.ShowIcon = false;
            this.Text = "CBFTP Client - FXP Browser";
            this.Load += new System.EventHandler(this.FtpClientForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripLeft.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStripRight.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabLogControl.ResumeLayout(false);
            this.tabPageConsole.ResumeLayout(false);
            this.tabPageTransfer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewLeft;
        private System.Windows.Forms.ColumnHeader columnHeaderLeftName;
        private System.Windows.Forms.ColumnHeader columnHeaderLeftSize;
        private System.Windows.Forms.ColumnHeader columnHeaderLeftModified;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLeftRefresh;
        private System.Windows.Forms.Button btnLeftUp;
        private System.Windows.Forms.TextBox txtLeftPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboLeftSite;
        private System.Windows.Forms.ListView listViewRight;
        private System.Windows.Forms.ColumnHeader columnHeaderRightName;
        private System.Windows.Forms.ColumnHeader columnHeaderRightSize;
        private System.Windows.Forms.ColumnHeader columnHeaderRightModified;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRightRefresh;
        private System.Windows.Forms.Button btnRightUp;
        private System.Windows.Forms.TextBox txtRightPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboRightSite;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLeft;
        private System.Windows.Forms.ToolStripMenuItem fxpToRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRight;
        private System.Windows.Forms.ToolStripMenuItem fxpToLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem1;
        private System.Windows.Forms.TabControl tabLogControl;
        private System.Windows.Forms.TabPage tabPageConsole;
        private System.Windows.Forms.RichTextBox txtConsoleLog;
        private System.Windows.Forms.TabPage tabPageTransfer;
        private System.Windows.Forms.RichTextBox txtTransferLog;
    }
}