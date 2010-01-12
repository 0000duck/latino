namespace BDocVisualizer
{
    partial class MainWnd
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
            this.drawableObjectViewer = new Latino.Visualization.DrawableObjectViewer();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.splitter = new System.Windows.Forms.Splitter();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // drawableObjectViewer
            // 
            this.drawableObjectViewer.CanvasCursor = System.Windows.Forms.Cursors.Cross;
            this.drawableObjectViewer.CanvasSize = new System.Drawing.Size(1024, 768);
            this.drawableObjectViewer.Cursor = System.Windows.Forms.Cursors.Cross;
            this.drawableObjectViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawableObjectViewer.Location = new System.Drawing.Point(0, 0);
            this.drawableObjectViewer.Name = "drawableObjectViewer";
            this.drawableObjectViewer.Size = new System.Drawing.Size(831, 619);
            this.drawableObjectViewer.TabIndex = 0;
            this.drawableObjectViewer.DrawableObjectClick += new Latino.Visualization.DrawableObjectViewer.DrawableObjectEventHandler(this.drawableObjectViewer1_DrawableObjectTargetChanged);
            // 
            // pnlInfo
            // 
            this.pnlInfo.Controls.Add(this.txtInfo);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlInfo.Location = new System.Drawing.Point(836, 0);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(298, 619);
            this.pnlInfo.TabIndex = 1;
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Location = new System.Drawing.Point(0, 0);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo.Size = new System.Drawing.Size(298, 619);
            this.txtInfo.TabIndex = 0;
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter.Location = new System.Drawing.Point(831, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(5, 619);
            this.splitter.TabIndex = 2;
            this.splitter.TabStop = false;
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 619);
            this.Controls.Add(this.drawableObjectViewer);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.pnlInfo);
            this.Name = "MainWnd";
            this.Text = "MainWnd";
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Latino.Visualization.DrawableObjectViewer drawableObjectViewer;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Splitter splitter;
    }
}