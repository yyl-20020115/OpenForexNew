using CommonSupport.UI;
namespace ForexPlatformFrontEnd
{
    partial class MarketWatchControl
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            this.listViewQuotes = new CommonSupport.UI.DoubleBufferListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripButton();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.chartTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // listViewQuotes
            // 
            this.listViewQuotes.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listViewQuotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewQuotes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewQuotes.ContextMenuStrip = this.contextMenuStrip;
            this.listViewQuotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewQuotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewQuotes.FullRowSelect = true;
            this.listViewQuotes.GridLines = true;
            this.listViewQuotes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewQuotes.Location = new System.Drawing.Point(0, 25);
            this.listViewQuotes.Name = "listViewQuotes";
            this.listViewQuotes.Size = new System.Drawing.Size(800, 575);
            this.listViewQuotes.TabIndex = 0;
            this.listViewQuotes.UseCompatibleStateImageBehavior = false;
            this.listViewQuotes.View = System.Windows.Forms.View.Details;
            this.listViewQuotes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewQuotes_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Symbol";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Ask";
            this.columnHeader2.Width = 75;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Bid";
            this.columnHeader3.Width = 75;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Spread";
            this.columnHeader4.Width = 75;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "High";
            this.columnHeader5.Width = 75;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Low";
            this.columnHeader6.Width = 75;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            toolStripSeparator1,
            this.tradeToolStripMenuItem,
            this.viewChartToolStripMenuItem,
            this.chartTradeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(154, 142);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addToolStripMenuItem.Text = "Add...";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // tradeToolStripMenuItem
            // 
            this.tradeToolStripMenuItem.Enabled = false;
            this.tradeToolStripMenuItem.Name = "tradeToolStripMenuItem";
            this.tradeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tradeToolStripMenuItem.Text = "Trade";
            this.tradeToolStripMenuItem.Visible = false;
            this.tradeToolStripMenuItem.Click += new System.EventHandler(this.tradeToolStripMenuItem_Click);
            // 
            // viewChartToolStripMenuItem
            // 
            this.viewChartToolStripMenuItem.Name = "viewChartToolStripMenuItem";
            this.viewChartToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.viewChartToolStripMenuItem.Text = "Chart";
            this.viewChartToolStripMenuItem.Click += new System.EventHandler(this.viewChartToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAdd,
            this.toolStripButtonRemove});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.Image = global::ForexPlatformFrontEnd.Properties.Resources.ADD2;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(46, 22);
            this.toolStripButtonAdd.Text = "Add";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // toolStripButtonRemove
            // 
            this.toolStripButtonRemove.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonRemove.Image = global::ForexPlatformFrontEnd.Properties.Resources.DELETE;
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButtonRemove";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(66, 22);
            this.toolStripButtonRemove.Text = "Remove";
            this.toolStripButtonRemove.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Interval = 500;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // chartTradeToolStripMenuItem
            // 
            this.chartTradeToolStripMenuItem.Name = "chartTradeToolStripMenuItem";
            this.chartTradeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.chartTradeToolStripMenuItem.Text = "Chart && Trade";
            this.chartTradeToolStripMenuItem.Click += new System.EventHandler(this.chartTradeToolStripMenuItem_Click);
            // 
            // MarketWatchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewQuotes);
            this.Controls.Add(this.toolStrip1);
            this.ImageName = "briefcase2view.png";
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MarketWatchControl";
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private DoubleBufferListView listViewQuotes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemove;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ToolStripMenuItem tradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartTradeToolStripMenuItem;


    }
}
