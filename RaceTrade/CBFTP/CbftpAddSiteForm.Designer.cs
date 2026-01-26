using System.Windows.Forms;
using System.Drawing;

namespace RaceTrade
{
    partial class CbftpAddSiteForm
    {
        private System.ComponentModel.IContainer components = null;

        // top
        private Label lblServer;
        private ComboBox cbftpServerCombo;
        private Label lblServerStatus;

        // Basic
        private GroupBox grpBasic;
        private Label lblName;
        private TextBox txtName;
        private CheckBox chkDisabled;
        private Label lblAddresses;
        private TextBox txtAddresses;
        private Label lblUser;
        private TextBox txtUser;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblBasePath;
        private TextBox txtBasePath;
        private Label lblPriority;
        private ComboBox cmbPriority;
        private Label lblListFrequency;
        private ComboBox cmbListFrequency;

        // Limits
        private GroupBox grpLimits;
        private Label lblMaxLogins;
        private NumericUpDown nudMaxLogins;
        private Label lblMaxSimUp;
        private NumericUpDown nudMaxSimUp;
        private Label lblMaxSimDown;
        private NumericUpDown nudMaxSimDown;

        // Allow
        private GroupBox grpAllow;
        private Label lblAllowDownload;
        private ComboBox cmbAllowDownload;
        private Label lblAllowUpload;
        private ComboBox cmbAllowUpload;
        private CheckBox chkPret;
        private CheckBox chkXdupe;

        // Proxy
        private GroupBox grpProxy;
        private Label lblProxyType;
        private ComboBox cmbProxyType;
        private Label lblProxyName;
        private TextBox txtProxyName;

        // TLS / Transfer
        private GroupBox grpTransfer;
        private Label lblTlsMode;
        private ComboBox cmbTlsMode;
        private Label lblTransferProto;
        private ComboBox cmbTransferProtocol;
        private Label lblTlsTransfer;
        private ComboBox cmbTlsTransferPolicy;
        private Label lblSrcPolicy;
        private ComboBox cmbTransferSourcePolicy;
        private Label lblDstPolicy;
        private ComboBox cmbTransferTargetPolicy;
        private Label lblListCmd;
        private ComboBox cmbListCommand;
        private Label lblMaxIdle;
        private NumericUpDown nudMaxIdleTime;
        private CheckBox chkStayLoggedIn;
        private CheckBox chkCepr;
        private CheckBox chkSscn;
        private CheckBox chkCpsv;
        private CheckBox chkBrokenPasv;
        private CheckBox chkForceBinaryMode;
        private CheckBox chkLeaveFreeSlot;

        // Affils
        private GroupBox grpAffils;
        private ListBox lstAffils;
        private TextBox txtAffil;
        private Button btnAddAffil;
        private Button btnRemoveAffil;

        // Sections
        private GroupBox grpSections;
        private ListView lvSections;
        private ColumnHeader colSectionName;
        private ColumnHeader colSectionPath;
        private Label lblSectionName;
        private TextBox txtSectionName;
        private Label lblSectionPath;
        private TextBox txtSectionPath;
        private Button btnAddSection;
        private Button btnRemoveSection;

        // Skiplist
        private GroupBox grpSkiplist;
        private ListView lvSkiplist;
        private ColumnHeader colSkipAction;
        private ColumnHeader colSkipScope;
        private ColumnHeader colSkipPattern;
        private Label lblSkipAction;
        private ComboBox cmbSkipAction;
        private Label lblSkipScope;
        private ComboBox cmbSkipScope;
        private CheckBox chkSkipDir;
        private CheckBox chkSkipFile;
        private CheckBox chkSkipRegex;
        private Label lblSkipPattern;
        private TextBox txtSkipPattern;
        private Button btnAddSkip;
        private Button btnRemoveSkip;

        // bottom buttons
        private Button btnSave;
        private Button btnCancel;

        /// <summary>
        /// Clean up
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblServer = new System.Windows.Forms.Label();
            this.cbftpServerCombo = new System.Windows.Forms.ComboBox();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.grpBasic = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkDisabled = new System.Windows.Forms.CheckBox();
            this.lblAddresses = new System.Windows.Forms.Label();
            this.txtAddresses = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblBasePath = new System.Windows.Forms.Label();
            this.txtBasePath = new System.Windows.Forms.TextBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.cmbPriority = new System.Windows.Forms.ComboBox();
            this.lblListFrequency = new System.Windows.Forms.Label();
            this.cmbListFrequency = new System.Windows.Forms.ComboBox();
            this.grpLimits = new System.Windows.Forms.GroupBox();
            this.lblMaxLogins = new System.Windows.Forms.Label();
            this.nudMaxLogins = new System.Windows.Forms.NumericUpDown();
            this.lblMaxSimUp = new System.Windows.Forms.Label();
            this.nudMaxSimUp = new System.Windows.Forms.NumericUpDown();
            this.lblMaxSimDown = new System.Windows.Forms.Label();
            this.nudMaxSimDown = new System.Windows.Forms.NumericUpDown();
            this.grpAllow = new System.Windows.Forms.GroupBox();
            this.lblAllowDownload = new System.Windows.Forms.Label();
            this.cmbAllowDownload = new System.Windows.Forms.ComboBox();
            this.lblAllowUpload = new System.Windows.Forms.Label();
            this.cmbAllowUpload = new System.Windows.Forms.ComboBox();
            this.chkPret = new System.Windows.Forms.CheckBox();
            this.chkXdupe = new System.Windows.Forms.CheckBox();
            this.grpProxy = new System.Windows.Forms.GroupBox();
            this.lblProxyType = new System.Windows.Forms.Label();
            this.cmbProxyType = new System.Windows.Forms.ComboBox();
            this.lblProxyName = new System.Windows.Forms.Label();
            this.txtProxyName = new System.Windows.Forms.TextBox();
            this.grpTransfer = new System.Windows.Forms.GroupBox();
            this.lblTlsMode = new System.Windows.Forms.Label();
            this.cmbTlsMode = new System.Windows.Forms.ComboBox();
            this.lblTransferProto = new System.Windows.Forms.Label();
            this.cmbTransferProtocol = new System.Windows.Forms.ComboBox();
            this.lblTlsTransfer = new System.Windows.Forms.Label();
            this.cmbTlsTransferPolicy = new System.Windows.Forms.ComboBox();
            this.lblSrcPolicy = new System.Windows.Forms.Label();
            this.cmbTransferSourcePolicy = new System.Windows.Forms.ComboBox();
            this.lblDstPolicy = new System.Windows.Forms.Label();
            this.cmbTransferTargetPolicy = new System.Windows.Forms.ComboBox();
            this.lblListCmd = new System.Windows.Forms.Label();
            this.cmbListCommand = new System.Windows.Forms.ComboBox();
            this.lblMaxIdle = new System.Windows.Forms.Label();
            this.nudMaxIdleTime = new System.Windows.Forms.NumericUpDown();
            this.chkStayLoggedIn = new System.Windows.Forms.CheckBox();
            this.chkCepr = new System.Windows.Forms.CheckBox();
            this.chkSscn = new System.Windows.Forms.CheckBox();
            this.chkCpsv = new System.Windows.Forms.CheckBox();
            this.chkBrokenPasv = new System.Windows.Forms.CheckBox();
            this.chkForceBinaryMode = new System.Windows.Forms.CheckBox();
            this.chkLeaveFreeSlot = new System.Windows.Forms.CheckBox();
            this.grpAffils = new System.Windows.Forms.GroupBox();
            this.lstAffils = new System.Windows.Forms.ListBox();
            this.txtAffil = new System.Windows.Forms.TextBox();
            this.btnAddAffil = new System.Windows.Forms.Button();
            this.btnRemoveAffil = new System.Windows.Forms.Button();
            this.grpSections = new System.Windows.Forms.GroupBox();
            this.lvSections = new System.Windows.Forms.ListView();
            this.colSectionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSectionPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSectionName = new System.Windows.Forms.Label();
            this.txtSectionName = new System.Windows.Forms.TextBox();
            this.lblSectionPath = new System.Windows.Forms.Label();
            this.txtSectionPath = new System.Windows.Forms.TextBox();
            this.btnAddSection = new System.Windows.Forms.Button();
            this.btnRemoveSection = new System.Windows.Forms.Button();
            this.grpSkiplist = new System.Windows.Forms.GroupBox();
            this.lvSkiplist = new System.Windows.Forms.ListView();
            this.colSkipAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkipScope = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkipPattern = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSkipAction = new System.Windows.Forms.Label();
            this.cmbSkipAction = new System.Windows.Forms.ComboBox();
            this.lblSkipScope = new System.Windows.Forms.Label();
            this.cmbSkipScope = new System.Windows.Forms.ComboBox();
            this.chkSkipDir = new System.Windows.Forms.CheckBox();
            this.chkSkipFile = new System.Windows.Forms.CheckBox();
            this.chkSkipRegex = new System.Windows.Forms.CheckBox();
            this.lblSkipPattern = new System.Windows.Forms.Label();
            this.txtSkipPattern = new System.Windows.Forms.TextBox();
            this.btnAddSkip = new System.Windows.Forms.Button();
            this.btnRemoveSkip = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Edit_cbftp_sites = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpBasic.SuspendLayout();
            this.grpLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxLogins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSimUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSimDown)).BeginInit();
            this.grpAllow.SuspendLayout();
            this.grpProxy.SuspendLayout();
            this.grpTransfer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxIdleTime)).BeginInit();
            this.grpAffils.SuspendLayout();
            this.grpSections.SuspendLayout();
            this.grpSkiplist.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.ForeColor = System.Drawing.Color.White;
            this.lblServer.Location = new System.Drawing.Point(10, 12);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(112, 17);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "CBFTP Server:";
            // 
            // cbftpServerCombo
            // 
            this.cbftpServerCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cbftpServerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbftpServerCombo.ForeColor = System.Drawing.Color.White;
            this.cbftpServerCombo.Location = new System.Drawing.Point(128, 9);
            this.cbftpServerCombo.Name = "cbftpServerCombo";
            this.cbftpServerCombo.Size = new System.Drawing.Size(154, 25);
            this.cbftpServerCombo.TabIndex = 1;
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblServerStatus.Location = new System.Drawing.Point(567, 11);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(267, 23);
            this.lblServerStatus.TabIndex = 2;
            // 
            // grpBasic
            // 
            this.grpBasic.Controls.Add(this.lblName);
            this.grpBasic.Controls.Add(this.txtName);
            this.grpBasic.Controls.Add(this.chkDisabled);
            this.grpBasic.Controls.Add(this.lblAddresses);
            this.grpBasic.Controls.Add(this.txtAddresses);
            this.grpBasic.Controls.Add(this.lblUser);
            this.grpBasic.Controls.Add(this.txtUser);
            this.grpBasic.Controls.Add(this.lblPassword);
            this.grpBasic.Controls.Add(this.txtPassword);
            this.grpBasic.Controls.Add(this.lblBasePath);
            this.grpBasic.Controls.Add(this.txtBasePath);
            this.grpBasic.Controls.Add(this.lblPriority);
            this.grpBasic.Controls.Add(this.cmbPriority);
            this.grpBasic.Controls.Add(this.lblListFrequency);
            this.grpBasic.Controls.Add(this.cmbListFrequency);
            this.grpBasic.ForeColor = System.Drawing.Color.White;
            this.grpBasic.Location = new System.Drawing.Point(10, 40);
            this.grpBasic.Name = "grpBasic";
            this.grpBasic.Size = new System.Drawing.Size(540, 230);
            this.grpBasic.TabIndex = 3;
            this.grpBasic.TabStop = false;
            this.grpBasic.Text = "Basic";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(15, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(48, 17);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(110, 27);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(260, 24);
            this.txtName.TabIndex = 1;
            // 
            // chkDisabled
            // 
            this.chkDisabled.AutoSize = true;
            this.chkDisabled.ForeColor = System.Drawing.Color.White;
            this.chkDisabled.Location = new System.Drawing.Point(400, 28);
            this.chkDisabled.Name = "chkDisabled";
            this.chkDisabled.Size = new System.Drawing.Size(94, 21);
            this.chkDisabled.TabIndex = 2;
            this.chkDisabled.Text = "Disabled";
            // 
            // lblAddresses
            // 
            this.lblAddresses.AutoSize = true;
            this.lblAddresses.ForeColor = System.Drawing.Color.White;
            this.lblAddresses.Location = new System.Drawing.Point(15, 60);
            this.lblAddresses.Name = "lblAddresses";
            this.lblAddresses.Size = new System.Drawing.Size(88, 17);
            this.lblAddresses.TabIndex = 3;
            this.lblAddresses.Text = "Addresses:";
            // 
            // txtAddresses
            // 
            this.txtAddresses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtAddresses.ForeColor = System.Drawing.Color.White;
            this.txtAddresses.Location = new System.Drawing.Point(110, 57);
            this.txtAddresses.Multiline = true;
            this.txtAddresses.Name = "txtAddresses";
            this.txtAddresses.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAddresses.Size = new System.Drawing.Size(410, 70);
            this.txtAddresses.TabIndex = 4;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(15, 135);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(48, 17);
            this.lblUser.TabIndex = 5;
            this.lblUser.Text = "User:";
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtUser.ForeColor = System.Drawing.Color.White;
            this.txtUser.Location = new System.Drawing.Point(110, 132);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(195, 24);
            this.txtUser.TabIndex = 6;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.ForeColor = System.Drawing.Color.White;
            this.lblPassword.Location = new System.Drawing.Point(15, 165);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(80, 17);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(110, 162);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(195, 24);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblBasePath
            // 
            this.lblBasePath.AutoSize = true;
            this.lblBasePath.ForeColor = System.Drawing.Color.White;
            this.lblBasePath.Location = new System.Drawing.Point(15, 195);
            this.lblBasePath.Name = "lblBasePath";
            this.lblBasePath.Size = new System.Drawing.Size(88, 17);
            this.lblBasePath.TabIndex = 9;
            this.lblBasePath.Text = "Base path:";
            // 
            // txtBasePath
            // 
            this.txtBasePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtBasePath.ForeColor = System.Drawing.Color.White;
            this.txtBasePath.Location = new System.Drawing.Point(110, 192);
            this.txtBasePath.Name = "txtBasePath";
            this.txtBasePath.Size = new System.Drawing.Size(195, 24);
            this.txtBasePath.TabIndex = 10;
            this.txtBasePath.Text = "/";
            // 
            // lblPriority
            // 
            this.lblPriority.AutoSize = true;
            this.lblPriority.ForeColor = System.Drawing.Color.White;
            this.lblPriority.Location = new System.Drawing.Point(311, 139);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(80, 17);
            this.lblPriority.TabIndex = 11;
            this.lblPriority.Text = "Priority:";
            // 
            // cmbPriority
            // 
            this.cmbPriority.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriority.ForeColor = System.Drawing.Color.White;
            this.cmbPriority.Items.AddRange(new object[] {
            "VERY_LOW",
            "LOW",
            "NORMAL",
            "HIGH",
            "VERY_HIGH"});
            this.cmbPriority.Location = new System.Drawing.Point(405, 136);
            this.cmbPriority.Name = "cmbPriority";
            this.cmbPriority.Size = new System.Drawing.Size(115, 25);
            this.cmbPriority.TabIndex = 12;
            // 
            // lblListFrequency
            // 
            this.lblListFrequency.AutoSize = true;
            this.lblListFrequency.ForeColor = System.Drawing.Color.White;
            this.lblListFrequency.Location = new System.Drawing.Point(311, 169);
            this.lblListFrequency.Name = "lblListFrequency";
            this.lblListFrequency.Size = new System.Drawing.Size(88, 17);
            this.lblListFrequency.TabIndex = 13;
            this.lblListFrequency.Text = "List freq:";
            // 
            // cmbListFrequency
            // 
            this.cmbListFrequency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbListFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbListFrequency.ForeColor = System.Drawing.Color.White;
            this.cmbListFrequency.Items.AddRange(new object[] {
            "VERY_LOW",
            "FIXED_LOW",
            "FIXED_AVERAGE",
            "FIXED_HIGH",
            "FIXED_VERY_HIGH",
            "AUTO",
            "DYNAMIC_LOW",
            "DYNAMIC_AVERAGE",
            "DYNAMIC_HIGH",
            "DYNAMIC_VERY_HIGH"});
            this.cmbListFrequency.Location = new System.Drawing.Point(405, 167);
            this.cmbListFrequency.Name = "cmbListFrequency";
            this.cmbListFrequency.Size = new System.Drawing.Size(115, 25);
            this.cmbListFrequency.TabIndex = 14;
            // 
            // grpLimits
            // 
            this.grpLimits.Controls.Add(this.lblMaxLogins);
            this.grpLimits.Controls.Add(this.nudMaxLogins);
            this.grpLimits.Controls.Add(this.lblMaxSimUp);
            this.grpLimits.Controls.Add(this.nudMaxSimUp);
            this.grpLimits.Controls.Add(this.lblMaxSimDown);
            this.grpLimits.Controls.Add(this.nudMaxSimDown);
            this.grpLimits.ForeColor = System.Drawing.Color.White;
            this.grpLimits.Location = new System.Drawing.Point(10, 280);
            this.grpLimits.Name = "grpLimits";
            this.grpLimits.Size = new System.Drawing.Size(260, 140);
            this.grpLimits.TabIndex = 5;
            this.grpLimits.TabStop = false;
            this.grpLimits.Text = "Limits";
            // 
            // lblMaxLogins
            // 
            this.lblMaxLogins.AutoSize = true;
            this.lblMaxLogins.ForeColor = System.Drawing.Color.White;
            this.lblMaxLogins.Location = new System.Drawing.Point(15, 30);
            this.lblMaxLogins.Name = "lblMaxLogins";
            this.lblMaxLogins.Size = new System.Drawing.Size(96, 17);
            this.lblMaxLogins.TabIndex = 0;
            this.lblMaxLogins.Text = "Max logins:";
            // 
            // nudMaxLogins
            // 
            this.nudMaxLogins.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.nudMaxLogins.ForeColor = System.Drawing.Color.White;
            this.nudMaxLogins.Location = new System.Drawing.Point(135, 27);
            this.nudMaxLogins.Name = "nudMaxLogins";
            this.nudMaxLogins.Size = new System.Drawing.Size(120, 24);
            this.nudMaxLogins.TabIndex = 1;
            this.nudMaxLogins.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblMaxSimUp
            // 
            this.lblMaxSimUp.AutoSize = true;
            this.lblMaxSimUp.ForeColor = System.Drawing.Color.White;
            this.lblMaxSimUp.Location = new System.Drawing.Point(15, 60);
            this.lblMaxSimUp.Name = "lblMaxSimUp";
            this.lblMaxSimUp.Size = new System.Drawing.Size(104, 17);
            this.lblMaxSimUp.TabIndex = 2;
            this.lblMaxSimUp.Text = "Sim uploads:";
            // 
            // nudMaxSimUp
            // 
            this.nudMaxSimUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.nudMaxSimUp.ForeColor = System.Drawing.Color.White;
            this.nudMaxSimUp.Location = new System.Drawing.Point(135, 57);
            this.nudMaxSimUp.Name = "nudMaxSimUp";
            this.nudMaxSimUp.Size = new System.Drawing.Size(120, 24);
            this.nudMaxSimUp.TabIndex = 3;
            this.nudMaxSimUp.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblMaxSimDown
            // 
            this.lblMaxSimDown.AutoSize = true;
            this.lblMaxSimDown.ForeColor = System.Drawing.Color.White;
            this.lblMaxSimDown.Location = new System.Drawing.Point(15, 90);
            this.lblMaxSimDown.Name = "lblMaxSimDown";
            this.lblMaxSimDown.Size = new System.Drawing.Size(120, 17);
            this.lblMaxSimDown.TabIndex = 4;
            this.lblMaxSimDown.Text = "Sim downloads:";
            // 
            // nudMaxSimDown
            // 
            this.nudMaxSimDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.nudMaxSimDown.ForeColor = System.Drawing.Color.White;
            this.nudMaxSimDown.Location = new System.Drawing.Point(135, 87);
            this.nudMaxSimDown.Name = "nudMaxSimDown";
            this.nudMaxSimDown.Size = new System.Drawing.Size(120, 24);
            this.nudMaxSimDown.TabIndex = 5;
            this.nudMaxSimDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // grpAllow
            // 
            this.grpAllow.Controls.Add(this.lblAllowDownload);
            this.grpAllow.Controls.Add(this.cmbAllowDownload);
            this.grpAllow.Controls.Add(this.lblAllowUpload);
            this.grpAllow.Controls.Add(this.cmbAllowUpload);
            this.grpAllow.Controls.Add(this.chkPret);
            this.grpAllow.Controls.Add(this.chkXdupe);
            this.grpAllow.ForeColor = System.Drawing.Color.White;
            this.grpAllow.Location = new System.Drawing.Point(280, 280);
            this.grpAllow.Name = "grpAllow";
            this.grpAllow.Size = new System.Drawing.Size(270, 140);
            this.grpAllow.TabIndex = 6;
            this.grpAllow.TabStop = false;
            this.grpAllow.Text = "Allow";
            // 
            // lblAllowDownload
            // 
            this.lblAllowDownload.AutoSize = true;
            this.lblAllowDownload.ForeColor = System.Drawing.Color.White;
            this.lblAllowDownload.Location = new System.Drawing.Point(15, 30);
            this.lblAllowDownload.Name = "lblAllowDownload";
            this.lblAllowDownload.Size = new System.Drawing.Size(80, 17);
            this.lblAllowDownload.TabIndex = 0;
            this.lblAllowDownload.Text = "Download:";
            // 
            // cmbAllowDownload
            // 
            this.cmbAllowDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbAllowDownload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAllowDownload.ForeColor = System.Drawing.Color.White;
            this.cmbAllowDownload.Items.AddRange(new object[] {
            "YES",
            "NO",
            "MATCH_ONLY"});
            this.cmbAllowDownload.Location = new System.Drawing.Point(100, 27);
            this.cmbAllowDownload.Name = "cmbAllowDownload";
            this.cmbAllowDownload.Size = new System.Drawing.Size(140, 25);
            this.cmbAllowDownload.TabIndex = 1;
            // 
            // lblAllowUpload
            // 
            this.lblAllowUpload.AutoSize = true;
            this.lblAllowUpload.ForeColor = System.Drawing.Color.White;
            this.lblAllowUpload.Location = new System.Drawing.Point(15, 60);
            this.lblAllowUpload.Name = "lblAllowUpload";
            this.lblAllowUpload.Size = new System.Drawing.Size(64, 17);
            this.lblAllowUpload.TabIndex = 2;
            this.lblAllowUpload.Text = "Upload:";
            // 
            // cmbAllowUpload
            // 
            this.cmbAllowUpload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbAllowUpload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAllowUpload.ForeColor = System.Drawing.Color.White;
            this.cmbAllowUpload.Items.AddRange(new object[] {
            "YES",
            "NO"});
            this.cmbAllowUpload.Location = new System.Drawing.Point(100, 57);
            this.cmbAllowUpload.Name = "cmbAllowUpload";
            this.cmbAllowUpload.Size = new System.Drawing.Size(140, 25);
            this.cmbAllowUpload.TabIndex = 3;
            // 
            // chkPret
            // 
            this.chkPret.AutoSize = true;
            this.chkPret.ForeColor = System.Drawing.Color.White;
            this.chkPret.Location = new System.Drawing.Point(15, 95);
            this.chkPret.Name = "chkPret";
            this.chkPret.Size = new System.Drawing.Size(110, 21);
            this.chkPret.TabIndex = 4;
            this.chkPret.Text = "Needs PRET";
            // 
            // chkXdupe
            // 
            this.chkXdupe.AutoSize = true;
            this.chkXdupe.ForeColor = System.Drawing.Color.White;
            this.chkXdupe.Location = new System.Drawing.Point(140, 95);
            this.chkXdupe.Name = "chkXdupe";
            this.chkXdupe.Size = new System.Drawing.Size(102, 21);
            this.chkXdupe.TabIndex = 5;
            this.chkXdupe.Text = "Use XDUPE";
            // 
            // grpProxy
            // 
            this.grpProxy.Controls.Add(this.lblProxyType);
            this.grpProxy.Controls.Add(this.cmbProxyType);
            this.grpProxy.Controls.Add(this.lblProxyName);
            this.grpProxy.Controls.Add(this.txtProxyName);
            this.grpProxy.ForeColor = System.Drawing.Color.White;
            this.grpProxy.Location = new System.Drawing.Point(10, 430);
            this.grpProxy.Name = "grpProxy";
            this.grpProxy.Size = new System.Drawing.Size(174, 390);
            this.grpProxy.TabIndex = 7;
            this.grpProxy.TabStop = false;
            this.grpProxy.Text = "Proxy";
            // 
            // lblProxyType
            // 
            this.lblProxyType.AutoSize = true;
            this.lblProxyType.ForeColor = System.Drawing.Color.White;
            this.lblProxyType.Location = new System.Drawing.Point(15, 30);
            this.lblProxyType.Name = "lblProxyType";
            this.lblProxyType.Size = new System.Drawing.Size(48, 17);
            this.lblProxyType.TabIndex = 0;
            this.lblProxyType.Text = "Type:";
            // 
            // cmbProxyType
            // 
            this.cmbProxyType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbProxyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProxyType.ForeColor = System.Drawing.Color.White;
            this.cmbProxyType.Items.AddRange(new object[] {
            "GLOBAL",
            "NONE",
            "USE"});
            this.cmbProxyType.Location = new System.Drawing.Point(70, 27);
            this.cmbProxyType.Name = "cmbProxyType";
            this.cmbProxyType.Size = new System.Drawing.Size(86, 25);
            this.cmbProxyType.TabIndex = 1;
            // 
            // lblProxyName
            // 
            this.lblProxyName.AutoSize = true;
            this.lblProxyName.ForeColor = System.Drawing.Color.White;
            this.lblProxyName.Location = new System.Drawing.Point(15, 60);
            this.lblProxyName.Name = "lblProxyName";
            this.lblProxyName.Size = new System.Drawing.Size(48, 17);
            this.lblProxyName.TabIndex = 2;
            this.lblProxyName.Text = "Name:";
            // 
            // txtProxyName
            // 
            this.txtProxyName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtProxyName.ForeColor = System.Drawing.Color.White;
            this.txtProxyName.Location = new System.Drawing.Point(70, 57);
            this.txtProxyName.Name = "txtProxyName";
            this.txtProxyName.Size = new System.Drawing.Size(86, 24);
            this.txtProxyName.TabIndex = 3;
            // 
            // grpTransfer
            // 
            this.grpTransfer.Controls.Add(this.lblTlsMode);
            this.grpTransfer.Controls.Add(this.cmbTlsMode);
            this.grpTransfer.Controls.Add(this.lblTransferProto);
            this.grpTransfer.Controls.Add(this.cmbTransferProtocol);
            this.grpTransfer.Controls.Add(this.lblTlsTransfer);
            this.grpTransfer.Controls.Add(this.cmbTlsTransferPolicy);
            this.grpTransfer.Controls.Add(this.lblSrcPolicy);
            this.grpTransfer.Controls.Add(this.cmbTransferSourcePolicy);
            this.grpTransfer.Controls.Add(this.lblDstPolicy);
            this.grpTransfer.Controls.Add(this.cmbTransferTargetPolicy);
            this.grpTransfer.Controls.Add(this.lblListCmd);
            this.grpTransfer.Controls.Add(this.cmbListCommand);
            this.grpTransfer.Controls.Add(this.lblMaxIdle);
            this.grpTransfer.Controls.Add(this.nudMaxIdleTime);
            this.grpTransfer.Controls.Add(this.chkStayLoggedIn);
            this.grpTransfer.Controls.Add(this.chkCepr);
            this.grpTransfer.Controls.Add(this.chkSscn);
            this.grpTransfer.Controls.Add(this.chkCpsv);
            this.grpTransfer.Controls.Add(this.chkBrokenPasv);
            this.grpTransfer.Controls.Add(this.chkForceBinaryMode);
            this.grpTransfer.Controls.Add(this.chkLeaveFreeSlot);
            this.grpTransfer.ForeColor = System.Drawing.Color.White;
            this.grpTransfer.Location = new System.Drawing.Point(560, 40);
            this.grpTransfer.Name = "grpTransfer";
            this.grpTransfer.Size = new System.Drawing.Size(552, 230);
            this.grpTransfer.TabIndex = 4;
            this.grpTransfer.TabStop = false;
            this.grpTransfer.Text = "TLS / Transfer";
            // 
            // lblTlsMode
            // 
            this.lblTlsMode.AutoSize = true;
            this.lblTlsMode.ForeColor = System.Drawing.Color.White;
            this.lblTlsMode.Location = new System.Drawing.Point(15, 30);
            this.lblTlsMode.Name = "lblTlsMode";
            this.lblTlsMode.Size = new System.Drawing.Size(80, 17);
            this.lblTlsMode.TabIndex = 0;
            this.lblTlsMode.Text = "TLS mode:";
            // 
            // cmbTlsMode
            // 
            this.cmbTlsMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbTlsMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTlsMode.ForeColor = System.Drawing.Color.White;
            this.cmbTlsMode.Items.AddRange(new object[] {
            "NONE",
            "AUTH_TLS",
            "IMPLICIT"});
            this.cmbTlsMode.Location = new System.Drawing.Point(145, 27);
            this.cmbTlsMode.Name = "cmbTlsMode";
            this.cmbTlsMode.Size = new System.Drawing.Size(132, 25);
            this.cmbTlsMode.TabIndex = 1;
            // 
            // lblTransferProto
            // 
            this.lblTransferProto.AutoSize = true;
            this.lblTransferProto.ForeColor = System.Drawing.Color.White;
            this.lblTransferProto.Location = new System.Drawing.Point(15, 60);
            this.lblTransferProto.Name = "lblTransferProto";
            this.lblTransferProto.Size = new System.Drawing.Size(128, 17);
            this.lblTransferProto.TabIndex = 2;
            this.lblTransferProto.Text = "Transfer proto:";
            // 
            // cmbTransferProtocol
            // 
            this.cmbTransferProtocol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbTransferProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferProtocol.ForeColor = System.Drawing.Color.White;
            this.cmbTransferProtocol.Items.AddRange(new object[] {
            "IPV4_ONLY",
            "PREFER_IPV4",
            "PREFER_IPV6",
            "IPV6_ONLY"});
            this.cmbTransferProtocol.Location = new System.Drawing.Point(145, 56);
            this.cmbTransferProtocol.Name = "cmbTransferProtocol";
            this.cmbTransferProtocol.Size = new System.Drawing.Size(132, 25);
            this.cmbTransferProtocol.TabIndex = 3;
            // 
            // lblTlsTransfer
            // 
            this.lblTlsTransfer.AutoSize = true;
            this.lblTlsTransfer.ForeColor = System.Drawing.Color.White;
            this.lblTlsTransfer.Location = new System.Drawing.Point(15, 90);
            this.lblTlsTransfer.Name = "lblTlsTransfer";
            this.lblTlsTransfer.Size = new System.Drawing.Size(112, 17);
            this.lblTlsTransfer.TabIndex = 4;
            this.lblTlsTransfer.Text = "TLS transfer:";
            // 
            // cmbTlsTransferPolicy
            // 
            this.cmbTlsTransferPolicy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbTlsTransferPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTlsTransferPolicy.ForeColor = System.Drawing.Color.White;
            this.cmbTlsTransferPolicy.Items.AddRange(new object[] {
            "ALWAYS_OFF",
            "PREFER_OFF",
            "PREFER_ON",
            "ALWAYS_ON"});
            this.cmbTlsTransferPolicy.Location = new System.Drawing.Point(145, 87);
            this.cmbTlsTransferPolicy.Name = "cmbTlsTransferPolicy";
            this.cmbTlsTransferPolicy.Size = new System.Drawing.Size(132, 25);
            this.cmbTlsTransferPolicy.TabIndex = 5;
            // 
            // lblSrcPolicy
            // 
            this.lblSrcPolicy.AutoSize = true;
            this.lblSrcPolicy.ForeColor = System.Drawing.Color.White;
            this.lblSrcPolicy.Location = new System.Drawing.Point(15, 120);
            this.lblSrcPolicy.Name = "lblSrcPolicy";
            this.lblSrcPolicy.Size = new System.Drawing.Size(96, 17);
            this.lblSrcPolicy.TabIndex = 6;
            this.lblSrcPolicy.Text = "Src policy:";
            // 
            // cmbTransferSourcePolicy
            // 
            this.cmbTransferSourcePolicy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbTransferSourcePolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferSourcePolicy.ForeColor = System.Drawing.Color.White;
            this.cmbTransferSourcePolicy.Items.AddRange(new object[] {
            "ALLOW",
            "BLOCK"});
            this.cmbTransferSourcePolicy.Location = new System.Drawing.Point(145, 117);
            this.cmbTransferSourcePolicy.Name = "cmbTransferSourcePolicy";
            this.cmbTransferSourcePolicy.Size = new System.Drawing.Size(132, 25);
            this.cmbTransferSourcePolicy.TabIndex = 7;
            // 
            // lblDstPolicy
            // 
            this.lblDstPolicy.AutoSize = true;
            this.lblDstPolicy.ForeColor = System.Drawing.Color.White;
            this.lblDstPolicy.Location = new System.Drawing.Point(15, 150);
            this.lblDstPolicy.Name = "lblDstPolicy";
            this.lblDstPolicy.Size = new System.Drawing.Size(96, 17);
            this.lblDstPolicy.TabIndex = 8;
            this.lblDstPolicy.Text = "Dst policy:";
            // 
            // cmbTransferTargetPolicy
            // 
            this.cmbTransferTargetPolicy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbTransferTargetPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferTargetPolicy.ForeColor = System.Drawing.Color.White;
            this.cmbTransferTargetPolicy.Items.AddRange(new object[] {
            "ALLOW",
            "BLOCK",
            "MATCH_ONLY"});
            this.cmbTransferTargetPolicy.Location = new System.Drawing.Point(145, 147);
            this.cmbTransferTargetPolicy.Name = "cmbTransferTargetPolicy";
            this.cmbTransferTargetPolicy.Size = new System.Drawing.Size(132, 25);
            this.cmbTransferTargetPolicy.TabIndex = 9;
            // 
            // lblListCmd
            // 
            this.lblListCmd.AutoSize = true;
            this.lblListCmd.ForeColor = System.Drawing.Color.White;
            this.lblListCmd.Location = new System.Drawing.Point(317, 35);
            this.lblListCmd.Name = "lblListCmd";
            this.lblListCmd.Size = new System.Drawing.Size(80, 17);
            this.lblListCmd.TabIndex = 10;
            this.lblListCmd.Text = "List cmd:";
            // 
            // cmbListCommand
            // 
            this.cmbListCommand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbListCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbListCommand.ForeColor = System.Drawing.Color.White;
            this.cmbListCommand.Items.AddRange(new object[] {
            "STAT_L",
            "LIST"});
            this.cmbListCommand.Location = new System.Drawing.Point(432, 29);
            this.cmbListCommand.Name = "cmbListCommand";
            this.cmbListCommand.Size = new System.Drawing.Size(98, 25);
            this.cmbListCommand.TabIndex = 11;
            // 
            // lblMaxIdle
            // 
            this.lblMaxIdle.AutoSize = true;
            this.lblMaxIdle.ForeColor = System.Drawing.Color.White;
            this.lblMaxIdle.Location = new System.Drawing.Point(317, 60);
            this.lblMaxIdle.Name = "lblMaxIdle";
            this.lblMaxIdle.Size = new System.Drawing.Size(112, 17);
            this.lblMaxIdle.TabIndex = 12;
            this.lblMaxIdle.Text = "Max idle (s):";
            // 
            // nudMaxIdleTime
            // 
            this.nudMaxIdleTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.nudMaxIdleTime.ForeColor = System.Drawing.Color.White;
            this.nudMaxIdleTime.Location = new System.Drawing.Point(432, 60);
            this.nudMaxIdleTime.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudMaxIdleTime.Name = "nudMaxIdleTime";
            this.nudMaxIdleTime.Size = new System.Drawing.Size(98, 24);
            this.nudMaxIdleTime.TabIndex = 13;
            this.nudMaxIdleTime.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // chkStayLoggedIn
            // 
            this.chkStayLoggedIn.AutoSize = true;
            this.chkStayLoggedIn.ForeColor = System.Drawing.Color.White;
            this.chkStayLoggedIn.Location = new System.Drawing.Point(320, 90);
            this.chkStayLoggedIn.Name = "chkStayLoggedIn";
            this.chkStayLoggedIn.Size = new System.Drawing.Size(142, 21);
            this.chkStayLoggedIn.TabIndex = 14;
            this.chkStayLoggedIn.Text = "Stay logged in";
            // 
            // chkCepr
            // 
            this.chkCepr.AutoSize = true;
            this.chkCepr.ForeColor = System.Drawing.Color.White;
            this.chkCepr.Location = new System.Drawing.Point(320, 110);
            this.chkCepr.Name = "chkCepr";
            this.chkCepr.Size = new System.Drawing.Size(62, 21);
            this.chkCepr.TabIndex = 15;
            this.chkCepr.Text = "CEPR";
            // 
            // chkSscn
            // 
            this.chkSscn.AutoSize = true;
            this.chkSscn.ForeColor = System.Drawing.Color.White;
            this.chkSscn.Location = new System.Drawing.Point(320, 130);
            this.chkSscn.Name = "chkSscn";
            this.chkSscn.Size = new System.Drawing.Size(62, 21);
            this.chkSscn.TabIndex = 16;
            this.chkSscn.Text = "SSCN";
            // 
            // chkCpsv
            // 
            this.chkCpsv.AutoSize = true;
            this.chkCpsv.ForeColor = System.Drawing.Color.White;
            this.chkCpsv.Location = new System.Drawing.Point(320, 150);
            this.chkCpsv.Name = "chkCpsv";
            this.chkCpsv.Size = new System.Drawing.Size(62, 21);
            this.chkCpsv.TabIndex = 17;
            this.chkCpsv.Text = "CPSV";
            // 
            // chkBrokenPasv
            // 
            this.chkBrokenPasv.AutoSize = true;
            this.chkBrokenPasv.ForeColor = System.Drawing.Color.White;
            this.chkBrokenPasv.Location = new System.Drawing.Point(15, 185);
            this.chkBrokenPasv.Name = "chkBrokenPasv";
            this.chkBrokenPasv.Size = new System.Drawing.Size(118, 21);
            this.chkBrokenPasv.TabIndex = 18;
            this.chkBrokenPasv.Text = "Broken PASV";
            // 
            // chkForceBinaryMode
            // 
            this.chkForceBinaryMode.AutoSize = true;
            this.chkForceBinaryMode.ForeColor = System.Drawing.Color.White;
            this.chkForceBinaryMode.Location = new System.Drawing.Point(145, 185);
            this.chkForceBinaryMode.Name = "chkForceBinaryMode";
            this.chkForceBinaryMode.Size = new System.Drawing.Size(166, 21);
            this.chkForceBinaryMode.TabIndex = 19;
            this.chkForceBinaryMode.Text = "Force binary mode";
            // 
            // chkLeaveFreeSlot
            // 
            this.chkLeaveFreeSlot.AutoSize = true;
            this.chkLeaveFreeSlot.ForeColor = System.Drawing.Color.White;
            this.chkLeaveFreeSlot.Location = new System.Drawing.Point(312, 185);
            this.chkLeaveFreeSlot.Name = "chkLeaveFreeSlot";
            this.chkLeaveFreeSlot.Size = new System.Drawing.Size(150, 21);
            this.chkLeaveFreeSlot.TabIndex = 20;
            this.chkLeaveFreeSlot.Text = "Leave free slot";
            // 
            // grpAffils
            // 
            this.grpAffils.Controls.Add(this.lstAffils);
            this.grpAffils.Controls.Add(this.txtAffil);
            this.grpAffils.Controls.Add(this.btnAddAffil);
            this.grpAffils.Controls.Add(this.btnRemoveAffil);
            this.grpAffils.ForeColor = System.Drawing.Color.White;
            this.grpAffils.Location = new System.Drawing.Point(197, 430);
            this.grpAffils.Name = "grpAffils";
            this.grpAffils.Size = new System.Drawing.Size(353, 390);
            this.grpAffils.TabIndex = 8;
            this.grpAffils.TabStop = false;
            this.grpAffils.Text = "Affils";
            // 
            // lstAffils
            // 
            this.lstAffils.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lstAffils.ForeColor = System.Drawing.Color.White;
            this.lstAffils.ItemHeight = 17;
            this.lstAffils.Location = new System.Drawing.Point(10, 20);
            this.lstAffils.Name = "lstAffils";
            this.lstAffils.Size = new System.Drawing.Size(334, 327);
            this.lstAffils.TabIndex = 0;
            // 
            // txtAffil
            // 
            this.txtAffil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtAffil.ForeColor = System.Drawing.Color.White;
            this.txtAffil.Location = new System.Drawing.Point(10, 355);
            this.txtAffil.Name = "txtAffil";
            this.txtAffil.Size = new System.Drawing.Size(182, 24);
            this.txtAffil.TabIndex = 1;
            // 
            // btnAddAffil
            // 
            this.btnAddAffil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.btnAddAffil.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnAddAffil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAffil.ForeColor = System.Drawing.Color.White;
            this.btnAddAffil.Location = new System.Drawing.Point(198, 355);
            this.btnAddAffil.Name = "btnAddAffil";
            this.btnAddAffil.Size = new System.Drawing.Size(70, 25);
            this.btnAddAffil.TabIndex = 2;
            this.btnAddAffil.Text = "Add";
            this.btnAddAffil.UseVisualStyleBackColor = false;
            this.btnAddAffil.Click += new System.EventHandler(this.btnAddAffil_Click);
            // 
            // btnRemoveAffil
            // 
            this.btnRemoveAffil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.btnRemoveAffil.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRemoveAffil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAffil.ForeColor = System.Drawing.Color.White;
            this.btnRemoveAffil.Location = new System.Drawing.Point(274, 355);
            this.btnRemoveAffil.Name = "btnRemoveAffil";
            this.btnRemoveAffil.Size = new System.Drawing.Size(70, 25);
            this.btnRemoveAffil.TabIndex = 3;
            this.btnRemoveAffil.Text = "Remove";
            this.btnRemoveAffil.UseVisualStyleBackColor = false;
            this.btnRemoveAffil.Click += new System.EventHandler(this.btnRemoveAffil_Click);
            // 
            // grpSections
            // 
            this.grpSections.Controls.Add(this.lvSections);
            this.grpSections.Controls.Add(this.lblSectionName);
            this.grpSections.Controls.Add(this.txtSectionName);
            this.grpSections.Controls.Add(this.lblSectionPath);
            this.grpSections.Controls.Add(this.txtSectionPath);
            this.grpSections.Controls.Add(this.btnAddSection);
            this.grpSections.Controls.Add(this.btnRemoveSection);
            this.grpSections.ForeColor = System.Drawing.Color.White;
            this.grpSections.Location = new System.Drawing.Point(560, 527);
            this.grpSections.Name = "grpSections";
            this.grpSections.Size = new System.Drawing.Size(552, 293);
            this.grpSections.TabIndex = 9;
            this.grpSections.TabStop = false;
            this.grpSections.Text = "Sections";
            // 
            // lvSections
            // 
            this.lvSections.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lvSections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSectionName,
            this.colSectionPath});
            this.lvSections.ForeColor = System.Drawing.Color.White;
            this.lvSections.FullRowSelect = true;
            this.lvSections.HideSelection = false;
            this.lvSections.Location = new System.Drawing.Point(10, 20);
            this.lvSections.MultiSelect = false;
            this.lvSections.Name = "lvSections";
            this.lvSections.Size = new System.Drawing.Size(524, 232);
            this.lvSections.TabIndex = 0;
            this.lvSections.UseCompatibleStateImageBehavior = false;
            this.lvSections.View = System.Windows.Forms.View.Details;
            // 
            // colSectionName
            // 
            this.colSectionName.Text = "Name";
            this.colSectionName.Width = 80;
            // 
            // colSectionPath
            // 
            this.colSectionPath.Text = "Path";
            this.colSectionPath.Width = 150;
            // 
            // lblSectionName
            // 
            this.lblSectionName.AutoSize = true;
            this.lblSectionName.ForeColor = System.Drawing.Color.White;
            this.lblSectionName.Location = new System.Drawing.Point(10, 262);
            this.lblSectionName.Name = "lblSectionName";
            this.lblSectionName.Size = new System.Drawing.Size(48, 17);
            this.lblSectionName.TabIndex = 1;
            this.lblSectionName.Text = "Name:";
            // 
            // txtSectionName
            // 
            this.txtSectionName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtSectionName.ForeColor = System.Drawing.Color.White;
            this.txtSectionName.Location = new System.Drawing.Point(64, 259);
            this.txtSectionName.Name = "txtSectionName";
            this.txtSectionName.Size = new System.Drawing.Size(119, 24);
            this.txtSectionName.TabIndex = 2;
            // 
            // lblSectionPath
            // 
            this.lblSectionPath.AutoSize = true;
            this.lblSectionPath.ForeColor = System.Drawing.Color.White;
            this.lblSectionPath.Location = new System.Drawing.Point(189, 260);
            this.lblSectionPath.Name = "lblSectionPath";
            this.lblSectionPath.Size = new System.Drawing.Size(48, 17);
            this.lblSectionPath.TabIndex = 3;
            this.lblSectionPath.Text = "Path:";
            // 
            // txtSectionPath
            // 
            this.txtSectionPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtSectionPath.ForeColor = System.Drawing.Color.White;
            this.txtSectionPath.Location = new System.Drawing.Point(243, 259);
            this.txtSectionPath.Name = "txtSectionPath";
            this.txtSectionPath.Size = new System.Drawing.Size(168, 24);
            this.txtSectionPath.TabIndex = 4;
            // 
            // btnAddSection
            // 
            this.btnAddSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.btnAddSection.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnAddSection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSection.ForeColor = System.Drawing.Color.White;
            this.btnAddSection.Location = new System.Drawing.Point(417, 258);
            this.btnAddSection.Name = "btnAddSection";
            this.btnAddSection.Size = new System.Drawing.Size(45, 25);
            this.btnAddSection.TabIndex = 5;
            this.btnAddSection.Text = "Add";
            this.btnAddSection.UseVisualStyleBackColor = false;
            this.btnAddSection.Click += new System.EventHandler(this.btnAddSection_Click);
            // 
            // btnRemoveSection
            // 
            this.btnRemoveSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.btnRemoveSection.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRemoveSection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveSection.ForeColor = System.Drawing.Color.White;
            this.btnRemoveSection.Location = new System.Drawing.Point(468, 258);
            this.btnRemoveSection.Name = "btnRemoveSection";
            this.btnRemoveSection.Size = new System.Drawing.Size(66, 25);
            this.btnRemoveSection.TabIndex = 6;
            this.btnRemoveSection.Text = "Remove";
            this.btnRemoveSection.UseVisualStyleBackColor = false;
            this.btnRemoveSection.Click += new System.EventHandler(this.btnRemoveSection_Click);
            // 
            // grpSkiplist
            // 
            this.grpSkiplist.Controls.Add(this.lvSkiplist);
            this.grpSkiplist.Controls.Add(this.lblSkipAction);
            this.grpSkiplist.Controls.Add(this.cmbSkipAction);
            this.grpSkiplist.Controls.Add(this.lblSkipScope);
            this.grpSkiplist.Controls.Add(this.cmbSkipScope);
            this.grpSkiplist.Controls.Add(this.chkSkipDir);
            this.grpSkiplist.Controls.Add(this.chkSkipFile);
            this.grpSkiplist.Controls.Add(this.chkSkipRegex);
            this.grpSkiplist.Controls.Add(this.lblSkipPattern);
            this.grpSkiplist.Controls.Add(this.txtSkipPattern);
            this.grpSkiplist.Controls.Add(this.btnAddSkip);
            this.grpSkiplist.Controls.Add(this.btnRemoveSkip);
            this.grpSkiplist.ForeColor = System.Drawing.Color.White;
            this.grpSkiplist.Location = new System.Drawing.Point(560, 280);
            this.grpSkiplist.Name = "grpSkiplist";
            this.grpSkiplist.Size = new System.Drawing.Size(552, 241);
            this.grpSkiplist.TabIndex = 10;
            this.grpSkiplist.TabStop = false;
            this.grpSkiplist.Text = "Skiplist";
            // 
            // lvSkiplist
            // 
            this.lvSkiplist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lvSkiplist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSkipAction,
            this.colSkipScope,
            this.colSkipPattern});
            this.lvSkiplist.ForeColor = System.Drawing.Color.White;
            this.lvSkiplist.FullRowSelect = true;
            this.lvSkiplist.HideSelection = false;
            this.lvSkiplist.Location = new System.Drawing.Point(9, 23);
            this.lvSkiplist.MultiSelect = false;
            this.lvSkiplist.Name = "lvSkiplist";
            this.lvSkiplist.Size = new System.Drawing.Size(521, 136);
            this.lvSkiplist.TabIndex = 0;
            this.lvSkiplist.UseCompatibleStateImageBehavior = false;
            this.lvSkiplist.View = System.Windows.Forms.View.Details;
            // 
            // colSkipAction
            // 
            this.colSkipAction.Text = "Action";
            this.colSkipAction.Width = 80;
            // 
            // colSkipScope
            // 
            this.colSkipScope.Text = "Scope";
            this.colSkipScope.Width = 80;
            // 
            // colSkipPattern
            // 
            this.colSkipPattern.Text = "Pattern";
            this.colSkipPattern.Width = 340;
            // 
            // lblSkipAction
            // 
            this.lblSkipAction.AutoSize = true;
            this.lblSkipAction.ForeColor = System.Drawing.Color.White;
            this.lblSkipAction.Location = new System.Drawing.Point(6, 169);
            this.lblSkipAction.Name = "lblSkipAction";
            this.lblSkipAction.Size = new System.Drawing.Size(64, 17);
            this.lblSkipAction.TabIndex = 1;
            this.lblSkipAction.Text = "Action:";
            // 
            // cmbSkipAction
            // 
            this.cmbSkipAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbSkipAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSkipAction.ForeColor = System.Drawing.Color.White;
            this.cmbSkipAction.Items.AddRange(new object[] {
            "ALLOW",
            "DENY",
            "UNIQUE",
            "SIMILAR"});
            this.cmbSkipAction.Location = new System.Drawing.Point(76, 165);
            this.cmbSkipAction.Name = "cmbSkipAction";
            this.cmbSkipAction.Size = new System.Drawing.Size(90, 25);
            this.cmbSkipAction.TabIndex = 2;
            // 
            // lblSkipScope
            // 
            this.lblSkipScope.AutoSize = true;
            this.lblSkipScope.ForeColor = System.Drawing.Color.White;
            this.lblSkipScope.Location = new System.Drawing.Point(172, 169);
            this.lblSkipScope.Name = "lblSkipScope";
            this.lblSkipScope.Size = new System.Drawing.Size(56, 17);
            this.lblSkipScope.TabIndex = 3;
            this.lblSkipScope.Text = "Scope:";
            // 
            // cmbSkipScope
            // 
            this.cmbSkipScope.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbSkipScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSkipScope.ForeColor = System.Drawing.Color.White;
            this.cmbSkipScope.Items.AddRange(new object[] {
            "IN_RACE",
            "ALL"});
            this.cmbSkipScope.Location = new System.Drawing.Point(234, 165);
            this.cmbSkipScope.Name = "cmbSkipScope";
            this.cmbSkipScope.Size = new System.Drawing.Size(90, 25);
            this.cmbSkipScope.TabIndex = 4;
            // 
            // chkSkipDir
            // 
            this.chkSkipDir.AutoSize = true;
            this.chkSkipDir.ForeColor = System.Drawing.Color.White;
            this.chkSkipDir.Location = new System.Drawing.Point(416, 169);
            this.chkSkipDir.Name = "chkSkipDir";
            this.chkSkipDir.Size = new System.Drawing.Size(54, 21);
            this.chkSkipDir.TabIndex = 5;
            this.chkSkipDir.Text = "Dir";
            // 
            // chkSkipFile
            // 
            this.chkSkipFile.AutoSize = true;
            this.chkSkipFile.ForeColor = System.Drawing.Color.White;
            this.chkSkipFile.Location = new System.Drawing.Point(476, 167);
            this.chkSkipFile.Name = "chkSkipFile";
            this.chkSkipFile.Size = new System.Drawing.Size(62, 21);
            this.chkSkipFile.TabIndex = 6;
            this.chkSkipFile.Text = "File";
            // 
            // chkSkipRegex
            // 
            this.chkSkipRegex.AutoSize = true;
            this.chkSkipRegex.ForeColor = System.Drawing.Color.White;
            this.chkSkipRegex.Location = new System.Drawing.Point(340, 168);
            this.chkSkipRegex.Name = "chkSkipRegex";
            this.chkSkipRegex.Size = new System.Drawing.Size(70, 21);
            this.chkSkipRegex.TabIndex = 7;
            this.chkSkipRegex.Text = "Regex";
            // 
            // lblSkipPattern
            // 
            this.lblSkipPattern.AutoSize = true;
            this.lblSkipPattern.ForeColor = System.Drawing.Color.White;
            this.lblSkipPattern.Location = new System.Drawing.Point(7, 210);
            this.lblSkipPattern.Name = "lblSkipPattern";
            this.lblSkipPattern.Size = new System.Drawing.Size(72, 17);
            this.lblSkipPattern.TabIndex = 8;
            this.lblSkipPattern.Text = "Pattern:";
            // 
            // txtSkipPattern
            // 
            this.txtSkipPattern.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.txtSkipPattern.ForeColor = System.Drawing.Color.White;
            this.txtSkipPattern.Location = new System.Drawing.Point(85, 203);
            this.txtSkipPattern.Name = "txtSkipPattern";
            this.txtSkipPattern.Size = new System.Drawing.Size(247, 24);
            this.txtSkipPattern.TabIndex = 9;
            // 
            // btnAddSkip
            // 
            this.btnAddSkip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.btnAddSkip.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnAddSkip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSkip.ForeColor = System.Drawing.Color.White;
            this.btnAddSkip.Location = new System.Drawing.Point(340, 202);
            this.btnAddSkip.Name = "btnAddSkip";
            this.btnAddSkip.Size = new System.Drawing.Size(50, 25);
            this.btnAddSkip.TabIndex = 10;
            this.btnAddSkip.Text = "Add";
            this.btnAddSkip.UseVisualStyleBackColor = false;
            this.btnAddSkip.Click += new System.EventHandler(this.btnAddSkip_Click);
            // 
            // btnRemoveSkip
            // 
            this.btnRemoveSkip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.btnRemoveSkip.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnRemoveSkip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveSkip.ForeColor = System.Drawing.Color.White;
            this.btnRemoveSkip.Location = new System.Drawing.Point(396, 202);
            this.btnRemoveSkip.Name = "btnRemoveSkip";
            this.btnRemoveSkip.Size = new System.Drawing.Size(76, 25);
            this.btnRemoveSkip.TabIndex = 11;
            this.btnRemoveSkip.Text = "Remove";
            this.btnRemoveSkip.UseVisualStyleBackColor = false;
            this.btnRemoveSkip.Click += new System.EventHandler(this.btnRemoveSkip_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(866, 826);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 32);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Create site";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(1012, 826);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Edit_cbftp_sites
            // 
            this.Edit_cbftp_sites.FormattingEnabled = true;
            this.Edit_cbftp_sites.Location = new System.Drawing.Point(396, 9);
            this.Edit_cbftp_sites.Name = "Edit_cbftp_sites";
            this.Edit_cbftp_sites.Size = new System.Drawing.Size(154, 25);
            this.Edit_cbftp_sites.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(302, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Edit Site:";
            // 
            // CbftpAddSiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1123, 868);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Edit_cbftp_sites);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.cbftpServerCombo);
            this.Controls.Add(this.lblServerStatus);
            this.Controls.Add(this.grpBasic);
            this.Controls.Add(this.grpTransfer);
            this.Controls.Add(this.grpLimits);
            this.Controls.Add(this.grpAllow);
            this.Controls.Add(this.grpProxy);
            this.Controls.Add(this.grpAffils);
            this.Controls.Add(this.grpSections);
            this.Controls.Add(this.grpSkiplist);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CbftpAddSiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Site to CBFTP";
            this.grpBasic.ResumeLayout(false);
            this.grpBasic.PerformLayout();
            this.grpLimits.ResumeLayout(false);
            this.grpLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxLogins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSimUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxSimDown)).EndInit();
            this.grpAllow.ResumeLayout(false);
            this.grpAllow.PerformLayout();
            this.grpProxy.ResumeLayout(false);
            this.grpProxy.PerformLayout();
            this.grpTransfer.ResumeLayout(false);
            this.grpTransfer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxIdleTime)).EndInit();
            this.grpAffils.ResumeLayout(false);
            this.grpAffils.PerformLayout();
            this.grpSections.ResumeLayout(false);
            this.grpSections.PerformLayout();
            this.grpSkiplist.ResumeLayout(false);
            this.grpSkiplist.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ComboBox Edit_cbftp_sites;
        private Label label1;
    }
}
