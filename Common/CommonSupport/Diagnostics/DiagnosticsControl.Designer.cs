namespace CommonSupport
{
    partial class DiagnosticsControl
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
            this.listViewVariables = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewVariables
            // 
            this.listViewVariables.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewVariables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewVariables.FullRowSelect = true;
            this.listViewVariables.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewVariables.Location = new System.Drawing.Point(0, 0);
            this.listViewVariables.Name = "listViewVariables";
            this.listViewVariables.Size = new System.Drawing.Size(330, 314);
            this.listViewVariables.TabIndex = 0;
            this.listViewVariables.UseCompatibleStateImageBehavior = false;
            this.listViewVariables.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 55;
            // 
            // DiagnosticsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewVariables);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(0, 24);
            this.Name = "DiagnosticsControl";
            this.Size = new System.Drawing.Size(330, 314);
            this.SizeChanged += new System.EventHandler(this.ApplicationDiagnosticsInformationControl_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewVariables;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;


    }
}
