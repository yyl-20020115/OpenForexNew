using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ForexPlatform;
using CommonSupport;
using System.Reflection;
using System.IO;

namespace ForexPlatformFrontEnd
{
    public partial class ExpertManagementControl : PlatformComponentControl
    {
        public ExpertManagementComponent ExpertManager
        {
            get { return (ExpertManagementComponent)base.Component; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExpertManagementControl(ExpertManagementComponent expertManager)
            : base(expertManager)
        {
            InitializeComponent();

            this.Name = UserFriendlyNameAttribute.GetTypeAttributeName(typeof(ExpertManagementComponent));

            expertManager.AddedExpertContainerEvent += new ExpertManagementComponent.ExpertContainerUpdateDelegate(expertManager_AddedExpertContainerEvent);
            expertManager.RemovedExpertContainerEvent += new ExpertManagementComponent.ExpertContainerUpdateDelegate(expertManager_RemovedExpertContainerEvent);
            expertManager.ExpertAssemblyAddedEvent += new ExpertManagementComponent.ExpertAssemblyAddedDelegate(expertManager_ExpertAssemblyAddedEvent);
            expertManager.ExpertAssemblyRemovedEvent += new ExpertManagementComponent.ExpertAssemblyAddedDelegate(expertManager_ExpertAssemblyRemovedEvent);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            tabControlExperts.TabPages.Clear();

            lock (virtualListViewExAssemblies)
            {
                virtualListViewExAssemblies.AdvancedColumnManagementUnsafe.Add(0, new VirtualListViewEx.ColumnManagementInfo() { FillWhiteSpace = true });
            }

            UpdateUI(true);
        }

        public override void UnInitializeControl()
        {
            if (ExpertManager != null)
            {
                ExpertManager.AddedExpertContainerEvent -= new ExpertManagementComponent.ExpertContainerUpdateDelegate(expertManager_AddedExpertContainerEvent);
                ExpertManager.RemovedExpertContainerEvent -= new ExpertManagementComponent.ExpertContainerUpdateDelegate(expertManager_RemovedExpertContainerEvent);
                ExpertManager.ExpertAssemblyAddedEvent -= new ExpertManagementComponent.ExpertAssemblyAddedDelegate(expertManager_ExpertAssemblyAddedEvent);
                ExpertManager.ExpertAssemblyRemovedEvent -= new ExpertManagementComponent.ExpertAssemblyAddedDelegate(expertManager_ExpertAssemblyRemovedEvent);
            }

            base.UnInitializeControl();

            UpdateUI(true);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI(bool updateExpertList)
        {
            if (ExpertManager == null)
            {
                listViewExperts.Enabled = false;
                listViewExperts.Items.Clear();
                virtualListViewExAssemblies.VirtualListSize = 0;
                virtualListViewExAssemblies.Enabled = false;
                return;
            }

            toolStripButtonEditExpert.Enabled = false;
            toolStripButtonDeleteExpert.Enabled = false;
            toolStripDropDownButtonRunExpert.Enabled = false;

            ExpertInformation expertContainer = null;
            if (listViewExperts.SelectedIndices.Count != 0)
            {
                expertContainer = (ExpertInformation)listViewExperts.SelectedItems[0].Tag;
                toolStripButtonEditExpert.Enabled = !expertContainer.IsExternal;
                toolStripDropDownButtonRunExpert.Enabled = true;
                toolStripButtonDeleteExpert.Enabled = true;
            }

            if (updateExpertList)
            {

                int index = 0;
                foreach (ExpertInformation container in ExpertManager.ExpertInfosArray)
                {
                    ListViewItem item;
                    if (listViewExperts.Items.Count <= index)
                    {
                        item = listViewExperts.Items.Add("");
                    }
                    else
                    {
                        item = listViewExperts.Items[index];
                    }

                    item.Name = container.Name;
                    item.Text = container.Name;
                    item.Tag = container;
                    if (container.IsExternal)
                    {
                        item.Group = listViewExperts.Groups["Imported"];
                    }
                    else
                    {
                        item.Group = listViewExperts.Groups["Local"];
                    }

                    index++;
                }

                while (listViewExperts.Items.Count > index)
                {
                    listViewExperts.Items.RemoveAt(listViewExperts.Items.Count - 1);
                }
            }

            virtualListViewExAssemblies.VirtualListSize = ExpertManager.ExpertsAssembliesArray.Length;
            virtualListViewExAssemblies.Refresh();
            virtualListViewExAssemblies.UpdateColumnWidths();
        }


        private void toolStripButtonExpertList_CheckStateChanged(object sender, EventArgs e)
        {
            panelExperts.Visible = toolStripButtonExpertList.Checked;
            splitterLeft.Visible = panelExperts.Visible;
        }

        private void toolStripButtonImportedDlls_CheckStateChanged(object sender, EventArgs e)
        {
            virtualListViewExAssemblies.Visible = toolStripButtonImportedAssemblies.Checked;
        }

        private void virtualListViewExExperts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI(false);
        }

        private void toolStripButtonNewExpert_Click(object sender, EventArgs e)
        {
            string filesFolder = Component.Platform.Settings.GetMappedPath("FilesFolder");
            string operationResultMessage;
            if (ExpertManager.CreateExpertFromFile(Path.Combine(filesFolder, "DefaultManagedExpert.cs"), "New Expert", out operationResultMessage) == false)
            {
                MessageBox.Show(operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void expertManager_AddedExpertContainerEvent(ExpertInformation container)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), new GeneralHelper.GenericDelegate<bool>(UpdateUI), true);

            GeneralHelper.DefaultDelegate newDelegate = delegate() 
                {
                    // Select the newly created expert.
                    this.listViewExperts.SelectedIndices.Clear();
                    if (listViewExperts.VirtualListSize > 0)
                    {
                        this.listViewExperts.SelectedIndices.Add(listViewExperts.VirtualListSize - 1);
                    }
                };

            WinFormsHelper.BeginManagedInvoke(this, newDelegate);
        }

        void expertManager_RemovedExpertContainerEvent(ExpertInformation container)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), new GeneralHelper.GenericDelegate<bool>(UpdateUI), true);
        }

        void expertManager_ExpertAssemblyRemovedEvent(Assembly assembly)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), new GeneralHelper.GenericDelegate<bool>(UpdateUI), true);
        }

        void expertManager_ExpertAssemblyAddedEvent(Assembly assembly)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), new GeneralHelper.GenericDelegate<bool>(UpdateUI), true);
        }

        private void virtualListViewExDlls_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (ExpertManager == null)
            {
                return;
            }
            
            if (e.ItemIndex < ExpertManager.ExpertsAssembliesArray.Length)
            {
                Assembly assembly = ExpertManager.ExpertsAssembliesArray[e.ItemIndex];
                e.Item = new ListViewItem(new string[] { assembly.FullName });
            }
        }

        private void toolStripButtonImportAssembly_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Assemblies (*.exe; *.dll)|*.exe;*.dll";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                //if (MessageBox.Show("Use assembly local copy?", "Select", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    string errorMessage;
                //    if (ExpertManagementComponent.CopyAssembllyLocally(path, out path, out errorMessage) == false)
                //    {
                //        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return;
                //    }
                //}

                if (ExpertManager.AddAssembly(path) == false)
                {
                    MessageBox.Show("Failed to add assembly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void virtualListViewExAssemblies_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (virtualListViewExAssemblies.SelectedIndices.Count > 0)
                {
                    ExpertManager.RemoveAssembly(ExpertManager.ExpertsAssembliesArray[virtualListViewExAssemblies.SelectedIndices[0]]);
                }
            }
        }

        private void runStandaloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewExperts.SelectedItems.Count != 1)
            {
                return;
            }

            ExpertInformation info = (ExpertInformation)listViewExperts.SelectedItems[0].Tag;

            string operationMessage = string.Empty;
            Type expertType = info.GetExpertType(ref operationMessage);
            if (expertType == null)
            {
                MessageBox.Show("Failed to create expert [" + operationMessage + "]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LocalExpertHost expertHost = new LocalExpertHost(info.Name, expertType);
            expertHost.Name = expertHost.Name;
            ExpertManager.Platform.RegisterComponent(expertHost);
        }

        private void toolStripButtonDeleteExpert_Click(object sender, EventArgs e)
        {
            if (listViewExperts.SelectedItems.Count != 1)
            {
                return;
            }

            ExpertInformation expertInfo = (ExpertInformation)listViewExperts.SelectedItems[0].Tag;

            if (MessageBox.Show("Delete expert [" + expertInfo.Name + "]. Are you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            // Remove page if in edit.
            foreach (TabPage page in tabControlExperts.TabPages)
            {
                ExpertEditorControl control = (ExpertEditorControl)page.Controls[0];
                control.ExpertUpdateEvent -= new ExpertEditorControl.ExpertUpdatedDelegate(editorControl_ExpertUpdateEvent);

                if (control.ExpertInformation == expertInfo)
                {// Already editting it.
                    tabControlExperts.TabPages.Remove(page);
                    break;
                }
            }

            if (expertInfo.IsExternal == false)
            {
                if (MessageBox.Show("Delete expert local (" + Path.GetFileName(expertInfo.FilePath) + ") file?", "Delete file?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    expertInfo.DeleteLocal();
                }
            }

            ExpertManager.RemoveExpert(expertInfo);
            UpdateUI(true);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (virtualListViewExAssemblies.SelectedIndices.Count > 0)
            {
                ExpertManager.RemoveAssembly(ExpertManager.ExpertsAssembliesArray[virtualListViewExAssemblies.SelectedIndices[0]]);
            }
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (listViewExperts.SelectedItems.Count != 1)
            {
                return;
            }

            ExpertInformation info = (ExpertInformation)listViewExperts.SelectedItems[0].Tag;

            foreach (TabPage page in tabControlExperts.TabPages)
            {
                ExpertEditorControl control = (ExpertEditorControl)page.Controls[0];
                if (control.ExpertInformation == info)
                {// Already editting it.
                    tabControlExperts.SelectedTab = page;
                    return;
                }
            }

            ExpertEditorControl editorControl = new ExpertEditorControl();
            editorControl.ExpertInformation = info;
            editorControl.ExpertUpdateEvent += new ExpertEditorControl.ExpertUpdatedDelegate(editorControl_ExpertUpdateEvent);
            TabPage newPage = new TabPage(info.Name);
            editorControl.Dock = DockStyle.Fill;
            newPage.Controls.Add(editorControl);
            tabControlExperts.TabPages.Add(newPage);
            tabControlExperts.SelectedTab = newPage;
        }

        void editorControl_ExpertUpdateEvent(ExpertEditorControl editorControl)
        {
            this.UpdateUI(true);
        }

        private void toolStripLabelRemove_MouseEnter(object sender, EventArgs e)
        {
            toolStripLabelRemove.Image = ForexPlatformFrontEnd.Properties.Resources.button_cancel_12_b;
        }

        private void toolStripLabelRemove_MouseLeave(object sender, EventArgs e)
        {
            toolStripLabelRemove.Image = ForexPlatformFrontEnd.Properties.Resources.button_cancel_12;
        }

        private void toolStripLabelRemove_Click(object sender, EventArgs e)
        {
            if (tabControlExperts.SelectedIndex > -1)
            {
                ExpertInformation info = ((ExpertEditorControl)tabControlExperts.SelectedTab.Controls[0]).ExpertInformation;
                if (info.IsSavedLocally == false)
                {
                    DialogResult result = MessageBox.Show("Expert [" + info.Name + "] not saved. Save changes now?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {// Yes.
                        info.SaveLocal();
                    }
                    else if (result == DialogResult.No)
                    {// No, just continue.
                    }
                    else
                    {// Cancel operation.
                        return;
                    }
                }

                ExpertEditorControl control = (ExpertEditorControl)tabControlExperts.SelectedTab.Controls[0];
                control.ExpertUpdateEvent -= new ExpertEditorControl.ExpertUpdatedDelegate(editorControl_ExpertUpdateEvent);
                tabControlExperts.TabPages.Remove(tabControlExperts.SelectedTab);
            }
        }

        private void listViewExperts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (toolStripButtonEditExpert.Enabled)
            {// On double click edit expert source, if applicable.
                toolStripButtonEdit_Click(sender, EventArgs.Empty);
            }
        }

        private void toolStripButtonSourceFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Source File (*.cs)|*.cs";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string operationResultMessage;
            if (ExpertManager.CreateExpertFromFile(ofd.FileName, Path.GetFileNameWithoutExtension(ofd.FileName), out operationResultMessage) == false)
            {
                MessageBox.Show(operationResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {// Show a tab with the source of the newly added expert.
                listViewExperts.SelectedIndices.Clear();
                listViewExperts.SelectedIndices.Add(listViewExperts.Items.Count - 1);
                toolStripButtonEdit_Click(sender, e);
            }
        }


    }
}
