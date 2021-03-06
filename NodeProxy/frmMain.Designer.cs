﻿namespace NodeProxy
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.MyNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuDomain = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShowApp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseApp = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnProxyAddr = new System.Windows.Forms.Button();
            this.btnNodeBrowse = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Domain = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProjectDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.txtNodeLog = new System.Windows.Forms.RichTextBox();
            this.ckAutoProxy = new System.Windows.Forms.CheckBox();
            this.lblNodePath = new System.Windows.Forms.Label();
            this.lblProxyAddr = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLoadLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MyNotify
            // 
            this.MyNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("MyNotify.Icon")));
            this.MyNotify.Text = "MyNotify";
            this.MyNotify.Visible = true;
            this.MyNotify.DoubleClick += new System.EventHandler(this.MyNotify_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDomain,
            this.mnuDisconnect,
            this.toolStripSeparator1,
            this.mnuShowApp,
            this.mnuCloseApp});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 98);
            // 
            // mnuDomain
            // 
            this.mnuDomain.Name = "mnuDomain";
            this.mnuDomain.Size = new System.Drawing.Size(172, 22);
            this.mnuDomain.Text = "www.domain.com";
            this.mnuDomain.Click += new System.EventHandler(this.mnuDomain_Click);
            // 
            // mnuDisconnect
            // 
            this.mnuDisconnect.Name = "mnuDisconnect";
            this.mnuDisconnect.Size = new System.Drawing.Size(172, 22);
            this.mnuDisconnect.Text = "Disconnect";
            this.mnuDisconnect.Click += new System.EventHandler(this.mnuDisconnect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // mnuShowApp
            // 
            this.mnuShowApp.Name = "mnuShowApp";
            this.mnuShowApp.Size = new System.Drawing.Size(172, 22);
            this.mnuShowApp.Text = "Configure";
            this.mnuShowApp.Click += new System.EventHandler(this.mnuShowApp_Click);
            // 
            // mnuCloseApp
            // 
            this.mnuCloseApp.Name = "mnuCloseApp";
            this.mnuCloseApp.Size = new System.Drawing.Size(172, 22);
            this.mnuCloseApp.Text = "Exit";
            this.mnuCloseApp.Click += new System.EventHandler(this.mnuCloseApp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Proxy Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Node JS Exec";
            // 
            // btnProxyAddr
            // 
            this.btnProxyAddr.Location = new System.Drawing.Point(384, 50);
            this.btnProxyAddr.Name = "btnProxyAddr";
            this.btnProxyAddr.Size = new System.Drawing.Size(69, 23);
            this.btnProxyAddr.TabIndex = 5;
            this.btnProxyAddr.Text = "Select";
            this.btnProxyAddr.UseVisualStyleBackColor = true;
            this.btnProxyAddr.Click += new System.EventHandler(this.btnProxyAddr_Click);
            // 
            // btnNodeBrowse
            // 
            this.btnNodeBrowse.Location = new System.Drawing.Point(384, 16);
            this.btnNodeBrowse.Name = "btnNodeBrowse";
            this.btnNodeBrowse.Size = new System.Drawing.Size(69, 23);
            this.btnNodeBrowse.TabIndex = 6;
            this.btnNodeBrowse.Text = "Select";
            this.btnNodeBrowse.UseVisualStyleBackColor = true;
            this.btnNodeBrowse.Click += new System.EventHandler(this.btnNodeBrowse_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Domain,
            this.ProjectDirectory});
            this.listView1.Location = new System.Drawing.Point(13, 63);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(464, 114);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Domain
            // 
            this.Domain.Text = "Domain";
            this.Domain.Width = 125;
            // 
            // ProjectDirectory
            // 
            this.ProjectDirectory.Text = "ProjectDirectory";
            this.ProjectDirectory.Width = 300;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Existing Projects";
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(13, 183);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnAddNew.TabIndex = 10;
            this.btnAddNew.Text = "Add New";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // txtNodeLog
            // 
            this.txtNodeLog.Location = new System.Drawing.Point(13, 371);
            this.txtNodeLog.Name = "txtNodeLog";
            this.txtNodeLog.Size = new System.Drawing.Size(458, 151);
            this.txtNodeLog.TabIndex = 11;
            this.txtNodeLog.Text = "";
            // 
            // ckAutoProxy
            // 
            this.ckAutoProxy.AutoSize = true;
            this.ckAutoProxy.Location = new System.Drawing.Point(16, 74);
            this.ckAutoProxy.Name = "ckAutoProxy";
            this.ckAutoProxy.Size = new System.Drawing.Size(194, 17);
            this.ckAutoProxy.TabIndex = 13;
            this.ckAutoProxy.Text = "Auto Enable/Disable Browser Proxy";
            this.ckAutoProxy.UseVisualStyleBackColor = true;
            this.ckAutoProxy.CheckedChanged += new System.EventHandler(this.ckAutoProxy_CheckedChanged);
            // 
            // lblNodePath
            // 
            this.lblNodePath.AutoSize = true;
            this.lblNodePath.Location = new System.Drawing.Point(94, 16);
            this.lblNodePath.Name = "lblNodePath";
            this.lblNodePath.Size = new System.Drawing.Size(0, 13);
            this.lblNodePath.TabIndex = 14;
            // 
            // lblProxyAddr
            // 
            this.lblProxyAddr.AutoSize = true;
            this.lblProxyAddr.Location = new System.Drawing.Point(101, 50);
            this.lblProxyAddr.Name = "lblProxyAddr";
            this.lblProxyAddr.Size = new System.Drawing.Size(79, 13);
            this.lblProxyAddr.TabIndex = 15;
            this.lblProxyAddr.Text = "127.0.0.1:8080";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 355);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Proxy Logs";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ckAutoProxy);
            this.groupBox1.Controls.Add(this.lblProxyAddr);
            this.groupBox1.Controls.Add(this.lblNodePath);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnNodeBrowse);
            this.groupBox1.Controls.Add(this.btnProxyAddr);
            this.groupBox1.Location = new System.Drawing.Point(12, 237);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 97);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "System Configuration";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnLoadLog
            // 
            this.btnLoadLog.Location = new System.Drawing.Point(19, 528);
            this.btnLoadLog.Name = "btnLoadLog";
            this.btnLoadLog.Size = new System.Drawing.Size(69, 25);
            this.btnLoadLog.TabIndex = 18;
            this.btnLoadLog.Text = "Load Log";
            this.btnLoadLog.UseVisualStyleBackColor = true;
            this.btnLoadLog.Click += new System.EventHandler(this.btnLoadLog_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(94, 528);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(69, 25);
            this.btnClearLog.TabIndex = 19;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(488, 24);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout,
            this.mnuHelpIndex});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.mnuHelpAbout.Text = "About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnuHelpIndex
            // 
            this.mnuHelpIndex.Name = "mnuHelpIndex";
            this.mnuHelpIndex.Size = new System.Drawing.Size(107, 22);
            this.mnuHelpIndex.Text = "Index";
            this.mnuHelpIndex.Click += new System.EventHandler(this.mnuHelpIndex_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(94, 183);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 21;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(405, 183);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(66, 23);
            this.startBtn.TabIndex = 22;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 555);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnLoadLog);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtNodeLog);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon MyNotify;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuDomain;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseApp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuShowApp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnProxyAddr;
        private System.Windows.Forms.Button btnNodeBrowse;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader Domain;
        private System.Windows.Forms.ColumnHeader ProjectDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.RichTextBox txtNodeLog;
        private System.Windows.Forms.CheckBox ckAutoProxy;
        private System.Windows.Forms.ToolStripMenuItem mnuDisconnect;
        private System.Windows.Forms.Label lblNodePath;
        private System.Windows.Forms.Label lblProxyAddr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLoadLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpIndex;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button startBtn;
    }
}

