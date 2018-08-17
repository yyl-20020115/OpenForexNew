namespace CommonSupport
{
    partial class DragControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required baseMethod for Designer support - do not modify 
        /// the contents of this baseMethod with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dragStrip = new CommonSupport.DragStripControl();
            this.SuspendLayout();
            // 
            // dragStrip
            // 
            this.dragStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.dragStrip.Location = new System.Drawing.Point(0, 0);
            this.dragStrip.Name = "dragStrip";
            this.dragStrip.Size = new System.Drawing.Size(263, 18);
            this.dragStrip.TabIndex = 1;
            // 
            // DraggableControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.dragStrip);
            this.Name = "DraggableControl";
            this.Size = new System.Drawing.Size(263, 194);
            this.ResumeLayout(false);

        }

        #endregion

        public DragStripControl dragStrip;
    }
}
