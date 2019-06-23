namespace ImageViewer1
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.placeValueHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gIFPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeDelayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.putValueHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sepToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1134, 580);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.PictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.PictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.zoomToolStripMenuItem,
            this.gIFPropertyToolStripMenuItem,
            this.sepToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(271, 144);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(270, 24);
            this.openToolStripMenuItem.Text = "Open (O)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(267, 6);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeValueHereToolStripMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(270, 24);
            this.zoomToolStripMenuItem.Text = "Zoom";
            // 
            // placeValueHereToolStripMenuItem
            // 
            this.placeValueHereToolStripMenuItem.Name = "placeValueHereToolStripMenuItem";
            this.placeValueHereToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.placeValueHereToolStripMenuItem.Text = "Place value here (+/-)";
            // 
            // gIFPropertyToolStripMenuItem
            // 
            this.gIFPropertyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startStopToolStripMenuItem,
            this.nextFrameToolStripMenuItem,
            this.previousFrameToolStripMenuItem,
            this.timeDelayToolStripMenuItem});
            this.gIFPropertyToolStripMenuItem.Enabled = false;
            this.gIFPropertyToolStripMenuItem.Name = "gIFPropertyToolStripMenuItem";
            this.gIFPropertyToolStripMenuItem.Size = new System.Drawing.Size(270, 24);
            this.gIFPropertyToolStripMenuItem.Text = "GIF Property";
            // 
            // startStopToolStripMenuItem
            // 
            this.startStopToolStripMenuItem.Name = "startStopToolStripMenuItem";
            this.startStopToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.startStopToolStripMenuItem.Text = "Start / Stop";
            // 
            // nextFrameToolStripMenuItem
            // 
            this.nextFrameToolStripMenuItem.Name = "nextFrameToolStripMenuItem";
            this.nextFrameToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.nextFrameToolStripMenuItem.Text = "Next Frame";
            // 
            // previousFrameToolStripMenuItem
            // 
            this.previousFrameToolStripMenuItem.Name = "previousFrameToolStripMenuItem";
            this.previousFrameToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.previousFrameToolStripMenuItem.Text = "Previous Frame";
            // 
            // timeDelayToolStripMenuItem
            // 
            this.timeDelayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.putValueHereToolStripMenuItem});
            this.timeDelayToolStripMenuItem.Name = "timeDelayToolStripMenuItem";
            this.timeDelayToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.timeDelayToolStripMenuItem.Text = "Time Delay";
            // 
            // putValueHereToolStripMenuItem
            // 
            this.putValueHereToolStripMenuItem.Name = "putValueHereToolStripMenuItem";
            this.putValueHereToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.putValueHereToolStripMenuItem.Text = "Put Value Here";
            // 
            // sepToolStripMenuItem
            // 
            this.sepToolStripMenuItem.Items.AddRange(new object[] {
            "What",
            "Is",
            "This"});
            this.sepToolStripMenuItem.Name = "sepToolStripMenuItem";
            this.sepToolStripMenuItem.Size = new System.Drawing.Size(210, 28);
            this.sepToolStripMenuItem.Text = "Sep";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(267, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(270, 24);
            this.deleteToolStripMenuItem.Text = "Delete (Del)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 580);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress_1);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem placeValueHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gIFPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeDelayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem putValueHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox sepToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

