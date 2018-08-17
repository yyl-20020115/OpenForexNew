using CommonSupport;
namespace ForexPlatformFrontEnd
{
    partial class ManagedExpertControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagedExpertControl));
            this.splitter2 = new CommonSupport.SplitterEx();
            this.splitter1 = new CommonSupport.SplitterEx();
            this.tracerControl = new CommonSupport.TracerControl();
            this.toolStripAdditional = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonInitialize = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonChart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
            this.expertSessionControl = new ForexPlatformFrontEnd.PlatformExpertSessionControl();
            this.ordersControlExpertSession = new ForexPlatformFrontEnd.OrdersControl();
            this.accountControl1 = new ForexPlatformFrontEnd.AccountControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.positionsControl1 = new ForexPlatformFrontEnd.PositionsControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.toolStripAdditional.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 325);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(800, 8);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            this.splitter2.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 569);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(800, 8);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            this.splitter1.Visible = false;
            // 
            // tracerControl
            // 
            this.tracerControl.DetailsVisible = false;
            this.tracerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tracerControl.Location = new System.Drawing.Point(0, 0);
            this.tracerControl.Name = "tracerControl";
            this.tracerControl.ShowDetails = false;
            this.tracerControl.ShowMethodFilter = true;
            this.tracerControl.Size = new System.Drawing.Size(792, 210);
            this.tracerControl.TabIndex = 0;
            this.tracerControl.Tracer = null;
            // 
            // toolStripAdditional
            // 
            this.toolStripAdditional.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripAdditional.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripAdditional.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonInitialize,
            this.toolStripSeparator1,
            this.toolStripButtonChart,
            this.toolStripButtonProperties});
            this.toolStripAdditional.Location = new System.Drawing.Point(260, 82);
            this.toolStripAdditional.Name = "toolStripAdditional";
            this.toolStripAdditional.Size = new System.Drawing.Size(140, 25);
            this.toolStripAdditional.TabIndex = 2;
            this.toolStripAdditional.Text = "toolStrip1";
            // 
            // toolStripButtonInitialize
            // 
            this.toolStripButtonInitialize.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInitialize.Image")));
            this.toolStripButtonInitialize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInitialize.Name = "toolStripButtonInitialize";
            this.toolStripButtonInitialize.Size = new System.Drawing.Size(55, 22);
            this.toolStripButtonInitialize.Text = "Setup";
            this.toolStripButtonInitialize.Click += new System.EventHandler(this.toolStripButtonInitialize_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonChart
            // 
            this.toolStripButtonChart.Checked = true;
            this.toolStripButtonChart.CheckOnClick = true;
            this.toolStripButtonChart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonChart.Enabled = false;
            this.toolStripButtonChart.Image = global::ForexPlatformFrontEnd.Properties.Resources.CHART;
            this.toolStripButtonChart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonChart.Name = "toolStripButtonChart";
            this.toolStripButtonChart.Size = new System.Drawing.Size(54, 22);
            this.toolStripButtonChart.Text = "Chart";
            this.toolStripButtonChart.Visible = false;
            this.toolStripButtonChart.CheckStateChanged += new System.EventHandler(this.toolStripButtonChart_CheckStateChanged);
            // 
            // toolStripButtonProperties
            // 
            this.toolStripButtonProperties.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonProperties.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonProperties.Image")));
            this.toolStripButtonProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonProperties.Name = "toolStripButtonProperties";
            this.toolStripButtonProperties.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonProperties.Text = "Properties";
            this.toolStripButtonProperties.Click += new System.EventHandler(this.toolStripButtonProperties_Click);
            // 
            // expertSessionControl
            // 
            this.expertSessionControl.CorrespondingOrdersControl = null;
            this.expertSessionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expertSessionControl.ImageName = "";
            this.expertSessionControl.Location = new System.Drawing.Point(0, 0);
            this.expertSessionControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.expertSessionControl.Name = "expertSessionControl";
            this.expertSessionControl.Session = null;
            this.expertSessionControl.ShowChartControl = false;
            this.expertSessionControl.Size = new System.Drawing.Size(800, 325);
            this.expertSessionControl.TabIndex = 4;
            // 
            // ordersControlExpertSession
            // 
            this.ordersControlExpertSession.AllowCompactMode = true;
            this.ordersControlExpertSession.AllowOrderManagement = false;
            this.ordersControlExpertSession.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersControlExpertSession.ImageName = "";
            this.ordersControlExpertSession.Location = new System.Drawing.Point(3, 3);
            this.ordersControlExpertSession.Margin = new System.Windows.Forms.Padding(2);
            this.ordersControlExpertSession.Name = "ordersControlExpertSession";
            this.ordersControlExpertSession.SessionManager = null;
            this.ordersControlExpertSession.SingleSession = null;
            this.ordersControlExpertSession.Size = new System.Drawing.Size(786, 204);
            this.ordersControlExpertSession.TabIndex = 1;
            // 
            // accountControl1
            // 
            this.accountControl1.Account = null;
            this.accountControl1.AutoSize = true;
            this.accountControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.accountControl1.Location = new System.Drawing.Point(0, 577);
            this.accountControl1.Margin = new System.Windows.Forms.Padding(2);
            this.accountControl1.Name = "accountControl1";
            this.accountControl1.Provider = null;
            this.accountControl1.Size = new System.Drawing.Size(800, 23);
            this.accountControl1.TabIndex = 6;
            this.accountControl1.Visible = false;
            this.accountControl1.VisibleChanged += new System.EventHandler(this.accountControl1_VisibleChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 333);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 236);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.Visible = false;
            this.tabControl1.VisibleChanged += new System.EventHandler(this.tabControl1_VisibleChanged);
            this.tabControl1.SizeChanged += new System.EventHandler(this.tabControl1_SizeChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.positionsControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 210);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Positions";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // positionsControl1
            // 
            this.positionsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionsControl1.ImageName = "";
            this.positionsControl1.Location = new System.Drawing.Point(3, 3);
            this.positionsControl1.Name = "positionsControl1";
            this.positionsControl1.Size = new System.Drawing.Size(786, 204);
            this.positionsControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ordersControlExpertSession);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 210);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Orders";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tracerControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(792, 210);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Trace";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ManagedExpertControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripAdditional);
            this.Controls.Add(this.expertSessionControl);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.accountControl1);
            this.ImageName = "currency_dollar_grayscale.png";
            this.Name = "ManagedExpertControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.SizeChanged += new System.EventHandler(this.ManagedExpertControl_SizeChanged);
            this.toolStripAdditional.ResumeLayout(false);
            this.toolStripAdditional.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripAdditional;
        private System.Windows.Forms.ToolStripButton toolStripButtonInitialize;
        private OrdersControl ordersControlExpertSession;
        private PlatformExpertSessionControl expertSessionControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonChart;
        private CommonSupport.TracerControl tracerControl;
        private System.Windows.Forms.ToolStripButton toolStripButtonProperties;
        private AccountControl accountControl1;
        private SplitterEx splitter2;
        private SplitterEx splitter1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private PositionsControl positionsControl1;
        private System.Windows.Forms.TabPage tabPage3;
    }
}
