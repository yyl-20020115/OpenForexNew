namespace ForexPlatformFrontEnd
{
    partial class MasterOrderManagementControl
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
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterOrderManagementControl));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.executionAccountsControl1 = new ForexPlatformFrontEnd.ExecutionAccountsControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ordersControl1 = new ForexPlatformFrontEnd.OrdersControl();
            this.masterOrderControl1 = new ForexPlatformFrontEnd.MasterOrderControl();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "book_yellow.png");
            this.imageList1.Images.SetKeyName(1, "address_book.png");
            this.imageList1.Images.SetKeyName(2, "FORM_RED.PNG");
            this.imageList1.Images.SetKeyName(3, "book_blue.png");
            // 
            // executionAccountsControl1
            // 
            this.executionAccountsControl1.AutoScroll = true;
            this.executionAccountsControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.executionAccountsControl1.Location = new System.Drawing.Point(0, 573);
            this.executionAccountsControl1.MaximumSize = new System.Drawing.Size(0, 27);
            this.executionAccountsControl1.MinimumSize = new System.Drawing.Size(0, 27);
            this.executionAccountsControl1.Name = "executionAccountsControl1";
            this.executionAccountsControl1.SessionManager = null;
            this.executionAccountsControl1.Size = new System.Drawing.Size(800, 27);
            this.executionAccountsControl1.TabIndex = 8;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 567);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(800, 6);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // ordersControl1
            // 
            this.ordersControl1.AllowCompactMode = true;
            this.ordersControl1.AllowOrderManagement = true;
            this.ordersControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ordersControl1.Location = new System.Drawing.Point(0, 149);
            this.ordersControl1.Margin = new System.Windows.Forms.Padding(2);
            this.ordersControl1.Name = "ordersControl1";
            this.ordersControl1.SessionManager = null;
            this.ordersControl1.SingleSession = null;
            this.ordersControl1.Size = new System.Drawing.Size(800, 418);
            this.ordersControl1.TabIndex = 11;
            // 
            // masterOrderControl1
            // 
            this.masterOrderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterOrderControl1.ImageName = "";
            this.masterOrderControl1.Location = new System.Drawing.Point(0, 0);
            this.masterOrderControl1.Name = "masterOrderControl1";
            this.masterOrderControl1.Size = new System.Drawing.Size(800, 141);
            this.masterOrderControl1.TabIndex = 12;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 141);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(800, 8);
            this.splitter2.TabIndex = 13;
            this.splitter2.TabStop = false;
            // 
            // MasterOrderManagementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.masterOrderControl1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.ordersControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.executionAccountsControl1);
            this.ImageName = "CLIENTS.PNG";
            this.Name = "MasterOrderManagementControl";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private ExecutionAccountsControl executionAccountsControl1;
        private System.Windows.Forms.Splitter splitter1;
        private OrdersControl ordersControl1;
        private MasterOrderControl masterOrderControl1;
        private System.Windows.Forms.Splitter splitter2;
    }
}
