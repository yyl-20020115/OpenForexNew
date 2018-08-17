using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using CommonSupport;

namespace Nitro.Framework.Goliath.FrontEnd
{
    /// <summary>
    /// Class extends the list view with some automated data binding and update capabilities.
    /// </summary>
    public class ListViewAutoMapper
    {
        object _syncRootDummy = new object();

        object _source = null;
        public object Source
        {
            get { return _source; }
            set 
            { 
                _source = value;

                ListView listView = _listView;
                if (listView != null)
                {
                    listView.Items.Clear();
                }
                UpdateUI();
            }
        }

        object _sourceSyncRoot = null;
        /// <summary>
        /// Assign this in case something needs to be locked prior to accessing the source.
        /// </summary>
        public object SourceSyncRoot
        {
            get { return _sourceSyncRoot; }
            set { _sourceSyncRoot = value; }
        }

        ListView _listView = null;

        public ListView ListView
        {
            get { return _listView; }
            set 
            { 
                _listView = value; 
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListViewAutoMapper()
        {
        }


        public void UpdateUI()
        {
            ListView listView = _listView;
            if (listView != null)
            {
                WinFormsHelper.BeginManagedInvoke(_listView, DoUpdateUI);
            }
        }

        /// <summary>
        /// If the list is about to change
        /// </summary>
        void DoUpdateUI()
        {
            ListView listView = _listView;
            if (listView == null)
            {
                return;
            }

            object source = _source;
            if (source == null)
            {
                listView.Items.Clear();
                return;
            }

            object sourceSyncRoot = _sourceSyncRoot;
            if (sourceSyncRoot == null)
            {
                sourceSyncRoot = _syncRootDummy;
            }

            lock (sourceSyncRoot)
            {// Lock prior to accessing the source.

                if (ReflectionHelper.IsTypeImplementingInterface(source.GetType(), typeof(IEnumerable)))
                {
                    IEnumerable enumerable = (IEnumerable)source;

                    int index = 0;
                    foreach(object item in enumerable)
                    {
                        ListViewItem listViewItem;
                        if (listView.Items.Count <= index)
                        {
                            listViewItem = new ListViewItem();
                            listViewItem.SubItems.Clear();
                            for (int k = 0; k < listView.Columns.Count; k++)
                            {
                                listViewItem.SubItems.Add("-");
                            }
                            listView.Items.Add(listViewItem);
                        }
                        else
                        {
                            listViewItem = listView.Items[index];
                        }

                        for (int j = 0; j < listView.Columns.Count; j++)
			            {
                            string stringValue = string.Empty;
                            object value = ReflectionHelper.GetDynamicValue(item, listView.Columns[j].Tag.ToString());
                            if (value != null)
                            {
                                stringValue = value.ToString();
                            }

                            if (listViewItem.SubItems[j].Text != stringValue)
                            {
                                listViewItem.SubItems[j].Text = stringValue;
                            }
			            }

                        if (listViewItem.Tag != item)
                        {
                            listViewItem.Tag = item;
                        }

                        index++;
                    }

                    while (listView.Items.Count > index)
                    {
                        listView.Items.RemoveAt(listView.Items.Count - 1);
                    }
                }

            }

        }
    }
}
