using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonFinancial;
using CommonSupport;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Form allows the manual creation of Expert SessionsArray on an expert sessionInformation host/manager.
    /// </summary>
    public partial class CreateExpertSessionForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public CreateExpertSessionForm(LocalExpertHost host)
        {
            InitializeComponent();
            this.createExpertSessionControl.Host = host;
            this.createExpertSessionControl.SessionCreatedEvent += new CreateExpertSessionControl2.SessionCreatedDelegate(createExpertSessionControl1_SessionCreatedEvent);
        }

        private void CreateExpertSessionForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = createExpertSessionControl.buttonCreateSession;
            createExpertSessionControl.VisibleChanged += new EventHandler(createExpertSessionControl_VisibleChanged);
            this.Text = Application.ProductName + " - " + " Select Session Type and Symbol";
        }

        void createExpertSessionControl_VisibleChanged(object sender, EventArgs e)
        {
            if (createExpertSessionControl.Visible == false)
            {
                createExpertSessionControl1_SessionCreatedEvent();
            }
        }

        void createExpertSessionControl1_SessionCreatedEvent()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CreateExpertSessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.createExpertSessionControl.SessionCreatedEvent -= new CreateExpertSessionControl2.SessionCreatedDelegate(createExpertSessionControl1_SessionCreatedEvent);
            this.createExpertSessionControl.Host = null;
        }

    }
}
