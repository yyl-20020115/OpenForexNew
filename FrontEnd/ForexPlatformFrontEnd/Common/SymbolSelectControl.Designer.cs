namespace ForexPlatformFrontEnd
{
    partial class SymbolSelectControl
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
            this.listViewSymbols = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.listViewPeriod = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.textBoxSymbol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.comboBoxDataProviderSources = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonNextSource = new System.Windows.Forms.Button();
            this.buttonPreviousSource = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // listViewSymbols
            // 
            this.listViewSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSymbols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewSymbols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.listViewSymbols.FullRowSelect = true;
            this.listViewSymbols.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSymbols.HideSelection = false;
            this.listViewSymbols.Location = new System.Drawing.Point(6, 30);
            this.listViewSymbols.Name = "listViewSymbols";
            this.listViewSymbols.Size = new System.Drawing.Size(460, 371);
            this.listViewSymbols.TabIndex = 34;
            this.listViewSymbols.UseCompatibleStateImageBehavior = false;
            this.listViewSymbols.View = System.Windows.Forms.View.Details;
            this.listViewSymbols.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewSymbols_MouseDoubleClick);
            this.listViewSymbols.SelectedIndexChanged += new System.EventHandler(this.listViewSymbols_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Group";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 373;
            // 
            // listViewPeriod
            // 
            this.listViewPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPeriod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewPeriod.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewPeriod.FullRowSelect = true;
            this.listViewPeriod.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPeriod.HideSelection = false;
            this.listViewPeriod.Location = new System.Drawing.Point(472, 30);
            this.listViewPeriod.Name = "listViewPeriod";
            this.listViewPeriod.Size = new System.Drawing.Size(121, 342);
            this.listViewPeriod.TabIndex = 33;
            this.listViewPeriod.UseCompatibleStateImageBehavior = false;
            this.listViewPeriod.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Period";
            this.columnHeader1.Width = 117;
            // 
            // textBoxSymbol
            // 
            this.textBoxSymbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxSymbol.Location = new System.Drawing.Point(44, 5);
            this.textBoxSymbol.Name = "textBoxSymbol";
            this.textBoxSymbol.Size = new System.Drawing.Size(138, 20);
            this.textBoxSymbol.TabIndex = 35;
            this.textBoxSymbol.WordWrap = false;
            this.textBoxSymbol.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSymbol_KeyUp);
            this.textBoxSymbol.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxSymbol_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Name";
            // 
            // buttonSelect
            // 
            this.buttonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSelect.Location = new System.Drawing.Point(472, 378);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(121, 23);
            this.buttonSelect.TabIndex = 37;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // comboBoxDataProviderSources
            // 
            this.comboBoxDataProviderSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDataProviderSources.DropDownHeight = 250;
            this.comboBoxDataProviderSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataProviderSources.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxDataProviderSources.FormattingEnabled = true;
            this.comboBoxDataProviderSources.IntegralHeight = false;
            this.comboBoxDataProviderSources.Location = new System.Drawing.Point(281, 4);
            this.comboBoxDataProviderSources.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxDataProviderSources.Name = "comboBoxDataProviderSources";
            this.comboBoxDataProviderSources.Size = new System.Drawing.Size(256, 21);
            this.comboBoxDataProviderSources.TabIndex = 38;
            this.comboBoxDataProviderSources.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataProviderSources_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Source";
            // 
            // buttonSearch
            // 
            this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSearch.Image = global::ForexPlatformFrontEnd.Properties.Resources.FIND;
            this.buttonSearch.Location = new System.Drawing.Point(188, 3);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(23, 23);
            this.buttonSearch.TabIndex = 35;
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonNextSource
            // 
            this.buttonNextSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonNextSource.Location = new System.Drawing.Point(569, 4);
            this.buttonNextSource.Name = "buttonNextSource";
            this.buttonNextSource.Size = new System.Drawing.Size(21, 21);
            this.buttonNextSource.TabIndex = 41;
            this.buttonNextSource.Text = ">";
            this.toolTip1.SetToolTip(this.buttonNextSource, "Next Souce");
            this.buttonNextSource.UseVisualStyleBackColor = true;
            this.buttonNextSource.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPreviousSource
            // 
            this.buttonPreviousSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPreviousSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPreviousSource.Location = new System.Drawing.Point(542, 4);
            this.buttonPreviousSource.Name = "buttonPreviousSource";
            this.buttonPreviousSource.Size = new System.Drawing.Size(21, 21);
            this.buttonPreviousSource.TabIndex = 42;
            this.buttonPreviousSource.Text = "<";
            this.toolTip1.SetToolTip(this.buttonPreviousSource, "Previous Source");
            this.buttonPreviousSource.UseVisualStyleBackColor = true;
            this.buttonPreviousSource.Click += new System.EventHandler(this.buttonPreviousSource_Click);
            // 
            // SymbolSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewPeriod);
            this.Controls.Add(this.buttonPreviousSource);
            this.Controls.Add(this.listViewSymbols);
            this.Controls.Add(this.buttonNextSource);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxDataProviderSources);
            this.Controls.Add(this.buttonSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSymbol);
            this.Name = "SymbolSelectControl";
            this.Size = new System.Drawing.Size(596, 404);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewSymbols;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView listViewPeriod;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TextBox textBoxSymbol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDataProviderSources;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonNextSource;
        private System.Windows.Forms.Button buttonPreviousSource;
        private System.Windows.Forms.ToolTip toolTip1;
        protected System.Windows.Forms.Button buttonSelect;
    }
}
