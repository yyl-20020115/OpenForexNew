using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    public partial class TimeManagementSkipToControl : UserControl
    {
        ITimeControl _timeControl;
        
        /// <summary>
        /// 
        /// </summary>
        public TimeManagementSkipToControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeManagementSkipToControl(ITimeControl timeControl)
        {
            InitializeComponent();
            _timeControl = timeControl;
        }

        private void TimeManagementSkipToControl_Load(object sender, EventArgs e)
        {
            UpdateUI(false);
            numericUpDownSkip.Focus();
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI(bool stepToFlag)
        {
            labelOf.Text = "of " + (_timeControl.TotalStepsCount).ToString();
            textBoxCurrent.Text = _timeControl.CurrentStep.ToString();
            if (_timeControl.CurrentStep.HasValue)
            {
                numericUpDownSkip.Minimum = _timeControl.CurrentStep.Value;
            }
            else
            {
                numericUpDownSkip.Minimum = 0;
            }
            if (_timeControl.TotalStepsCount.HasValue)
            {
                numericUpDownSkip.Maximum = _timeControl.TotalStepsCount.Value;
                numericUpDownSkip.Enabled = true;
            }
            else
            {
                numericUpDownSkip.Maximum = 0;
                numericUpDownSkip.Enabled = false;
            }

            if (stepToFlag)
            {// Step to complete flag.
                buttonSkipTo.Enabled = _timeControl.CurrentStep < _timeControl.TotalStepsCount;
                buttonSkipToEnd.Enabled = buttonSkipTo.Enabled;
                numericUpDownSkip.Enabled = buttonSkipTo.Enabled;
                timerUI.Enabled = false;
                UpdateUI(false);
            }
        }

        /// <summary>
        /// Non UI thread.
        /// </summary>
        void DoStepTo()
        {
            _timeControl.StepTo((int)numericUpDownSkip.Value);

            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<bool>(UpdateUI), true);
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            UpdateUI(false);
        }

        private void buttonSkipToEnd_Click(object sender, EventArgs e)
        {
            numericUpDownSkip.Value = numericUpDownSkip.Maximum;
            buttonSkipTo_Click(sender, e);
        }

        private void buttonSkipTo_Click(object sender, EventArgs e)
        {
            if (numericUpDownSkip.Value != int.Parse(textBoxCurrent.Text))
            {
                buttonSkipTo.Enabled = false;
                buttonSkipToEnd.Enabled = false;
                numericUpDownSkip.Enabled = false;

                timerUI.Enabled = true;

                GeneralHelper.FireAndForget(DoStepTo);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            timerUI.Enabled = false;
            this.Hide();
        }


    }
}
