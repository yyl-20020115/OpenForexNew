
namespace CommonFinancial
{
    partial class ChartForm
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
        /// Required baseMethod for Designer support - do not modify
        /// the contents of this baseMethod with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chartControl = new CommonFinancial.ChartControl();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.Margin = new System.Windows.Forms.Padding(1);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(676, 461);
            this.chartControl.TabIndex = 0;
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 461);
            this.Controls.Add(this.chartControl);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ChartForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Chart Form";
            this.ResumeLayout(false);

        }

        #endregion

        protected ChartControl chartControl;


    }
}