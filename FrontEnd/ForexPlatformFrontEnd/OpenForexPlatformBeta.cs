using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CommonFinancial;
using CommonSupport;
using ForexPlatform;
using ForexPlatformFrontEnd.Properties;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Main UI form class for the Open Forex Platform Application, contains the Platform instance and is the containing form
    /// for all other UI controls.
    /// </summary>
    public partial class OpenForexPlatformBeta : Form
    {
        Platform _platform;
        /// <summary>
        /// Instance of the platform currently loaded at this form. May be null.
        /// </summary>
        public Platform Platform
        {
            get { return _platform; }
        }

        //ApplicationControlAutomationManager _controlAutomationManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenForexPlatformBeta()
        {
            InitializeComponent();

            this.Visible = false;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void OpenForexPlatformBeta_Load(object sender, EventArgs e)
        {
            //_controlAutomationManager = new ApplicationControlAutomationManager(this);
            //_controlAutomationManager.RegisterControlHandler(typeof(Control), new HelpHandler());

            this.Visible = false;

            SplashForm splash = new SplashForm();
            splash.StartPosition = FormStartPosition.CenterScreen;
            splash.Show();
            splash.Refresh();

            combinedContainerControl.ClearControls();
            combinedContainerControl.ImageList = this.imageList;
            combinedContainerControl.SelectedTabbedControlChangedEvent += new CombinedContainerControl.ControlUpdateDelegate(combinedContainerControl_FocusedControlChangedEvent);

            Platform platform = new Platform("DefaultPlatform");
            if (LoadPlatform(platform) == false)
            {// Failed to load the platform.
                this.Close();
            }
            else
            {
                // Precreate all controls before showing anything, to evade unpleasant flickering on startup.
                this.CreateControl();

                Application.DoEvents();
                this.Visible = true;
                this.WindowState = FormWindowState.Maximized;
            }
            
            splash.Hide();

            statusStripToolStripMenuItem.Checked = statusStripMain.Visible;
            toolStripMenuItemFullDiagnosticsMode.Enabled = platform.Settings.DeveloperMode;
        }

        void combinedContainerControl_FocusedControlChangedEvent(CombinedContainerControl containerControl, CommonBaseControl control)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), new GeneralHelper.GenericDelegate<PlatformComponent>(UpdateFormText), control != null ? control.Tag as PlatformComponent : null);
        }

        void combinedContainerControl_ContainedControlRemovedEvent(CombinedContainerControl container, Control control)
        {
            PlatformComponent component = null;
            // First, catch the component before uninitializing the control.
            if (_platform != null && control.Tag != null && control is PlatformComponentControl
                && _platform.GetComponentByIdentification((((PlatformComponentControl)control).Component).SubscriptionClientID.Id, true) != null)
            {
                component = ((PlatformComponentControl)control).Component;
                ((PlatformComponentControl)control).SetApplicationStatusStrip(null);
            }

            if (control is CommonBaseControl)
            {
                (control as CommonBaseControl).SaveState();
            }

            if (component != null)
            {// Remove component from platform.
                if (_platform.IsMandatoryComponent(component) == false)
                {
                    _platform.UnRegisterComponent(component);
                }

                component.UISerializationInfo.AddValue("componentVisible", false);
            }

            if (control is CommonBaseControl)
            {
                (control as CommonBaseControl).UnInitializeControl();
            }

            UpdateComponentsMenues();
        }

        /// <summary>
        /// Handle closing the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenForexPlatformBeta_FormClosing(object sender, FormClosingEventArgs e)
        {
            //GeneralHelper.SetApplicationClosing();

            // Unload everything.
            LoadPlatform(null);
        }

        void SetComponentMenuItemDropDownItems(ToolStripItemCollection items, Type parentingType, 
            bool includeParentingType, string creationTitle, Image creationImage, Type[] constructorTypes)
        {
            if (_platform == null)
            {
                return;
            }

            if (constructorTypes != null)
            {// AddElement types available for creation

                List<Type> resultingTypes = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(parentingType,
                    false, false, ReflectionHelper.GetApplicationEntryAssemblyAndReferencedAssemblies(), constructorTypes);
                if (includeParentingType && parentingType.IsAbstract == false)
                {
                    resultingTypes.Add(parentingType);
                }

                foreach (Type componentType in resultingTypes)
                {
                    if (componentType.IsAbstract)
                    {
                        continue;
                    }

                    string name = parentingType.Name;
                    UserFriendlyNameAttribute.GetTypeAttributeValue(componentType, ref name);

                    // Prepare names for showing in a menu.
                    name = name.Replace("&", "&&");

                    ToolStripMenuItem newItem = new ToolStripMenuItem(creationTitle + name, creationImage);
                    newItem.Tag = componentType;
                    newItem.Click += new EventHandler(createItem_Click);

                    if (_platform.CanAcceptComponent(componentType) == false)
                    {// Maybe component is invisible mandatory, and we should be able to show it.
                        if (ComponentManagementAttribute.GetTypeAttribute(componentType).IsMandatory)
                        {// Look to see if mandatory UI already created.
                            List<PlatformComponent> components = _platform.GetComponentsByType(componentType);
                            foreach (PlatformComponent component in components)
                            {
                                bool visible = false;
                                // For the button to be enabled, component must have assigned visibility flag and also its value to false.
                                newItem.Enabled = component.UISerializationInfo.TryGetValue<bool>("componentVisible", ref visible) 
                                    && visible == false;
                            }
                        }
                        else
                        {
                            newItem.Enabled = false;
                        }
                    }
                    else
                    {
                        newItem.Enabled = true;
                    }

                    items.Add(newItem);
                }
            }

            //// Also check the UI components that are stand alone (have no platform component).
            //foreach (TabPage page in tabControl.TabPages)
            //{
            //    if (((PlatformComponentControl)page.Tag).Component == null ||
            //        ((PlatformComponentControl)page.Tag).Component is PlatformComponent == false)
            //    {
            //        if (parentingType.IsInstanceOfType((CommonBaseControl)page.Tag))
            //        {// Found existing standalone component.
            //            string name = ((CommonBaseControl)page.Tag).Name;
            //            ToolStripMenuItem newItem = new ToolStripMenuItem("Remove " + name, ForexPlatformFrontEnd.Properties.Resources.DELETE2);
            //            newItem.Tag = ((CommonBaseControl)page.Tag);
            //            newItem.Click += new EventHandler(removeItem_Click);
            //            item.DropDownItems.AddElement(newItem);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateComponentsMenues()
        {
            //combinedContainerControl.toolStripDropDownButtonNew.Visible = false;

            combinedContainerControl.toolStripButtonStart.DropDownItems.Clear();

            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(PlatformDiagnostics), true, "Show ", Resources.TV, new Type[] { });

            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(AdapterManagementComponent), true, "Show ", Resources.GEARS, new Type[] { });
            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(DataStoreManagementComponent), true, "Show ", Resources.CABINET, new Type[] { });
            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(ExpertManagementComponent), true, "Show ", Resources.breakpoint_selection, new Type[] { });
            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(SourceManagementComponent), true, "Show ", Resources.cube_molecule, new Type[] { });

            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(PlatformNews), true, "Show ", Resources.environment_information, new Type[] { });

            combinedContainerControl.toolStripButtonStart.DropDownItems.Add(new ToolStripSeparator());

            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(TradePlatformComponent), true, "Create ", Resources.briefcase2_view, new Type[] { });

            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(ManualTradeExpert), true, "Create ", Resources.currency_dollar_grayscale, new Type[] { typeof(ISourceAndExpertSessionManager), typeof(string) });
            //SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(Expert), false, "", Resources.ADD2, null);

            //SetComponentMenuItemDropDownItems(toolStripItemSources.DropDownItems, typeof(PlatformSource), false, "Create ", Resources.ADD2, new Type[] { });

            //combinedContainerControl.toolStripButtonStart.DropDownItems.Add(new ToolStripSeparator());

            //SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(PlatformOperator), false, "Create ", Resources.ADD2, new Type[] { });

            combinedContainerControl.toolStripButtonStart.DropDownItems.Add(new ToolStripSeparator());

            // Wizards are stand alones that execute in separate dialogs.
            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(YahooFinanceAdapter), true, "Create ", Resources.cube_green, new Type[] { typeof(Platform) });

            combinedContainerControl.toolStripButtonStart.DropDownItems.Add(new ToolStripSeparator());

            // Wizards are stand alones that execute in separate dialogs.
            SetComponentMenuItemDropDownItems(combinedContainerControl.toolStripButtonStart.DropDownItems, typeof(WizardControl), false, "Run ", Resources.magic_wand, new Type[] { typeof(Platform) });

            ToolStripMenuItem fileNewMenu = newComponentToolStripMenuItem;
            fileNewMenu.DropDownItems.Clear();

            ToolStripMenuItem toolStripItemPlatform = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Platform");
            toolStripItemPlatform.Image = Resources.CABINET;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem toolStripItemExperts = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Experts");
            toolStripItemExperts.Image = Resources.breakpoint;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem toolStripItemSources = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Sources");
            toolStripItemSources.Image = Resources.cube_green;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem toolStripItemOperators = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Operators");
            toolStripItemOperators.Image = Resources.EXCHANGE;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem toolStripItemCommunity = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Community");
            toolStripItemCommunity.Image = Resources.environment_information;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem toolStripItemTesters = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Testers");
            toolStripItemTesters.Image = Resources.HELP;

            fileNewMenu.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem toolStripItemWizards = (ToolStripMenuItem)fileNewMenu.DropDownItems.Add("Wizards");
            toolStripItemWizards.Image = Resources.magic_wand;

            SetComponentMenuItemDropDownItems(toolStripItemPlatform.DropDownItems, typeof(PlatformDiagnostics), true, "Show ", Resources.TV, new Type[] { });
            SetComponentMenuItemDropDownItems(toolStripItemPlatform.DropDownItems, typeof(ManagementPlatformComponent), true, "Show ", Resources.dot, new Type[] { });

            SetComponentMenuItemDropDownItems(toolStripItemPlatform.DropDownItems, typeof(TradePlatformComponent), true, "Show ", Resources.dot, new Type[] { });
            
            SetComponentMenuItemDropDownItems(toolStripItemExperts.DropDownItems, typeof(Expert), false, "Create ", Resources.ADD2, new Type[] { typeof(ISourceAndExpertSessionManager), typeof(string) });
            //SetComponentMenuItemDropDownItems(toolStripItemExperts, typeof(ExpertHost), false, "Create host ", Resources.ADD2, null);
            
            SetComponentMenuItemDropDownItems(toolStripItemSources.DropDownItems, typeof(PlatformSource), false, "Create ", Resources.ADD2, new Type[] { });

            SetComponentMenuItemDropDownItems(toolStripItemOperators.DropDownItems, typeof(PlatformOperator), false, "Create ", Resources.ADD2, new Type[] { });

            SetComponentMenuItemDropDownItems(toolStripItemCommunity.DropDownItems, typeof(PlatformNews), true, "Create ", Resources.ADD2, new Type[] { });

            // Tester are stand alone UI components added to the tab.
            SetComponentMenuItemDropDownItems(toolStripItemTesters.DropDownItems, typeof(TesterControl), false, "Create ", Resources.ADD2, new Type[] { });

            // Wizards are stand alones that execute in separate dialogs.
            SetComponentMenuItemDropDownItems(toolStripItemWizards.DropDownItems, typeof(WizardControl), false, "Run ", Resources.magic_wand, new Type[] { typeof(Platform) });
        }

        void createItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Type componentType = (Type)item.Tag;

            PlatformComponent component = null;

            bool showPropertiesForm = ComponentManagementAttribute.GetTypeAttribute(componentType).RequestPreStartSetup;
                
            if (ComponentManagementAttribute.GetTypeAttribute(componentType).IsMandatory)
            {// Mandatory components we do not create, only show / hide.
                component = _platform.GetFirstComponentByType(componentType);
                component.UISerializationInfo.AddValue("componentVisible", true);

                platform_ActiveComponentAddedEvent(component, false);
                return;
            }

            if (componentType.IsSubclassOf(typeof(Expert)))
            {
                component = new LocalExpertHost(UserFriendlyNameAttribute.GetTypeAttributeName(componentType), componentType);
                showPropertiesForm = showPropertiesForm || ComponentManagementAttribute.GetTypeAttribute(typeof(LocalExpertHost)).RequestPreStartSetup;
            }
            else if (componentType.IsSubclassOf(typeof(WizardControl)))
            {// Wizards are run in Hosting forms.

                ConstructorInfo info = componentType.GetConstructor(new Type[] { typeof(Platform) });

                if (info != null)
                {
                    WizardControl wizardControl = (WizardControl)info.Invoke(new object[] { _platform });
                    HostingForm hostingForm = new HostingForm(UserFriendlyNameAttribute.GetTypeAttributeName(componentType), wizardControl);
                    hostingForm.Icon = Resources.magic_wand1;
                    hostingForm.Show();
                    return;
                }
            }
            else if (componentType.IsSubclassOf(typeof(CommonBaseControl)))
            {// Tester/editor etc. controls have no components, they are standalone UI components.
                ConstructorInfo info = componentType.GetConstructor(new Type[] { });
                // If failed to find orderInfo, just fall trough to failed to create component (which remains null).
                if (info != null)
                {// Since this is a UI only component, just create and return.
                    CommonBaseControl testerControl = (CommonBaseControl)info.Invoke(new object[] { });
                    /*tabControl.SelectedTab = */AddComponentControl(testerControl, true);

                    return;
                }
            }
            else
            {
                ConstructorInfo info = componentType.GetConstructor(new Type[] { });
                if (info != null)
                {
                    component = (PlatformComponent)info.Invoke(new object[] { });
                }
            }

            // ...
            if (component == null)
            {
                MessageBox.Show("Failed to create component.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set settings for component.
            if (component.SetInitialState(_platform.Settings) == false)
            {
                MessageBox.Show("Component failed to initialize from initial state.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (showPropertiesForm)
            {
                // Show properties for the user to configure.
                PropertiesForm form = new PropertiesForm("Properties", component);
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            // Register to platform.
            _platform.RegisterComponent(component);
        }

        /// <summary>
        /// Central baseMethod, call this to load all user interface related to platform instance.
        /// </summary>
        /// <param name="platform"></param>
        bool LoadPlatform(Platform platform)
        {
            try
            {
                if (_platform != null)
                {
                    _platform.ActiveComponentAddedEvent -= new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentAddedEvent);
                    _platform.ActiveComponentRemovedEvent -= new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentRemovedEvent);
                    _platform.ComponentDeserializationFailedEvent -= new Platform.ComponentDeserializationFailedDelegate(_platform_ComponentDeserializationFailedEvent);
                    _platform.ActiveComponentChangedOperationalStateEvent -= new Platform.ActiveComponentChangedOperationalStateDelegate(_platform_ActiveComponentChangedOperationalStateEvent);

                    combinedContainerControl.ContainedControlRemovedEvent -= new CombinedContainerControl.ControlUpdateDelegate(combinedContainerControl_ContainedControlRemovedEvent);

                    // Save controls ui state (to each controls persistence dataDelivery).
                    combinedContainerControl.SaveState(_platform.UISerializationInfo);

                    _platform.UISerializationInfo.AddValue("MainForm.StatusStripVisible", statusStripMain.Visible);
                    _platform.UISerializationInfo.AddValue("MainForm.ComponentsTabAlignment", (int)this.combinedContainerControl.toolStripMain.Dock);

                    // Before stopping the platform, take down the UIs to allow them to serialize dataDelivery.
                    combinedContainerControl.ClearControls();

                    if (platform == null)
                    {// If this is a final close, hide the form and wait for the long UnInitialize in hidden mode to make for a better UI experience.
                        this.Visible = false;
                        this.Refresh();
                    }

                    SystemMonitor.CheckError(_platform.UnInitialize(), "Failed to uninitialize platform.");
                    _platform = null;
                }

                _platform = platform;

                if (_platform != null)
                {
                    combinedContainerControl.ContainedControlRemovedEvent += new CombinedContainerControl.ControlUpdateDelegate(combinedContainerControl_ContainedControlRemovedEvent);

                    _platform.ActiveComponentAddedEvent += new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentAddedEvent);
                    _platform.ActiveComponentRemovedEvent += new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentRemovedEvent);
                    _platform.ComponentDeserializationFailedEvent += new Platform.ComponentDeserializationFailedDelegate(_platform_ComponentDeserializationFailedEvent);
                    _platform.ActiveComponentChangedOperationalStateEvent += new Platform.ActiveComponentChangedOperationalStateDelegate(_platform_ActiveComponentChangedOperationalStateEvent);

                    if (_platform.Initialize(new PlatformSettings(Settings.Default)) == false)
                    {
                        if (MessageBox.Show("The Platform has failed to initialize. Possible reasons are it is not configured properly or local database is relocated." + System.Environment.NewLine + "Start a new instance?", "Initialization Failed", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                        {// Crash safe - exit procedure.
                            _platform.ActiveComponentAddedEvent -= new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentAddedEvent);
                            _platform.ActiveComponentRemovedEvent -= new Platform.ActiveComponentUpdateDelegate(platform_ActiveComponentRemovedEvent);
                            _platform.ComponentDeserializationFailedEvent -= new Platform.ComponentDeserializationFailedDelegate(_platform_ComponentDeserializationFailedEvent);
                            _platform.ActiveComponentChangedOperationalStateEvent -= new Platform.ActiveComponentChangedOperationalStateDelegate(_platform_ActiveComponentChangedOperationalStateEvent);

                            _platform.DeleteFromPersistence();
                            _platform = null;

                            // Load a clean platform instance.
                            return LoadPlatform(new Platform("DefaultPlatform"));
                        }

                        SystemMonitor.Error("Failed to initialize platform [" + _platform.Name + "].");
                        _platform = null;
                        return false;
                    }

                    if (_platform.UISerializationInfo.ContainsValue("MainForm.StatusStripVisible"))
                    {
                        statusStripMain.Visible = _platform.UISerializationInfo.GetBoolean("MainForm.StatusStripVisible");
                    }

                    if (_platform.UISerializationInfo.ContainsValue("MainForm.ComponentsTabAlignment"))
                    {
                        this.combinedContainerControl.toolStripMain.Dock = (DockStyle)_platform.UISerializationInfo.GetInt32("MainForm.ComponentsTabAlignment");
                    }

                    // Restore controls ui state (to each controls persistence dataDelivery) - AFTER the platform has been initialized.
                    combinedContainerControl.RestoreState(_platform.UISerializationInfo);
                }

                UpdateFormText(null);

                UpdateToolsAndViewMenu();
                UpdateComponentsMenues();
            }
            catch(Exception ex)
            {
                MessageBox.Show("The platform has experienced an unexpected error [" + ex.Message + "].", "Unexpected Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            return true;
        }

        LocalExpertHost localexperthost = null;

        void UpdateFormText(PlatformComponent activeComponent)
        {
            if (activeComponent == null)
            {
                this.Text = "Open Forex Platform [v." + GeneralHelper.ApplicationVersion + "]";
            }
            else
            {
                this.Text = "Open Forex Platform [v." + GeneralHelper.ApplicationVersion + "] - " + activeComponent.Name;
            }
        }

        void UpdateToolsAndViewMenu()
        {
            // Tools menu.
            toolStripMenuItemFullDiagnosticsMode.Checked = false;
            autoRegisterAssembliesToGACToolStripMenuItem.Checked = false;
            registerAssembliesToGACToolStripMenuItem.Enabled = _platform != null;
            unregisterAssembliesFromGACToolStripMenuItem.Enabled = _platform != null;
            
            // View menu.
            statusStripToolStripMenuItem.Checked = statusStripMain.Visible;
            
            if (_platform != null)
            {
                // View menu.
                toolStripMenuItemToolStripTop.Checked = combinedContainerControl.toolStripMain.Dock == DockStyle.Top;
                toolStripMenuItemToolStripRight.Checked = combinedContainerControl.toolStripMain.Dock == DockStyle.Right;
                toolStripMenuItemToolStripLeft.Checked = combinedContainerControl.toolStripMain.Dock == DockStyle.Left;
                toolStripMenuItemToolStripBottom.Checked = combinedContainerControl.toolStripMain.Dock == DockStyle.Bottom;
                noneToolStripMenuItem.Checked = combinedContainerControl.toolStripMain.Dock == DockStyle.None;

                // Tools menu.
                toolStripMenuItemFullDiagnosticsMode.Checked = _platform.Settings.DiagnosticsMode;
            }
        }

        /// <summary>
        /// Helper function to create a new properly assigned tab page for the given component control.
        /// </summary>
        void AddComponentControl(CommonBaseControl componentControl, bool focus)
        {
            combinedContainerControl.AddControl(componentControl);

            if (componentControl is PlatformComponentControl)
            {
                ((PlatformComponentControl)componentControl).SetApplicationStatusStrip(this.statusStripMain);
            }

            if (focus)
            {
                combinedContainerControl.ChangeCheckedControl(componentControl);
            }
        }

        private void tabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            foreach (CommonBaseControl control in e.Control.Controls)
            {// Bring down those manually, since no automatic way was found to do this (windows do not get destroyed until form closing).
                control.UnInitializeControl();
            }
        }

        void _platform_ActiveComponentChangedOperationalStateEvent(Platform platform, PlatformComponent component, OperationalStateEnum previousState)
        {
            //WinFormsHelper.BeginFilteredManagedInvoke(this, new EventHandler(tabControl_SelectedIndexChanged), null, EventArgs.Empty);
        }

        void uiThread_ActiveComponentUpdateEvent(PlatformComponent component, bool added, bool isInitial)
        {
            if (added)
            {
                bool visible = true;
                if (component.UISerializationInfo.ContainsValue("componentVisible"))
                {
                    visible = component.UISerializationInfo.GetBoolean("componentVisible");
                }

                if (visible)
                {
                    CommonBaseControl control = CommonBaseControl.CreateCorrespondingControl(component, true);
                    AddComponentControl(control, isInitial == false);
                }
            }
            else
            {
                CommonBaseControl control = combinedContainerControl.GetControlByTag(component);
                if (control != null)
                {
                    combinedContainerControl.RemoveControl(control);
                }
            }

            UpdateComponentsMenues();
        }

        void platform_ActiveComponentAddedEvent(PlatformComponent component, bool isInitial)
        {
            WinFormsHelper.BeginManagedInvoke(this, new GeneralHelper.GenericDelegate<PlatformComponent, bool, bool>(
                uiThread_ActiveComponentUpdateEvent), component, true, isInitial);
        }

        void platform_ActiveComponentRemovedEvent(PlatformComponent component, bool isInitial)
        {
            WinFormsHelper.BeginManagedInvoke(this, new GeneralHelper.GenericDelegate<PlatformComponent, bool, bool>(
                uiThread_ActiveComponentUpdateEvent), component, false, isInitial);
        }

        void _platform_ComponentDeserializationFailedEvent(long componentId, string componentTypeName)
        {
            WinFormsHelper.BeginManagedInvoke(this, new GeneralHelper.GenericDelegate<long, string>(
                uiThread_ComponentDeSerializationFailed), componentId, componentTypeName);
        }

        private void uiThread_ComponentDeSerializationFailed(long componentId, string componentTypeName)
        {
            if (WinFormsHelper.ShowMessageBox("A component [id " + componentId.ToString() + ", type " + componentTypeName + "] has failed on deserialization or initialization, " + System.Environment.NewLine + "would you like to remove it?", 
                "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                _platform.RemovePersistedComponentById(componentId);
            }
        }

        private void toolStripLabelAdds_MouseEnter(object sender, EventArgs e)
        {
            //toolStripLabelRemove.Image = ForexPlatformFrontEnd.Properties.Resources.button_cancel_12_b;
        }

        private void toolStripLabelAdds_MouseLeave(object sender, EventArgs e)
        {
            //toolStripLabelRemove.Image = ForexPlatformFrontEnd.Properties.Resources.button_cancel_12;
        }

        private void consecutiveTradesTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //void itemReload_Click(object sender, EventArgs e)
        //{
        //    if (tabControl.SelectedTab != null)
        //    {
        //        if (tabControl.SelectedTab.Tag is PlatformComponentControl)
        //        {// Platform component - remote trough platform.
        //            _platform.InitializeComponent(((PlatformComponentControl)(tabControl.SelectedTab.Tag)).Component as PlatformComponent);
        //            tabControl.SelectedTab.Enabled = true;
        //        }
        //    }
        //}

        //void itemUnload_Click(object sender, EventArgs e)
        //{
        //    if (tabControl.SelectedTab != null)
        //    {
        //        if (tabControl.SelectedTab.Tag is PlatformComponentControl)
        //        {// Platform component - remote trough platform.
        //            _platform.UnInitializeComponent(((PlatformComponentControl)(tabControl.SelectedTab.Tag)).Component as PlatformComponent);
        //            tabControl.SelectedTab.Enabled = false;
        //        }
        //    }
        //}

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open Forex Platform" + System.Environment.NewLine + "version." + GeneralHelper.ApplicationVersion + System.Environment.NewLine + "www.openforexplatform.com", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenForexPlatformBeta_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void toolStripMenuItemFullDiagnosticsMode_Click(object sender, EventArgs e)
        {
            if (_platform == null)
            {
                return;
            }

            _platform.Settings.DiagnosticsMode = !_platform.Settings.DiagnosticsMode;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GeneralHelper.RunUrl("http://www.openforexplatform.com/wiki/");
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.combinedContainerControl.toolStripMain.Dock = DockStyle.None;
            UpdateToolsAndViewMenu();
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.combinedContainerControl.toolStripMain.Dock = DockStyle.Top;
            UpdateToolsAndViewMenu();
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.combinedContainerControl.toolStripMain.Dock = DockStyle.Right;
            UpdateToolsAndViewMenu();
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.combinedContainerControl.toolStripMain.Dock = DockStyle.Left;
            UpdateToolsAndViewMenu();
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.combinedContainerControl.toolStripMain.Dock = DockStyle.Bottom;
            UpdateToolsAndViewMenu();
        }

        private void menuItemView_DropDownOpening(object sender, EventArgs e)
        {
            componentsTabTitlesToolStripMenuItem.Checked = combinedContainerControl.ShowComponentsTabsTitles;
        }

        private void componentsTabTitlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (componentsTabTitlesToolStripMenuItem.Checked != combinedContainerControl.ShowComponentsTabsTitles)
            {
                combinedContainerControl.ShowComponentsTabsTitles = componentsTabTitlesToolStripMenuItem.Checked;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItemResetUI_Click(object sender, EventArgs e)
        {
            combinedContainerControl.ResetControlPlacement();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void statusStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStripMain.Visible = statusStripToolStripMenuItem.Checked;

            UpdateToolsAndViewMenu();
        }

        private void removeComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            combinedContainerControl.RemoveCurrentCheckedControl();
        }

        private void loadAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Assemblies (*.exe; *.dll)|*.exe;*.dll";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(ofd.FileName);
                    IndicatorFactory.Instance.CollectCustomIndicatorsFromAssembly(assembly);
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError("Failed to load external assembly", ex);
                }
            }
        }


    }
}
