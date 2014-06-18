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
            this.zedGraphControl_Speed = new ZedGraph.ZedGraphControl();
            this.textBox_telemetria = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // zedGraphControl_Speed
            // 
            this.zedGraphControl_Speed.IsAutoScrollRange = false;
            this.zedGraphControl_Speed.IsEnableHPan = true;
            this.zedGraphControl_Speed.IsEnableHZoom = true;
            this.zedGraphControl_Speed.IsEnableVPan = true;
            this.zedGraphControl_Speed.IsEnableVZoom = true;
            this.zedGraphControl_Speed.IsScrollY2 = false;
            this.zedGraphControl_Speed.IsShowContextMenu = true;
            this.zedGraphControl_Speed.IsShowCursorValues = false;
            this.zedGraphControl_Speed.IsShowHScrollBar = false;
            this.zedGraphControl_Speed.IsShowPointValues = false;
            this.zedGraphControl_Speed.IsShowVScrollBar = false;
            this.zedGraphControl_Speed.IsZoomOnMouseCenter = false;
            this.zedGraphControl_Speed.Location = new System.Drawing.Point(12, 12);
            this.zedGraphControl_Speed.Name = "zedGraphControl_Speed";
            this.zedGraphControl_Speed.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl_Speed.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.zedGraphControl_Speed.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl_Speed.PointDateFormat = "g";
            this.zedGraphControl_Speed.PointValueFormat = "G";
            this.zedGraphControl_Speed.ScrollMaxX = 0D;
            this.zedGraphControl_Speed.ScrollMaxY = 0D;
            this.zedGraphControl_Speed.ScrollMaxY2 = 0D;
            this.zedGraphControl_Speed.ScrollMinX = 0D;
            this.zedGraphControl_Speed.ScrollMinY = 0D;
            this.zedGraphControl_Speed.ScrollMinY2 = 0D;
            this.zedGraphControl_Speed.Size = new System.Drawing.Size(479, 389);
            this.zedGraphControl_Speed.TabIndex = 0;
            this.zedGraphControl_Speed.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl_Speed.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.zedGraphControl_Speed.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.zedGraphControl_Speed.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl_Speed.ZoomStepFraction = 0.1D;
            // 
            // textBox_telemetria
            // 
            this.textBox_telemetria.Location = new System.Drawing.Point(3, 407);
            this.textBox_telemetria.Multiline = true;
            this.textBox_telemetria.Name = "textBox_telemetria";
            this.textBox_telemetria.Size = new System.Drawing.Size(1368, 557);
            this.textBox_telemetria.TabIndex = 1;
            // 
            // RemoteControlSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1383, 976);
            this.Controls.Add(this.textBox_telemetria);
            this.Controls.Add(this.zedGraphControl_Speed);
            this.Name = "RemoteControlSystem";
            this.Text = "RemoteControlSystem";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl_Speed;
        private System.Windows.Forms.TextBox textBox_telemetria;
    }
}

