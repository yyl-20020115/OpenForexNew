using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using CommonFinancial;

namespace ForexPlatformFrontEnd
{
    public partial class SessionInfosControl : UserControl
    {
        //SessionSource _manager;
        //public SessionSource Manager
        //{
        //    get { return _manager; }
        //    set
        //    {
        //        if (_manager != null)
        //        {
        //            _manager.OperationalStateChangedEvent -= new OperationalStateChangedDelegate(_sessionManager_OperationalStatusChangedEvent);
        //            _manager.SessionsUpdateEvent -= new SessionSource.SessionSourceUpdateDelegate(_manager_SessionUpdateEvent);
        //            _manager = null;
        //        }

        //        _manager = value;
        //        if (_manager != null)
        //        {
        //            _manager.OperationalStateChangedEvent += new OperationalStateChangedDelegate(_sessionManager_OperationalStatusChangedEvent);
        //            _manager.SessionsUpdateEvent += new SessionSource.SessionSourceUpdateDelegate(_manager_SessionUpdateEvent);
        //        }

        //        DoUpdateUI();
        //    }
        //}

        //public Info? SelectedSessionInfo
        //{
        //    set
        //    {
        //        listViewSessions.SelectedIndices.Clear();
        //        //foreach (ListViewItem item in listViewSessions.Items)
        //        //{
        //        //    if (value.HasValue && value.Value.Equals((Info)item.Tag))
        //        //    {
        //        //        item.Selected = true;
        //        //        return;
        //        //    }
        //        //}

        //        return;
        //    }

        //    get
        //    {
        //        //if (listViewSessions.SelectedIndices.Count == 0)
        //        //{
        //        //    return null;
        //        //}
        //        //return (Info)listViewSessions.SelectedItems[0].Tag;
        //        return null;
        //    }
        //}

        public ListView ListViewSessions
        {
            get { return this.listViewSessions; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SessionInfosControl()
        {
            InitializeComponent();
        }

        ///// <summary>
        ///// External thread.
        ///// </summary>
        //void _manager_SessionUpdateEvent(SessionSource parameter1)
        //{
        //    if (this.IsDisposed == false &&
        //        this.Disposing == false && this.IsHandleCreated)
        //    {
        //        WinFormsHelper.BeginFilteredManagedInvoke(this, new GeneralHelper.DefaultDelegate(DoUpdateUI));
        //    }
        //}

        void _sessionManager_OperationalStatusChangedEvent(IOperational parameter1, OperationalStateEnum parameter2)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// UI thread.
        /// Update user interface based on the underlying information.
        /// </summary>
        public void UpdateUI()
        {
            TracerHelper.TraceEntry();

            listViewSessions.SuspendLayout();

            //if (_manager != null)
            //{
            //    if (_manager.OperationalState != OperationalStateEnum.Operational)
            //    {
            //        listViewSessions.ForeColor = SystemColors.GrayText;
            //    }

            //    int totalCount = 0;
            //    lock (_manager)
            //    {// Since we do a lenghtly iteration make sure full lock is applied.
            //        foreach (string group in _manager.Groups)
            //        {
            //            totalCount += _manager.GetGroupSessionsCount(group);
            //        }
            //    }
            //    listViewSessions.VirtualListSize = totalCount;
            //}

            listViewSessions.ResumeLayout();
        }

        /// <summary>
        /// UI thread.
        /// </summary>
        private void SessionInfosControl_Resize(object sender, EventArgs e)
        {
            listViewSessions.Columns[listViewSessions.Columns.Count - 1].Width = -2;
        }

        private void SessionInfosControl_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void listViewSessions_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //if (_manager != null)
            //{
            //    int totalCount = 0;
            //    lock (_manager)
            //    {// Since we do a lenghtly iteration make sure full lock is applied.
            //        foreach (string group in _manager.Groups)
            //        {
            //            List<DataSessionInfo> sessions = new List<DataSessionInfo>(_manager.GetGroupSessions(group));
            //            int currentCount = sessions.Count;
            //            if (e.ItemIndex >= totalCount && e.ItemIndex < totalCount + currentCount)
            //            {
            //                DataSessionInfo orderInfo = sessions[e.ItemIndex - totalCount];

            //                e.Item = new ListViewItem(new string[] { orderInfo.Name, orderInfo.BaseCurrency.Name, "-", "-" });
            //                e.Item.Tag = orderInfo;

            //                return;
            //            }

            //            totalCount += sessions.Count;
            //        }
            //    }
            //}

            e.Item = new ListViewItem("Item not found.");
            SystemMonitor.Error("Item not found.");
        }

        private void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            //if (_manager != null)
            //{
            //    _manager.UpdateSessions();
            //}
        }


    }
}
