using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control visualizes properties for the MT4 integration component.
    /// </summary>
    public partial class AdapterManagementComponentControl : PlatformComponentControl
    {
        List<Type> _adapterTypes = new List<Type>();

        /// <summary>
        /// 
        /// </summary>
        AdapterManagementComponent Operator
        {
            get { return (AdapterManagementComponent)base.Component; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdapterManagementComponentControl()
            : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Functional constructor.
        /// </summary>
        public AdapterManagementComponentControl(AdapterManagementComponent integrationOperator)
            : base(integrationOperator)
        {
            InitializeComponent();

            this.Name = Operator.Name;

            //// Store in platform orderInfo, since we want to keep this orderInfo, event if the component is unregisted/removed from platform.
            //if (integrationOperator.IsInitialized &&
            //    integrationOperator.Platform.UISerializationInfo.ContainsValue("IntegrationMT4PlatformOperatorControl.integrationAddresses"))
            //{
            //    _integrationAddresses.AddRange(integrationOperator.Platform.UISerializationInfo.GetValue<string[]>("IntegrationMT4PlatformOperatorControl.integrationAddresses"));
            //}

            integrationOperator.Adapters.ItemOperationalStatusChangedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemOperationalStatusChangedEvent);
            integrationOperator.Adapters.ItemAddedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemAddedEvent);
            integrationOperator.Adapters.ItemRemovedEvent += new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemRemovedEvent);

            UpdateUI();

            if (integrationOperator.IsInitialized)
            {
                //toolStripTextBoxNewAddress.Text = integrationOperator.Platform.Settings.DefaultMT4IntegrationAddress;
            }

            // We need to create early.
            this.Tag = integrationOperator;
            this.CreateControl();
        }

        private void AdapterManagementOperatorControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            _adapterTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(IIntegrationAdapter), false, false, 
                ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies(), null);

            foreach (Type type in _adapterTypes)
            {
                toolStripComboBoxAdapterType.Items.Add(UserFriendlyNameAttribute.GetTypeAttributeName(type));
            }

            if (_adapterTypes.Count == 0)
            {
                toolStripComboBoxAdapterType.Enabled = false;
                toolStripButtonCreate.Enabled = false;
            }
            else
            {
                toolStripComboBoxAdapterType.SelectedIndex = 0;
            }
        }

        public override void UnInitializeControl()
        {
            if (Operator != null)
            {
                Operator.Adapters.ItemOperationalStatusChangedEvent -= new GenericContainer<IIntegrationAdapter>.ItemUpdateDelegate(Adapters_ItemOperationalStatusChangedEvent);
            }

            base.UnInitializeControl();
        }

        void Adapters_ItemRemovedEvent(GenericContainer<IIntegrationAdapter> keeper, IIntegrationAdapter item)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        void Adapters_ItemAddedEvent(GenericContainer<IIntegrationAdapter> keeper, IIntegrationAdapter item)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        void Adapters_ItemOperationalStatusChangedEvent(GenericContainer<IIntegrationAdapter> keeper, IIntegrationAdapter item)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, UpdateUI);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (this.DesignMode)
            {
                return;
            }

            if (Operator == null)
            {
                this.Enabled = false;
                return;
            }

            IIntegrationAdapter[] adapters = Operator.Adapters.ToArray();
            for (int i = 0; i < adapters.Length; i++)
            {
                ListViewItem item = null;
                if (listViewIntegrations.Items.Count <= i)
                {
                    item = new ListViewItem();
                    item.SubItems.Add("");
                    item.SubItems.Add("");

                    listViewIntegrations.Items.Add(item);
                }
                else
                {
                    item = listViewIntegrations.Items[i];
                }

                SetItemAsAdapter(item, adapters[i]);
            }

            while (listViewIntegrations.Items.Count > adapters.Length)
            {
                listViewIntegrations.Items.RemoveAt(listViewIntegrations.Items.Count - 1);
            }
        }

        void SetItemAsAdapter(ListViewItem item, IIntegrationAdapter adapter)
        {
            item.Text = adapter.IsStarted.ToString();
            item.SubItems[1].Text = adapter.OperationalState.ToString();
            item.SubItems[2].Text = adapter.Name;

            if (adapter.IsStarted == false)
            {
                item.ForeColor = SystemColors.GrayText;
            }
            else
            {
                if (adapter.OperationalState == OperationalStateEnum.Initializing)
                {
                    item.ForeColor = Color.Blue;
                }
                else if (adapter.OperationalState != OperationalStateEnum.Operational)
                {
                    item.ForeColor = Color.Red;
                }
                else
                {
                    item.ForeColor = SystemColors.WindowText;
                }
            }

            item.Tag = adapter;
        }

        //private void buttonAdd_Click(object sender, EventArgs e)
        //{
        //    //if (_integrationAddresses.Contains(toolStripTextBoxNewAddress.Text) == false
        //    //    && string.IsNullOrEmpty(toolStripTextBoxNewAddress.Text) == false)
        //    //{
        //    //    Uri uri = null;
        //    //    if (Uri.TryCreate(toolStripTextBoxNewAddress.Text, UriKind.Absolute, out uri))
        //    //    {
        //    //        _integrationAddresses.AddElement(toolStripTextBoxNewAddress.Text);
        //    //        DoUpdateUI();
        //    //    }
        //    //    else
        //    //    {
        //    //        MessageBox.Show("Invalid URI address.");
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show("Item with this address already added.");
        //    //}

        //    UpdatePersistenceData();
        //}

        //void UpdatePersistenceData()
        //{
        //    if (Operator != null && Operator.Platform != null)
        //    {
        //        Operator.Platform.UISerializationInfo.AddValue("IntegrationMT4PlatformOperatorControl.integrationAddresses", _integrationAddresses.ToArray());
        //    }
        //}

        //private void buttonRemove_Click(object sender, EventArgs e)
        //{
        //    foreach (ListViewItem item in listViewAddresses.SelectedItems)
        //    {
        //        _integrationAddresses.Remove(item.Text);
        //    }

        //    UpdatePersistenceData();

        //    DoUpdateUI();
        //}

        //private void toolStripButtonConnect_Click(object sender, EventArgs e)
        //{
        //    //foreach (ListViewItem item in listViewAddresses.SelectedItems)
        //    //{
        //    //    Operator.AddConnection(item.Text);
        //    //}
        //    //DoUpdateUI();
        //}

        //private void toolStripButtonRemoveIntegration_Click(object sender, EventArgs e)
        //{
        //    //foreach (ListViewItem item in listViewIntegrations.SelectedItems)
        //    //{
        //    //    MT4Adapter connection = (MT4Adapter)item.Tag;
        //    //    if (connection.OperationalState == OperationalStateEnum.Initializing)
        //    //    {
        //    //        MessageBox.Show("Connection [" + connection.IntegrationUri.ToString() + "] still initializing. Wait for it to initialize or time out [" + connection.DefaultTimeOut.TotalSeconds.ToString() +"] secs before removing.");
        //    //    }
        //    //    else
        //    //    {
        //    //        Operator.RemoveConnection(connection);
        //    //    }
        //    //}

        //    //DoUpdateUI();
        //}


        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void toolStripButtonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                int index = toolStripComboBoxAdapterType.SelectedIndex;
                Type adapterType = _adapterTypes[index];

                // First try the specialized constructor, if available.
                ConstructorInfo constructor = adapterType.GetConstructor(new Type[] { typeof(AdapterManagementComponent) });

                if (constructor == null)
                {// Try the default parameterless constructor.
                    constructor = adapterType.GetConstructor(new Type[] { });
                }

                if (constructor == null)
                {
                    SystemMonitor.Error("Constructor not found.");
                    return;
                }

                IIntegrationAdapter adapter;
                if (constructor.GetParameters() == null || constructor.GetParameters().Length == 0)
                {// Default constructor.
                    adapter = (IIntegrationAdapter)constructor.Invoke(null);
                }
                else
                {// Specialized constructor.
                    adapter = (IIntegrationAdapter)constructor.Invoke(new object[] { Operator });
                }

                if (adapter == null)
                {
                    MessageBox.Show("Failed to create adapter.");
                    return;
                }

                PropertiesForm form = new PropertiesForm("Adapter Properties", adapter);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Operator.Adapters.Add(adapter);
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.Error(GeneralHelper.GetExceptionMessage(ex));
                //if (ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false)
                //{
                //    SystemMonitor.Error(ex.InnerException.Message);
                //}
            }
        }

        private void toolStripButtonStartAdapter_Click(object sender, EventArgs e)
        {

            foreach (ListViewItem item in listViewIntegrations.SelectedItems)
            {
                GeneralHelper.FireAndForget(delegate(ListViewItem itemValue)
                {
                    string operationMessage = string.Empty;

                    IIntegrationAdapter adapter = (IIntegrationAdapter)itemValue.Tag;
                    string operationResultMessage;

                    if (Component != null && Component.Platform != null
                        && adapter.Start(Component.Platform, out operationResultMessage) == false)
                    {
                        operationMessage += "Adapter [" + adapter.Name + "] failed to start [" + operationResultMessage + "]." + Environment.NewLine;
                    }

                    WinFormsHelper.BeginManagedInvoke(this, delegate()
                    {
                        if (string.IsNullOrEmpty(operationMessage) == false)
                        {
                            MessageBox.Show(operationMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        UpdateUI();
                    });

                }, item);
            }
        }

        private void toolStripButtonStopAdapter_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewIntegrations.SelectedItems)
            {
                // Make sure to pass item as parameter, otherwise the value is in foreach and chages before the thread starts.
                GeneralHelper.FireAndForget(delegate(ListViewItem itemValue)
                {
                    IIntegrationAdapter adapter = (IIntegrationAdapter)itemValue.Tag;
                    System.Diagnostics.Trace.WriteLine(adapter.Name);
                    string operationResultMessage;
                    adapter.Stop(out operationResultMessage);
                    
                    WinFormsHelper.BeginManagedInvoke(this, UpdateUI);
                }, item);
            }

        }

        private void toolStripButtonRemoveAdapter_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewIntegrations.SelectedItems)
            {
                IIntegrationAdapter adapter = (IIntegrationAdapter)item.Tag;
                if (WinFormsHelper.ShowMessageBox(string.Format("Remove adapter [{0}]?", adapter.Name), string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Operator.Adapters.Remove(adapter);
                }
            }
        }

        private void toolStripButtonAdapterProperties_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewIntegrations.SelectedItems)
            {
                IIntegrationAdapter adapter = (IIntegrationAdapter)item.Tag;
                PropertiesForm form = new PropertiesForm("Adapter Properties", adapter);
                form.ShowOkCancel = false;
                form.ShowDialog();
            }
        }

        private void listViewIntegrations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toolStripButtonStartAdapter_Click(sender, EventArgs.Empty);
        }


    }
}
