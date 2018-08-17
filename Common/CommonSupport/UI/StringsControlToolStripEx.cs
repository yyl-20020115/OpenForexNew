using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CommonSupport;

namespace CommonSupport
{
    /// <summary>
    /// Advanced toolstrip for managing strings, with add and remove buttons.
    /// </summary>
    public class StringsControlToolStripEx : ToolStrip
    {
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripSplitButton toolStripButtonRemove;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;

        /// <summary>
        /// Is the control operating in single string mode.
        /// </summary>
        public bool SingleStringMode
        {
            get
            {
                return _dataSource != null && _dataSourceProperty != null
                    && _dataSourceProperty.PropertyType == typeof(string);
            }
        } 

        volatile string _label = string.Empty;
        /// <summary>
        /// Text on the start label of control.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// Values, format enumerable.
        /// </summary>
        public IEnumerable<string> ValuesEnumerable
        {
            get
            {
                foreach(object item in toolStripComboBox.Items)
                {
                    yield return item.ToString();
                }
            }

        }

        /// <summary>
        /// Values, in string list.
        /// </summary>
        public List<string> Values
        {
            get
            {
                return GeneralHelper.EnumerableToList<string>(ValuesEnumerable);
            }

            set
            {
                toolStripComboBox.Items.Clear();

                if (value != null)
                {
                    foreach (string item in value)
                    {
                        toolStripComboBox.Items.Add(item);
                    }
                }

                UpdateUI();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentValue
        {
            get { return toolStripComboBox.Text; }
            set { toolStripComboBox.Text = value; }
        }

        object _dataSource = null;
        /// <summary>
        /// 
        /// </summary>
        public object DataSource
        {
            get { return _dataSource; }
        }

        PropertyInfo _dataSourceProperty = null;
        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo DataSourceProperty
        {
            get { return _dataSourceProperty; }
        }

        public delegate void ToolStripUpdateDelegate(ToolStrip toolStrip);
        public event ToolStripUpdateDelegate ValuesUpdatedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StringsControlToolStripEx()
        {
            InitializeComponent();

            toolStripComboBox.KeyDown += new KeyEventHandler(toolStripComboBox_KeyDown);
            toolStripButtonRemove.Click += new EventHandler(toolStripButtonRemove_Click);
            toolStripButtonAdd.Click += new EventHandler(toolStripButtonAdd_Click);
            deleteAllToolStripMenuItem.Click += new EventHandler(deleteAllToolStripMenuItem_Click);
            ValuesUpdatedEvent += new ToolStripUpdateDelegate(StringsControlToolStripEx_ValuesUpdatedEvent);

            UpdateUI();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringsControlToolStripEx));

            this.toolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonRemove = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.Dock = System.Windows.Forms.DockStyle.None;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox,
            this.toolStripButtonAdd,
            this.toolStripButtonRemove});
            this.Location = new System.Drawing.Point(124, 180);
            this.Name = "toolStrip1";
            this.Size = new System.Drawing.Size(263, 25);
            this.TabIndex = 0;
            this.Text = "toolStrip1";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox.Name = "toolStripComboBox1";
            this.toolStripComboBox.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButtonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteAllToolStripMenuItem});
            this.toolStripButtonRemove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemove")));
            this.toolStripButtonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemove.Name = "toolStripButton1";
            this.toolStripButtonRemove.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonRemove.Text = "Delete / Remove";
            this.toolStripButtonRemove.ToolTipText = "Delete / Remove";
            this.toolStripButtonRemove.DisplayStyle = ToolStripItemDisplayStyle.Image;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Enabled = true;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel1.Text = "";
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            // 
            // toolStripButton2
            // 
            this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd")));
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Add / Assign";
            this.toolStripButtonAdd.ToolTipText = "Add / Assign";
            this.toolStripButtonAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(609, 307);
            this.Name = "Form1";
            this.Text = "Form1";
            this.PerformLayout();
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDataSource(Object dataSource, PropertyInfo info)
        {
            _dataSource = dataSource;
            _dataSourceProperty = info;

            LoadValues();
        }

        /// <summary>
        /// Load values from data source.
        /// </summary>
        public void LoadValues()
        {
            if (_dataSource != null && _dataSourceProperty != null
                && _dataSourceProperty.CanRead)
            {
                if (_dataSourceProperty.PropertyType == typeof(List<string>))
                {
                    Values = (List<string>)_dataSourceProperty.GetValue(_dataSource, null);
                }
                else if (_dataSourceProperty.PropertyType == typeof(IEnumerable<string>))
                {
                    Values = new List<string>((IEnumerable<string>)_dataSourceProperty.GetValue(_dataSource, null));
                }
                else if (_dataSourceProperty.PropertyType == typeof(string[]))
                {
                    Values = new List<string>((string[])_dataSourceProperty.GetValue(_dataSource, null));
                }
                else if (SingleStringMode)
                {
                    Values = new List<string>(new string[] { (string)_dataSourceProperty.GetValue(_dataSource, null) });
                }
            }
        }

        /// <summary>
        /// Update values to data source.
        /// </summary>
        void UpdateValues()
        {
            if (_dataSource != null && _dataSourceProperty != null
                && _dataSourceProperty.CanWrite)
            {
                if (_dataSourceProperty.PropertyType == typeof(List<string>))
                {
                    _dataSourceProperty.SetValue(_dataSource, Values, null);
                }
                else if (_dataSourceProperty.PropertyType == typeof(IEnumerable<string>))
                {
                    _dataSourceProperty.SetValue(_dataSource, Values, null);
                }
                else if (_dataSourceProperty.PropertyType == typeof(string[]))
                {
                    _dataSourceProperty.SetValue(_dataSource, Values.ToArray(), null);
                }
                else if (SingleStringMode)
                {
                    _dataSourceProperty.SetValue(_dataSource, CurrentValue, null);
                }
            }
        }

        /// <summary>
        /// Update the user interface elements based on the underlying data.
        /// </summary>
        void UpdateUI()
        {
            string labelText = string.Empty;
            if (string.IsNullOrEmpty(_label) == false)
            {
                labelText = _label;
            }

            Color labelColor = SystemColors.ControlText;
            if (SingleStringMode)
            {
                if (Values.Count != 0 && string.IsNullOrEmpty(Values[0]) == false)
                {
                    labelColor = Color.DarkRed;
                }
            }
            else
            {
                labelText += "[" + Values.Count + "]";

                if (Values.Count > 0)
                {
                    labelColor = Color.DarkRed;
                }

            }

            if (toolStripLabel1.Text != labelText)
            {
                toolStripLabel1.Text = labelText;
            }

            if (toolStripLabel1.ForeColor != labelColor)
            {
                toolStripLabel1.ForeColor = labelColor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        void StringsControlToolStripEx_ValuesUpdatedEvent(ToolStrip toolStrip)
        {
            UpdateUI();
            UpdateValues();
        }

        /// <summary>
        /// Helper.
        /// </summary>
        void RaiseValuesUpdatedEvent()
        {
            if (ValuesUpdatedEvent != null)
            {
                ValuesUpdatedEvent(this);
            }
        }

        void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (Values.Contains(CurrentValue) == false && string.IsNullOrEmpty(CurrentValue) == false)
            {
                if (SingleStringMode)
                {
                    toolStripComboBox.Items.Clear();
                    toolStripComboBox.Items.Add(CurrentValue);
                }
                else
                {
                    toolStripComboBox.Items.Add(CurrentValue);
                    CurrentValue = string.Empty;
                }

                RaiseValuesUpdatedEvent();
            }
        }

        void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox.Items.Count > 0)
            {
                toolStripComboBox.Items.Clear();
                RaiseValuesUpdatedEvent();
            }
        }

        void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            if (SingleStringMode)
            {
                toolStripComboBox.Items.Clear();
                CurrentValue = string.Empty;
                RaiseValuesUpdatedEvent();
            }
            else if (Values.Contains(CurrentValue) && string.IsNullOrEmpty(CurrentValue) == false)
            {
                toolStripComboBox.Items.Remove(CurrentValue);
                RaiseValuesUpdatedEvent();
            }
            else if (string.IsNullOrEmpty(CurrentValue) && Values.Count == 1)
            { // Special mode, will clear single value when its inly one.
                toolStripComboBox.Items.Clear();
                CurrentValue = string.Empty;
                RaiseValuesUpdatedEvent();
            }
        }

        void toolStripComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripButtonAdd_Click(sender, e);
            }
        }


    }
}
