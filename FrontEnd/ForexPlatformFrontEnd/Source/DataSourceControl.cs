using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    public partial class DataSourceControl : PlatformComponentControl
    {
        DataSource DataSource
        {
            get { return (DataSource)base.Component; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSourceControl()
            : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSourceControl(DataSource dataSource)
            : base(dataSource)
        {
            Construct(dataSource);
        }

        //public DataSourceControl(FileDataSource dataSource)
        //    : base(dataSource)
        //{
        //    Construct(dataSource);
        //}

        //public DataSourceControl(RemoteDataSource dataSource)
        //    : base(dataSource)
        //{
        //    Construct(dataSource);
        //}

        void Construct(DataSource dataSource)
        {
            InitializeComponent();

            //Title = DataSource.Name;
            this.Name = dataSource.Name;
            this.sessionInfosControl1.SessionManager = DataSource;

            DataSource.SessionValuesUpdateEvent += new DataSource.SessionValuesUpdateDelegate(DataProviderSource_SessionValuesUpdateEvent);
            DataSource.OperationalStatusChangedEvent += new OperationalStatusChangedDelegate(DataSource_OperationalStatusChangedEvent);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateUI();
        }

        void DataSource_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate(UpdateUI));
        }

        private void DataProviderSourceControl_Load(object sender, EventArgs e)
        {
            //sessionInfosControl1.ListViewSessions.Columns[sessionInfosControl1.ListViewSessions.Columns.Count - 1].Width = 200;
            //sessionInfosControl1.ListViewSessions.Columns.Add("Last Update").Width = 200;
            //sessionInfosControl1.ListViewSessions.Columns.Add("Value Count").Width = 140;
        }

        void DataProviderSource_SessionValuesUpdateEvent(DataSource parameter1, SessionInfo parameter2, int parameter3)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.GenericDelegate<DataSource, SessionInfo, int>(SessionValuesUpdateEvent), parameter1, parameter2, parameter3);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (DataSource != null)
            {
                this.toolStripLabelStatus.Text = DataSource.OperationalState.ToString();
                toolStripLabelName.Text = DataSource.Name;

                if (DataSource.OperationalState != OperationalStateEnum.Operational)
                {
                    toolStripLabelStatus.ForeColor = Color.DarkRed;
                }
                else
                {
                    toolStripLabelStatus.ForeColor = Color.DarkGreen;
                }
            }
        }


        /// <summary>
        /// UI thread.
        /// </summary>
        void SessionValuesUpdateEvent(DataSource parameter1, SessionInfo parameter2, int parameter3)
        {
            //ListViewItem item = sessionInfosControl1.GetItemBySessionInfo(parameter2);
            //if (item != null)
            //{
            //    while(item.SubItems.Count < 5)
            //    {
            //        item.SubItems.Add("");
            //    }

            //    item.SubItems[3].Text = DateTime.Now.ToString();
            //    item.SubItems[4].Text = parameter3.ToString();
            //}
        }

    }
}
