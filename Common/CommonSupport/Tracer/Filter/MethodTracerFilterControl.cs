using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace CommonSupport
{
    /// <summary>
    /// Control allows controlling the originating baseMethod based filtering of tracer items.
    /// </summary>
    public partial class MethodTracerFilterControl : UserControl
    {
        MethodTracerFilter _filter;
        /// <summary>
        /// The filter instance associated with this control.
        /// </summary>
        public MethodTracerFilter Filter
        {
            get { return _filter; }
            set 
            {
                if (_filter != null)
                {
                    _filter.FilterUpdatedEvent -= new TracerFilter.FilterUpdatedDelegate(_filter_FilterUpdatedEvent);
                    _filter = null;
                }

                _filter = value;

                if (_filter != null)
                {
                    _filter.FilterUpdatedEvent += new TracerFilter.FilterUpdatedDelegate(_filter_FilterUpdatedEvent);
                }

                UpdateUI();

                treeView.ExpandAll();
            }
        }

       
        /// <summary>
        /// 
        /// </summary>
        public MethodTracerFilterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Possible NON UI thread.
        /// </summary>
        void _filter_FilterUpdatedEvent(TracerFilter filter)
        {
            WinFormsHelper.BeginManagedInvoke(this, new GeneralHelper.DefaultDelegate(UpdateUI));
        }

        /// <summary>
        /// Update user interface based on the underlying information.
        /// </summary>
        void UpdateUI()
        {
            if (_filter == null)
            {
                treeView.Nodes.Clear();
                return;
            }

            int i = 0;
            lock (_filter)
            {
                foreach (Assembly assembly in _filter.Assemblies)
                {
                    if (i > treeView.Nodes.Count - 1)
                    {
                        treeView.Nodes.Add("New node");
                    }

                    SetNodeToAssembly(treeView.Nodes[i], assembly);
                    i++;
                }
            }
            
            // Clear remaining nodes.
            for (int j = treeView.Nodes.Count - 1; j >= i; j--)
            {
                treeView.Nodes.RemoveAt(j);
            }
        }

        void SetNodeToType(TreeNode node, Type type)
        {
            node.Tag = type;
            node.Text = type.Name + " Class";

            lock (_filter)
            {
                node.Checked = _filter.GetAssemblyInfo(type.Assembly).Types[type];
            }
        }

        void SetNodeToAssembly(TreeNode assemblyNode, Assembly assembly)
        {
            assemblyNode.Tag = assembly;
            assemblyNode.Text = assembly.GetName().Name;
            lock (_filter)
            {
                assemblyNode.Checked = _filter.GetAssemblyInfo(assembly).Enabled;

                int i = 0;
                foreach (Type type in _filter.GetAssemblyInfo(assembly).Types.Keys)
                {
                    if (i > assemblyNode.Nodes.Count - 1)
                    {
                        assemblyNode.Nodes.Add("New node");
                    }

                    SetNodeToType(assemblyNode.Nodes[i], type);
                    i++;
                }

                // Clear remaining nodes.
                for (int j = assemblyNode.Nodes.Count - 1; j >= i; j--)
                {
                    assemblyNode.Nodes.RemoveAt(j);
                }
            }
        }


        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_filter == null)
            {
                return;
            }
            lock (_filter)
            {
                if (e.Node.Tag is Assembly)
                {
                    Assembly assembly = (Assembly)e.Node.Tag;
                    if (_filter.GetAssemblyInfo(assembly).Enabled != e.Node.Checked)
                    {// Protect when SetNodeToAssembly is called.
                        _filter.GetAssemblyInfo(assembly).Enabled = e.Node.Checked;
                    }
                }
                else if (e.Node.Tag is Type)
                {
                    Type type = (Type)e.Node.Tag;
                    if (_filter.GetAssemblyInfo(type.Assembly).Types[type] != e.Node.Checked)
                    {// Protect when SetNodeToType is called.
                        _filter.GetAssemblyInfo(type.Assembly).Types[type] = e.Node.Checked;
                    }
                }
                else
                {
                    SystemMonitor.Throw("Unhandled type.");
                }
            }
        }

        private void toolStripButtonCheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                node.Checked = true;
            }
        }

        private void toolStripButtonUnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                node.Checked = false;
            }
        }

    }
}
