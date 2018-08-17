namespace ForexPlatformFrontEnd
{
    partial class PriceHelpControl
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
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "10",
            "10"}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "10",
            "10"}, -1);
            this.label8 = new System.Windows.Forms.Label();
            this.buttonX = new System.Windows.Forms.Button();
            this.listViewPointsAbove = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.listViewPointsBelow = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Value";
            // 
            // buttonX
            // 
            this.buttonX.BackColor = System.Drawing.SystemColors.Control;
            this.buttonX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonX.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonX.Location = new System.Drawing.Point(108, 3);
            this.buttonX.Margin = new System.Windows.Forms.Padding(2);
            this.buttonX.Name = "buttonX";
            this.buttonX.Size = new System.Drawing.Size(16, 16);
            this.buttonX.TabIndex = 39;
            this.buttonX.TabStop = false;
            this.buttonX.Text = ".";
            this.buttonX.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonX.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonX.UseVisualStyleBackColor = false;
            this.buttonX.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // listViewPointsAbove
            // 
            this.listViewPointsAbove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPointsAbove.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewPointsAbove.FullRowSelect = true;
            this.listViewPointsAbove.GridLines = true;
            this.listViewPointsAbove.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPointsAbove.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem11});
            this.listViewPointsAbove.Location = new System.Drawing.Point(2, 21);
            this.listViewPointsAbove.Name = "listViewPointsAbove";
            this.listViewPointsAbove.Scrollable = false;
            this.listViewPointsAbove.Size = new System.Drawing.Size(60, 186);
            this.listViewPointsAbove.TabIndex = 41;
            this.listViewPointsAbove.UseCompatibleStateImageBehavior = false;
            this.listViewPointsAbove.View = System.Windows.Forms.View.Details;
            this.listViewPointsAbove.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewPointsAbove_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Above";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // listViewPointsBelow
            // 
            this.listViewPointsBelow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPointsBelow.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listViewPointsBelow.FullRowSelect = true;
            this.listViewPointsBelow.GridLines = true;
            this.listViewPointsBelow.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPointsBelow.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem12});
            this.listViewPointsBelow.Location = new System.Drawing.Point(64, 21);
            this.listViewPointsBelow.Name = "listViewPointsBelow";
            this.listViewPointsBelow.Scrollable = false;
            this.listViewPointsBelow.Size = new System.Drawing.Size(60, 186);
            this.listViewPointsBelow.TabIndex = 42;
            this.listViewPointsBelow.UseCompatibleStateImageBehavior = false;
            this.listViewPointsBelow.View = System.Windows.Forms.View.Details;
            this.listViewPointsBelow.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewPointsBelow_MouseDoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 0;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Below";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PriceHelpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.listViewPointsBelow);
            this.Controls.Add(this.listViewPointsAbove);
            this.Controls.Add(this.buttonX);
            this.Controls.Add(this.label8);
            this.Name = "PriceHelpControl";
            this.Size = new System.Drawing.Size(128, 210);
            this.Load += new System.EventHandler(this.PriceHelpControl_Load);
            this.Leave += new System.EventHandler(this.PriceHelpControl_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonX;
        private System.Windows.Forms.ListView listViewPointsAbove;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listViewPointsBelow;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}
