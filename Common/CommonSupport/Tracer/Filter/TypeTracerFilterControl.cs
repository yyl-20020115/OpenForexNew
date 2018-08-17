using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    /// <summary>
    /// UI control for managing the Item type based filter.
    /// </summary>
    public partial class TypeTracerFilterControl : UserControl
    {
        TypeTracerFilter _filter = null;
        public TypeTracerFilter Filter
        {
            get { lock (this) { return _filter; } }
            set
            {
                lock (this)
                {
                    _filter = value;
                }
                UpdateUI();
            }
        }

        bool _isUpdating = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TypeTracerFilterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Swap the filter to evade modifying it while loading the elements.
            TypeTracerFilter currentFilter = _filter;
            _filter = null;

            string[] names = Enum.GetNames(typeof(TracerItem.TypeEnum));
            Array values = Enum.GetValues(typeof(TracerItem.TypeEnum));
            for (int i = 0; i < names.Length; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = names[i];
                item.Tag = values.GetValue(i);

                switch ((TracerItem.TypeEnum)item.Tag)
	            {
		            case TracerItem.TypeEnum.MethodEntry:
                        item.ImageIndex = 2;
                     break;
                    case TracerItem.TypeEnum.MethodExit:
                     item.ImageIndex = 3;
                     break;
                    case TracerItem.TypeEnum.Trace:
                     item.ImageIndex = 4;
                     break;
                    case TracerItem.TypeEnum.System:
                     item.ImageIndex = 5;
                     break;
                    case (TracerItem.TypeEnum.Warning | TracerItem.TypeEnum.System):
                    case (TracerItem.TypeEnum.Warning | TracerItem.TypeEnum.Operation):
                    case TracerItem.TypeEnum.Warning:
                     item.ImageIndex = 1;
                     break;
                    case (TracerItem.TypeEnum.Error | TracerItem.TypeEnum.System):
                    case (TracerItem.TypeEnum.Error | TracerItem.TypeEnum.Operation):
                    case TracerItem.TypeEnum.Error:
                     item.ImageIndex = 0;
                     break;
                    default:
                     item.ImageIndex = 4;
                     break;
	            }

                listViewTypes.Items.Add(item);
            }

            // Restore the current filter.
            _filter = currentFilter;

            UpdateUI();
        }

        /// <summary>
        /// Update user interface based on underlying data.
        /// </summary>
        void UpdateUI()
        {
            this.Enabled = _filter != null;

            _isUpdating = true;

            foreach (ListViewItem item in listViewTypes.Items)
            {
                if (_filter != null)
                {
                    item.Checked = _filter.GetItemTypeFiltering((TracerItem.TypeEnum)item.Tag);
                }
                else
                {
                    item.Checked = true;
                }
            }

            _isUpdating = false;
        }

        void UpdateItemTypeFiltering()
        {
            List<TracerItem.TypeEnum> types = new List<TracerItem.TypeEnum>();
            List<bool> filterings = new List<bool>();

            foreach (ListViewItem item in listViewTypes.Items)
            {
                types.Add((TracerItem.TypeEnum)item.Tag);
                filterings.Add(item.Checked);
            }

            if (_filter != null)
            {
                _filter.SetItemTypesFiltering(types.ToArray(), filterings.ToArray());
            }
        }

        private void listViewTypes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_isUpdating == false)
            {
                UpdateItemTypeFiltering();
            }
        }

        private void toolStripButtonCheckAll_Click(object sender, EventArgs e)
        {
            _isUpdating = true;
            foreach (ListViewItem item in this.listViewTypes.Items)
            {
                if (item.Checked == false)
                {
                    item.Checked = true;
                }
            }
            _isUpdating = false;
            UpdateItemTypeFiltering();
        }

        private void toolStripButtonCheckNone_Click(object sender, EventArgs e)
        {
            _isUpdating = true;
            foreach (ListViewItem item in this.listViewTypes.Items)
            {
                if (item.Checked == true)
                {
                    item.Checked = false;
                }
            }
            _isUpdating = false;
            UpdateItemTypeFiltering();
        }

        private void toolStripButtonCheckImportant_Click(object sender, EventArgs e)
        {
            _isUpdating = true;
            
            foreach (ListViewItem item in this.listViewTypes.Items)
            {
                TracerItem.TypeEnum type = (TracerItem.TypeEnum)item.Tag;
                item.Checked = (type & TracerItem.TypeEnum.Error) != 0
                                || (type & TracerItem.TypeEnum.Warning) != 0;
            }

            _isUpdating = false;

            UpdateItemTypeFiltering();
        }
    }
}
