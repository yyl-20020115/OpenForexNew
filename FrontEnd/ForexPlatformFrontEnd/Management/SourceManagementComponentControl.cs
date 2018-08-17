using System;
using System.Windows.Forms;
using CommonSupport;
using ForexPlatform;
using CommonFinancial;
using System.Collections.Generic;

namespace ForexPlatformFrontEnd
{
    /// <summary>
    /// Control visualizes properties for the MT4 integration component.
    /// </summary>
    public partial class SourceManagementComponentControl : PlatformComponentControl
    {
        /// <summary>
        /// The MT4 integration operator component.
        /// </summary>
        SourceManagementComponent Operator
        {
            get { return (SourceManagementComponent)base.Component; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SourceManagementComponentControl()
            : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Functional constructor.
        /// </summary>
        public SourceManagementComponentControl(SourceManagementComponent integrationOperator)
            : base(integrationOperator)
        {
            InitializeComponent();

            this.Name = Operator.Name;

            Operator.SourcesUpdateEvent += new SourceManagementComponent.SourcesUpdateDelegate(Operator_SourcesUpdateEvent);

            // We need to create early.
            this.Tag = integrationOperator;
            this.CreateControl();
        }

        void Operator_SourcesUpdateEvent(SourceManagementComponent sourceOperator)
        {
            WinFormsHelper.BeginFilteredManagedInvoke(this, TimeSpan.FromMilliseconds(250), UpdateUI);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateUI();
        }

        void UpdateUI()
        {
            listViewSources.Items.Clear();
            if (Operator == null || Operator.Sources == null)
            {
                return;
            }

            foreach (SourceInfo source in Operator.Sources.SourcesArray)
            {
                ListViewItem item = new ListViewItem();
                SetItemsAsSource(item, source);
                listViewSources.Items.Add(item);
            }

            listViewSources.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewSources.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewSources.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        void SetItemsAsSource(ListViewItem item, SourceInfo source)
        {
            item.SubItems.Clear();
            item.Text = source.ComponentId.Name;
            
            if (source.ComponentId.IdentifiedComponentType != null)
            {
                item.SubItems.Add(source.ComponentId.IdentifiedComponentType.Name);
            }
            else
            {
                item.SubItems.Add("NA");
            }
            
            item.SubItems.Add(GeneralHelper.GetCombinedEnumName(typeof(SourceTypeEnum), (int)source.SourceType));
            item.SubItems.Add(source.ComponentId.Print());
        }

        private void AdapterManagementOperatorControl_Load(object sender, EventArgs e)
        {
        }

        public override void UnInitializeControl()
        {
            if (Operator != null)
            {
            }

            base.UnInitializeControl();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = string.Empty;

            foreach (ListViewItem item in listViewSources.SelectedItems)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    if (i != 0)
                    {
                        text += " || ";
                    }

                    text += item.SubItems[i].Text;
                }

                text += Environment.NewLine;
            }


            if (string.IsNullOrEmpty(text) == false)
            {
                Clipboard.SetText(text);
            }
        }



    }
}
