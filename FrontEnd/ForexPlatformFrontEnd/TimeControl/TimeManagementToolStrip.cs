using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Toolstrip defines tradeEntities operations of time controlled items.
    /// </summary>
    public class TimeManagementToolStrip : ToolStrip
    {
        /// <summary>
        /// When the last spep was done.
        /// </summary>
        DateTime _lastStep = DateTime.Now;

        /// <summary>
        /// Maximum steps per update.
        /// </summary>
        int _maxStepsPerUpdate = 50;

        /// <summary>
        /// Left over steps from previous execs.
        /// </summary>
        float _stepsReserve = 0;

        ITimeControl _control;
        /// <summary>
        /// This is the main controller providing dataDelivery form the controlling.
        /// </summary>
        public ITimeControl Controller
        {
            get { return _control; }
            set
            {
                if (_control != null)
                {
                    if (_control is IOperational)
                    {
                        ((IOperational)_control).OperationalStateChangedEvent -= new OperationalStateChangedDelegate(TimeManagementToolStrip_OperationalStatusChangedEvent);
                    }
                    _control.CurrentStepChangedEvent -= new ITimeControlStepUpdateDelegate(_controller_CurrentPeriodChangedEvent);
                    _control = null;
                }

                _control = value;
                this.Enabled = _control != null;

                if (_control != null)
                {
                    if (_control is IOperational)
                    {
                        ((IOperational)_control).OperationalStateChangedEvent += new OperationalStateChangedDelegate(TimeManagementToolStrip_OperationalStatusChangedEvent);
                    }
            
                    _control.CurrentStepChangedEvent += new ITimeControlStepUpdateDelegate(_controller_CurrentPeriodChangedEvent);
                    
                    UpdateUI();
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeManagementToolStrip()
        {
            InitializeComponent();

            this.Enabled = false;

            _host = new ToolStripControlHost(trackBarSpeed);
            this.Items.Add(_host);
        }

        private ToolStripControlHost _host;

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
            ToolStripSeparator toolStripSeparator1;
            ToolStripSeparator toolStripSeparator2;
            ToolStripSeparator toolStripSeparator3;
            ToolStripLabel toolStripLabel1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeManagementToolStrip));
            //this.toolStrip = new ToolStrip();
            this.toolStripSeparator4 = new ToolStripSeparator();
            this.toolStripButtonRestart = new ToolStripButton();
            this.toolStripButtonSkipTo = new ToolStripButton();
            this.toolStripButtonStep = new ToolStripButton();
            this.toolStripButtonRun = new ToolStripButton();
            this.toolStripButtonStop = new ToolStripButton();
            this.toolStripSeparator5 = new ToolStripSeparator();
            this.toolStripLabelStatus = new ToolStripLabel();
            this.toolStripSeparator6 = new ToolStripSeparator();
            this.toolStripLabelSpeed = new ToolStripLabel();
            this.trackBarSpeed = new TrackBar();
            this.timerStep = new Timer(this.components);
            this.toolTip = new ToolTip(this.components);
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(41, 22);
            toolStripLabel1.Text = "Time";
            // 
            // toolStrip
            // 
            this.GripStyle = ToolStripGripStyle.Visible;
            this.Items.AddRange(new ToolStripItem[] {
            toolStripLabel1,
            this.toolStripSeparator4,
            this.toolStripButtonRestart,
            toolStripSeparator2,
            this.toolStripButtonStep,
            toolStripSeparator1,
            this.toolStripButtonRun,
            this.toolStripButtonStop,
            this.toolStripSeparator6,
            this.toolStripButtonSkipTo,
            toolStripSeparator3,
            this.toolStripLabelStatus,
            this.toolStripSeparator5,
            this.toolStripLabelSpeed});
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "toolStrip";
            this.RenderMode = ToolStripRenderMode.System;
            this.Size = new System.Drawing.Size(800, 25);
            this.TabIndex = 0;
            this.Text = "toolStrip1";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBegining
            // 
            this.toolStripButtonRestart.Enabled = false;
            this.toolStripButtonRestart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRestartImage")));
            this.toolStripButtonRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRestart.Name = "toolStripButtonBegining";
            this.toolStripButtonRestart.Size = new System.Drawing.Size(81, 22);
            this.toolStripButtonRestart.Text = "Restart";
            this.toolStripButtonRestart.ToolTipText = "Restart";
            this.toolStripButtonRestart.Click += new EventHandler(toolStripButtonRestart_Click);

            // 
            // toolStripButtonSkipTo
            // 
            this.toolStripButtonSkipTo.Enabled = true;
            this.toolStripButtonSkipTo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSkipToImage")));
            this.toolStripButtonSkipTo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSkipTo.Name = "toolStripButtonSkipTo";
            this.toolStripButtonSkipTo.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonSkipTo.Text = "Run To";
            this.toolStripButtonSkipTo.Click += new System.EventHandler(this.toolStripButtonSkipTo_Click);
            // 
            // toolStripButtonStep
            // 
            this.toolStripButtonStep.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStepImage")));
            this.toolStripButtonStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStep.Name = "toolStripButtonStep";
            this.toolStripButtonStep.Size = new System.Drawing.Size(57, 22);
            this.toolStripButtonStep.Text = "Step";
            this.toolStripButtonStep.ToolTipText = "Step Forward";
            this.toolStripButtonStep.Click += new System.EventHandler(this.toolStripButtonStep_Click);
            // 
            // toolStripButtonRun
            // 
            this.toolStripButtonRun.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRunImage")));
            this.toolStripButtonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonRun.Text = "Run";
            this.toolStripButtonRun.ToolTipText = "Run Auto-Step";
            this.toolStripButtonRun.Click += new System.EventHandler(this.toolStripButtonRun_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.Enabled = false;
            this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStopImage")));
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(67, 22);
            this.toolStripButtonStop.Text = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonPause_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelStatus
            // 
            this.toolStripLabelStatus.AutoSize = false;
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Size = new System.Drawing.Size(130, 22);
            this.toolStripLabelStatus.Text = "0 of 0";
            this.toolStripLabelStatus.ToolTipText = "Current period out of total periods";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelSpeed
            // 
            this.toolStripLabelSpeed.Name = "toolStripLabelSpeed";
            this.toolStripLabelSpeed.Size = new System.Drawing.Size(124, 22);
            this.toolStripLabelSpeed.Text = "Running Speed";
            this.toolStripLabelSpeed.ToolTipText = "Running speed of auto-steps";
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.AutoSize = false;
            this.trackBarSpeed.Location = new System.Drawing.Point(614, 4);
            this.trackBarSpeed.Maximum = 40;
            this.trackBarSpeed.Minimum = 1;
            this.trackBarSpeed.Name = "trackBarSpeed";
            this.trackBarSpeed.Size = new System.Drawing.Size(220, 19);
            this.trackBarSpeed.TabIndex = 1;
            this.trackBarSpeed.SmallChange = 1;
            this.trackBarSpeed.LargeChange = 2;
            this.trackBarSpeed.TickStyle = TickStyle.None;
            this.trackBarSpeed.Value = 1;
            this.trackBarSpeed.ValueChanged += new System.EventHandler(this.trackBarSpeed_ValueChanged);
            // 
            // timerStep
            // 
            this.timerStep.Tick += new System.EventHandler(this.timerStep_Tick);
            // 
            // TimeManagementControl
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            //this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            //this.Controls.AddElement(this.trackBarSpeed);
            //this.Controls.AddElement(this.toolStrip);
            this.Name = "TimeManagementControl";
            this.Size = new System.Drawing.Size(800, 32);
            this.Resize += new System.EventHandler(this.TimeManagementToolStrip_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStripButton toolStripButtonRestart;
        private ToolStripButton toolStripButtonSkipTo;
        private ToolStripButton toolStripButtonRun;
        private ToolStripButton toolStripButtonStop;
        private TrackBar trackBarSpeed;
        private ToolStripLabel toolStripLabelSpeed;
        private ToolStripButton toolStripButtonStep;
        private ToolStripSeparator toolStripSeparator4;
        private Timer timerStep;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripLabel toolStripLabelStatus;
        private ToolStripSeparator toolStripSeparator6;
        private ToolTip toolTip;


        void _controller_CurrentPeriodChangedEvent(ITimeControl control)
        {
            if (InvokeRequired)
            {
                WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
            }
            else
            {
                UpdateUI();
            }
        }

        private void TimeManagementToolStrip_Resize(object sender, EventArgs e)
        {
            //trackBarSpeed.Location = new Point(toolStripLabelSpeed.Bounds.Right, trackBarSpeed.Location.Y);
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {// Stepping interval must be not less than 100 ms.
            this.toolStripLabelSpeed.Text = "Running Speed (" + StepsPerSecond.ToString("000.0") + " Steps/Sec)";
            timerStep.Interval = Math.Max(100, (int)((float)1000 / StepsPerSecond));
        }

        /// <summary>
        /// Steps per second increase exponentially with the increase of the trackbar.
        /// </summary>
        public float StepsPerSecond
        {
            get 
            {
                float multiplicator = Math.Max(0.5f, trackBarSpeed.Value / 10f);
                return multiplicator * multiplicator * trackBarSpeed.Value;
            }
        }

        /// <summary>
        /// How big should the next step be. It calculates time passed since the last step and the StepsPerSecond required.
        /// </summary>
        public int GetPendingStepSize()
        {
            TimeSpan timePassed = DateTime.Now - _lastStep;
            float steps = Math.Min(_maxStepsPerUpdate, (float)timePassed.TotalMilliseconds * (float)StepsPerSecond / (float)1000);
            _stepsReserve += steps;
            _lastStep = DateTime.Now;

            int result = (int)_stepsReserve;
            _stepsReserve = _stepsReserve - result;

            return result;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.trackBarSpeed.Value = 10;
        }

        void toolStripButtonRestart_Click(object sender, EventArgs e)
        {
            //_controller.restart
        }

        void TimeManagementToolStrip_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum previousState)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI); 
        }

        private void toolStripButtonSkipTo_Click(object sender, EventArgs e)
        {
            HostingForm form = new HostingForm("Fast Run", new TimeManagementSkipToControl(_control));
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.ShowDialog();
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            toolStripLabelStatus.Text = "Step " + _control.CurrentStep.ToString() + " of " + _control.TotalStepsCount.ToString();

        }

        void UpdateUIEnables()
        {
            toolStripButtonRestart.Enabled = false; // !timerStep.Enabled;
            toolStripButtonStop.Enabled = timerStep.Enabled;
            toolStripButtonRun.Enabled = !timerStep.Enabled;
            toolStripButtonSkipTo.Enabled = !timerStep.Enabled;
            toolStripButtonStep.Enabled = !timerStep.Enabled;
        }

        protected void Step(float steps)
        {
            ITimeControl control = _control;
            if (control == null)
            {
                return;
            }

            // TODO: for a better precision, remember the leftovers in steps, and pass them on next go.
            if (control.StepForward((int)steps) == false)
            {
                timerStep.Enabled = false;
                UpdateUIEnables();
            }
        }

        private void toolStripButtonStep_Click(object sender, EventArgs e)
        {
            Step(1);
            UpdateUI();
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            GetPendingStepSize(); // Reset the stepping start interval.
            timerStep.Enabled = true;
            UpdateUIEnables();
        }

        private void timerStep_Tick(object sender, EventArgs e)
        {
            int steps = GetPendingStepSize();
            if (steps > 0)
            {
                Step(steps);
                UpdateUI();
                this.Refresh();
            }
        }

        private void toolStripButtonPause_Click(object sender, EventArgs e)
        {
            timerStep.Enabled = false;
            UpdateUIEnables();
        }
    }
}

