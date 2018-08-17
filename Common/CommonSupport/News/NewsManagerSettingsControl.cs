using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Rss;

namespace CommonSupport
{
    public partial class NewsManagerSettingsControl : UserControl
    {
        NewsManagerControl _control;
        List<Type> _preconfiguredTypes = new List<Type>();

        bool _operational = false;

        /// <summary>
        /// 
        /// </summary>
        public NewsManagerSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public NewsManagerSettingsControl(NewsManagerControl control)
        {
            InitializeComponent();
            _control = control;
        }

        private void NewsManagerSettingsControl_Load(object sender, EventArgs e)
        {
            List<Type> types = ReflectionHelper.GatherTypeChildrenTypesFromAssemblies(typeof(EventSource), ReflectionHelper.GetApplicationEntryAssemblyReferencedAssemblies());
            // Gather suitable news source types, evading the RssNewsSource, since it is up for dynamic creation.
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(typeof(EventItemTypeAttribute), false);
                if (type != typeof(RssNewsSource) && attributes != null &&
                    ((EventItemTypeAttribute)attributes[0]).TypeValue == typeof(RssNewsEvent))
                {
                    comboBoxPreconfigured.Items.Add(type.Name);
                    _preconfiguredTypes.Add(type);
                }
            }

            UpdateUI();

        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            _operational = false;

            numericUpDownMaxItemsCount.Enabled = _control != null && _control.Manager != null;
            numericUpDownUpdateInterval.Enabled = _control != null && _control.Manager != null;
            listViewFeeds.Enabled = _control != null && _control.Manager != null;
            buttonAdd.Enabled = _control != null && _control.Manager != null;
            buttonDelete.Enabled = _control != null && _control.Manager != null;
            checkBoxAutoUpdate.Enabled = _control != null && _control.Manager != null;

            //int selectedFeedIndex = -1;
            //if (listViewFeeds.SelectedItems.Count > 0)
            //{
            //    selectedFeedIndex = listViewFeeds.SelectedIndices[0];
            //}

            listViewFeeds.Items.Clear();
            if (_control == null || _control.Manager == null)
            {
                return;
            }

            numericUpDownMaxItemsCount.Value = _control.MaximumItemsShown;
            
            // This must be before the numeric value.
            checkBoxAutoUpdate.Checked = _control.Manager.AutoUpdateEnabled;
            numericUpDownUpdateInterval.Value = (decimal)_control.Manager.AutoUpdateInterval.TotalSeconds;

            foreach (EventSource source in _control.Manager.NewsSourcesUnsafe)
            {
                string title = source.Name;
                ListViewItem item = new ListViewItem(new string[] { title, source.Address });
                item.Checked = source.Enabled;
                listViewFeeds.Items.Add(item);
            }

            //if (selectedFeedIndex > -1 && selectedFeedIndex < listViewFeeds.Items.Count)
            //{// Restore selection.
            //    listViewFeeds.SelectedIndices.Add(selectedFeedIndex);
            //}

            if (listViewFeeds.Items.Count > 0)
            {
                foreach (ColumnHeader header in listViewFeeds.Columns)
                {
                    header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }

            _operational = true;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxAdd.Text))
            {
                return;
            }

            RssNewsSource source = new RssNewsSource();
            source.Initialize(this.textBoxAdd.Text);
            source.Update();
            if (source.OperationalState != OperationalStateEnum.Operational)
            {
                MessageBox.Show("Failed to find source.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _control.Manager.AddSource(source);
                textBoxAdd.Text = "";
                UpdateUI();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewFeeds.SelectedIndices.Count == 0)
            {
                return;
            }

            if (MessageBox.Show("Deleting a source will delete it and all its items." + Environment.NewLine + "The data can not be restored, are you sure?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }

            List<EventSource> sources = new List<EventSource>();
            foreach (int index in listViewFeeds.SelectedIndices)
            {
                sources.Add(_control.Manager.NewsSourcesUnsafe[index]);
            }
            foreach(EventSource source in sources)
            {
                _control.Manager.RemoveSource(source);
            }

            UpdateUI();
        }

        private void numericUpDownMaxItemsCount_ValueChanged(object sender, EventArgs e)
        {
            _control.MaximumItemsShown = (int)numericUpDownMaxItemsCount.Value;
            _control.UpdateUI();
        }

        private void updateInterval_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownUpdateInterval.Enabled = checkBoxAutoUpdate.Checked;
            _control.Manager.AutoUpdateEnabled = checkBoxAutoUpdate.Checked;
            if (checkBoxAutoUpdate.Checked)
            {
                _control.Manager.AutoUpdateInterval = TimeSpan.FromSeconds((double)numericUpDownUpdateInterval.Value);
            }

            _control.UpdateUI();
        }

        private void buttonAddPreconfigured_Click(object sender, EventArgs e)
        {
            if (comboBoxPreconfigured.SelectedIndex < 0)
            {
                return;
            }

            Type type = _preconfiguredTypes[comboBoxPreconfigured.SelectedIndex];
            EventSource newSource = (EventSource)Activator.CreateInstance(type);
            
            foreach (EventSource source in _control.Manager.NewsSourcesUnsafe)
            {
                if (source.GetType() == newSource.GetType())
                {
                    MessageBox.Show("A source of this type already created (only one instance of preconfigured types allowed).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _control.Manager.AddSource(newSource);
            UpdateUI();
        }

        private void listViewFeeds_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_operational)
            {
                _control.Manager.NewsSourcesUnsafe[e.Item.Index].Enabled = !_control.Manager.NewsSourcesUnsafe[e.Item.Index].Enabled;
                _control.UpdateUI();
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            if (listViewFeeds.SelectedIndices.Count == 0)
            {
                return;
            }

            if (listViewFeeds.SelectedIndices.Count > 1)
            {
                MessageBox.Show("Select only one feed to show settings for.");
                return;
            }

            EventSource source = _control.Manager.NewsSourcesUnsafe[listViewFeeds.SelectedIndices[0]];
            NewsSourceSettingsControl control = new NewsSourceSettingsControl(source);
            HostingForm form = new HostingForm("Source Settings", control);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();

            _control.UpdateUI();
        }

    }
}
