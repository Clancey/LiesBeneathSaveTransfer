namespace LiesBeneathSaveTransfer
{
	partial class Form1
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.loadingPanel = new System.Windows.Forms.Panel();
			this.loadingProgressBar = new System.Windows.Forms.ProgressBar();
			this.loadingLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.deviceComboBox = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.backupButton = new System.Windows.Forms.Button();
			this.toDesktopButton = new System.Windows.Forms.Button();
			this.syncToQuestButton = new System.Windows.Forms.Button();
			this.cancelBackupButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1.SuspendLayout();
			this.loadingPanel.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.loadingPanel);
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
			this.flowLayoutPanel1.Controls.Add(this.backupButton);
			this.flowLayoutPanel1.Controls.Add(this.cancelBackupButton);
			this.flowLayoutPanel1.Controls.Add(this.toDesktopButton);
			this.flowLayoutPanel1.Controls.Add(this.syncToQuestButton);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, -2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(798, 453);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// loadingPanel
			// 
			this.loadingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loadingPanel.AutoSize = true;
			this.loadingPanel.Controls.Add(this.loadingProgressBar);
			this.loadingPanel.Controls.Add(this.loadingLabel);
			this.loadingPanel.Location = new System.Drawing.Point(3, 3);
			this.loadingPanel.Name = "loadingPanel";
			this.loadingPanel.Size = new System.Drawing.Size(785, 74);
			this.loadingPanel.TabIndex = 0;
			// 
			// loadingProgressBar
			// 
			this.loadingProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loadingProgressBar.Location = new System.Drawing.Point(7, 42);
			this.loadingProgressBar.Name = "loadingProgressBar";
			this.loadingProgressBar.Size = new System.Drawing.Size(769, 29);
			this.loadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.loadingProgressBar.TabIndex = 0;
			this.loadingProgressBar.Value = 1;
			// 
			// loadingLabel
			// 
			this.loadingLabel.AutoSize = true;
			this.loadingLabel.Location = new System.Drawing.Point(9, 8);
			this.loadingLabel.Name = "loadingLabel";
			this.loadingLabel.Size = new System.Drawing.Size(66, 20);
			this.loadingLabel.TabIndex = 0;
			this.loadingLabel.Text = "Loading";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Device";
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Controls.Add(this.deviceComboBox);
			this.flowLayoutPanel2.Controls.Add(this.button1);
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 103);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(785, 54);
			this.flowLayoutPanel2.TabIndex = 3;
			// 
			// deviceComboBox
			// 
			this.deviceComboBox.FormattingEnabled = true;
			this.deviceComboBox.Location = new System.Drawing.Point(3, 3);
			this.deviceComboBox.Name = "deviceComboBox";
			this.deviceComboBox.Size = new System.Drawing.Size(655, 28);
			this.deviceComboBox.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.AutoSize = true;
			this.button1.Location = new System.Drawing.Point(664, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(100, 37);
			this.button1.TabIndex = 3;
			this.button1.Text = "Refresh";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// backupButton
			// 
			this.backupButton.Location = new System.Drawing.Point(3, 163);
			this.backupButton.Name = "backupButton";
			this.backupButton.Size = new System.Drawing.Size(75, 40);
			this.backupButton.TabIndex = 4;
			this.backupButton.Text = "Backup";
			this.backupButton.UseVisualStyleBackColor = true;
			this.backupButton.Click += new System.EventHandler(this.backupButton_Click);
			// 
			// toDesktopButton
			// 
			this.toDesktopButton.Location = new System.Drawing.Point(3, 249);
			this.toDesktopButton.Name = "toDesktopButton";
			this.toDesktopButton.Size = new System.Drawing.Size(238, 41);
			this.toDesktopButton.TabIndex = 5;
			this.toDesktopButton.Text = "Sync to Windows";
			this.toDesktopButton.UseVisualStyleBackColor = true;
			this.toDesktopButton.Click += new System.EventHandler(this.toDesktopButton_Click);
			// 
			// syncToQuestButton
			// 
			this.syncToQuestButton.Enabled = false;
			this.syncToQuestButton.Location = new System.Drawing.Point(3, 296);
			this.syncToQuestButton.Name = "syncToQuestButton";
			this.syncToQuestButton.Size = new System.Drawing.Size(238, 36);
			this.syncToQuestButton.TabIndex = 6;
			this.syncToQuestButton.Text = "Sync to Oculus Quest";
			this.syncToQuestButton.UseVisualStyleBackColor = true;
			this.syncToQuestButton.Click += new System.EventHandler(this.syncToQuestButton_Click);
			// 
			// cancelBackupButton
			// 
			this.cancelBackupButton.Enabled = false;
			this.cancelBackupButton.Location = new System.Drawing.Point(3, 209);
			this.cancelBackupButton.Name = "cancelBackupButton";
			this.cancelBackupButton.Size = new System.Drawing.Size(75, 34);
			this.cancelBackupButton.TabIndex = 7;
			this.cancelBackupButton.Text = "Cancel Backup";
			this.cancelBackupButton.UseVisualStyleBackColor = true;
			this.cancelBackupButton.Click += new System.EventHandler(this.cancelBackupButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(801, 450);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "Form1";
			this.Text = "Lies Beneath Save Manager";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.loadingPanel.ResumeLayout(false);
			this.loadingPanel.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel loadingPanel;
		private System.Windows.Forms.Label loadingLabel;
		private System.Windows.Forms.ProgressBar loadingProgressBar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox deviceComboBox;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button backupButton;
		private System.Windows.Forms.Button toDesktopButton;
		private System.Windows.Forms.Button syncToQuestButton;
		private System.Windows.Forms.Button cancelBackupButton;
	}
}

