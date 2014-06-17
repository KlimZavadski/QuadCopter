namespace RemoteControlSystem
{
    partial class RemoteControlSystem
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
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.trackBar_Speed = new System.Windows.Forms.TrackBar();
			this.trackBar_DriverSpeed1 = new System.Windows.Forms.TrackBar();
			this.trackBar_DriverSpeed4 = new System.Windows.Forms.TrackBar();
			this.trackBar_DriverSpeed3 = new System.Windows.Forms.TrackBar();
			this.trackBar_DriverSpeed2 = new System.Windows.Forms.TrackBar();
			this.textBox_Telemetry = new System.Windows.Forms.TextBox();
			this.zedGraphControl = new ZedGraph.ZedGraphControl();
			this.statusBar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed2)).BeginInit();
			this.SuspendLayout();
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
			this.statusBar.Location = new System.Drawing.Point(0, 390);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(502, 22);
			this.statusBar.TabIndex = 0;
			this.statusBar.Text = "statusStrip1";
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(86, 17);
			this.toolStripStatusLabel.Text = "Not connected";
			// 
			// trackBar_Speed
			// 
			this.trackBar_Speed.Enabled = false;
			this.trackBar_Speed.LargeChange = 1;
			this.trackBar_Speed.Location = new System.Drawing.Point(13, 13);
			this.trackBar_Speed.Maximum = 150;
			this.trackBar_Speed.Minimum = 70;
			this.trackBar_Speed.Name = "trackBar_Speed";
			this.trackBar_Speed.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_Speed.Size = new System.Drawing.Size(45, 200);
			this.trackBar_Speed.TabIndex = 1;
			this.trackBar_Speed.TickFrequency = 10;
			this.trackBar_Speed.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar_Speed.Value = 70;
			// 
			// trackBar_DriverSpeed1
			// 
			this.trackBar_DriverSpeed1.Enabled = false;
			this.trackBar_DriverSpeed1.LargeChange = 1;
			this.trackBar_DriverSpeed1.Location = new System.Drawing.Point(109, 7);
			this.trackBar_DriverSpeed1.Maximum = 150;
			this.trackBar_DriverSpeed1.Minimum = 70;
			this.trackBar_DriverSpeed1.Name = "trackBar_DriverSpeed1";
			this.trackBar_DriverSpeed1.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_DriverSpeed1.Size = new System.Drawing.Size(45, 100);
			this.trackBar_DriverSpeed1.TabIndex = 2;
			this.trackBar_DriverSpeed1.TickFrequency = 10;
			this.trackBar_DriverSpeed1.Value = 70;
			// 
			// trackBar_DriverSpeed4
			// 
			this.trackBar_DriverSpeed4.Enabled = false;
			this.trackBar_DriverSpeed4.LargeChange = 1;
			this.trackBar_DriverSpeed4.Location = new System.Drawing.Point(109, 113);
			this.trackBar_DriverSpeed4.Maximum = 150;
			this.trackBar_DriverSpeed4.Minimum = 70;
			this.trackBar_DriverSpeed4.Name = "trackBar_DriverSpeed4";
			this.trackBar_DriverSpeed4.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_DriverSpeed4.Size = new System.Drawing.Size(45, 100);
			this.trackBar_DriverSpeed4.TabIndex = 3;
			this.trackBar_DriverSpeed4.TickFrequency = 10;
			this.trackBar_DriverSpeed4.Value = 70;
			// 
			// trackBar_DriverSpeed3
			// 
			this.trackBar_DriverSpeed3.Enabled = false;
			this.trackBar_DriverSpeed3.LargeChange = 1;
			this.trackBar_DriverSpeed3.Location = new System.Drawing.Point(239, 113);
			this.trackBar_DriverSpeed3.Maximum = 150;
			this.trackBar_DriverSpeed3.Minimum = 70;
			this.trackBar_DriverSpeed3.Name = "trackBar_DriverSpeed3";
			this.trackBar_DriverSpeed3.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_DriverSpeed3.Size = new System.Drawing.Size(45, 100);
			this.trackBar_DriverSpeed3.TabIndex = 5;
			this.trackBar_DriverSpeed3.TickFrequency = 10;
			this.trackBar_DriverSpeed3.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trackBar_DriverSpeed3.Value = 70;
			// 
			// trackBar_DriverSpeed2
			// 
			this.trackBar_DriverSpeed2.Enabled = false;
			this.trackBar_DriverSpeed2.LargeChange = 1;
			this.trackBar_DriverSpeed2.Location = new System.Drawing.Point(239, 7);
			this.trackBar_DriverSpeed2.Maximum = 150;
			this.trackBar_DriverSpeed2.Minimum = 70;
			this.trackBar_DriverSpeed2.Name = "trackBar_DriverSpeed2";
			this.trackBar_DriverSpeed2.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar_DriverSpeed2.Size = new System.Drawing.Size(45, 100);
			this.trackBar_DriverSpeed2.TabIndex = 4;
			this.trackBar_DriverSpeed2.TickFrequency = 10;
			this.trackBar_DriverSpeed2.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trackBar_DriverSpeed2.Value = 70;
			// 
			// textBox_Telemetry
			// 
			this.textBox_Telemetry.Location = new System.Drawing.Point(290, 13);
			this.textBox_Telemetry.Multiline = true;
			this.textBox_Telemetry.Name = "textBox_Telemetry";
			this.textBox_Telemetry.Size = new System.Drawing.Size(200, 200);
			this.textBox_Telemetry.TabIndex = 6;
			// 
			// zedGraphControl
			// 
			this.zedGraphControl.Location = new System.Drawing.Point(13, 219);
			this.zedGraphControl.Name = "zedGraphControl";
			this.zedGraphControl.ScrollGrace = 0D;
			this.zedGraphControl.ScrollMaxX = 0D;
			this.zedGraphControl.ScrollMaxY = 0D;
			this.zedGraphControl.ScrollMaxY2 = 0D;
			this.zedGraphControl.ScrollMinX = 0D;
			this.zedGraphControl.ScrollMinY = 0D;
			this.zedGraphControl.ScrollMinY2 = 0D;
			this.zedGraphControl.Size = new System.Drawing.Size(201, 167);
			this.zedGraphControl.TabIndex = 7;
			// 
			// RemoteControlSystem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 412);
			this.Controls.Add(this.zedGraphControl);
			this.Controls.Add(this.textBox_Telemetry);
			this.Controls.Add(this.trackBar_DriverSpeed3);
			this.Controls.Add(this.trackBar_DriverSpeed2);
			this.Controls.Add(this.trackBar_DriverSpeed4);
			this.Controls.Add(this.trackBar_DriverSpeed1);
			this.Controls.Add(this.trackBar_Speed);
			this.Controls.Add(this.statusBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RemoteControlSystem";
			this.Text = "Remote Control System v1.0";
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar_DriverSpeed2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.TrackBar trackBar_Speed;
		private System.Windows.Forms.TrackBar trackBar_DriverSpeed1;
		private System.Windows.Forms.TrackBar trackBar_DriverSpeed4;
		private System.Windows.Forms.TrackBar trackBar_DriverSpeed3;
		private System.Windows.Forms.TrackBar trackBar_DriverSpeed2;
		private System.Windows.Forms.TextBox textBox_Telemetry;
		private ZedGraph.ZedGraphControl zedGraphControl;
    }
}

