using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommonSupport
{
    public partial class AManagedTreeControl : TreeView
    {
        public AManagedTreeControl()
        {
            InitializeComponent();
        }

        AManaged _root;
        public AManaged Root
        {
            set
            {
                _root = value;
                UpdateUIComponents();
            }

            get
            {
                return _root;
            }
        }

        private void SynchronizeCollectionWithNode(TreeNode parentNode, int startingIndex, AManaged[] items)
        {
            for (int i = startingIndex; i < items.Length + startingIndex; i++)
            {
                AManaged item = items[i - startingIndex];

                if (parentNode.Nodes.Count > i && parentNode.Nodes[i].Tag == item)
                {// Node with this index exists and is proper one.
                    parentNode.Nodes[i].ImageIndex = item.ImageIndex;
                    parentNode.Nodes[i].SelectedImageIndex = item.ImageIndex;
                    parentNode.Nodes[i].Text = item.Name;
                }
                else
                {// No existing node or improper one.

                    if (parentNode.Nodes.Count > i)
                    {// This is invalid existing node, remove.
                        parentNode.Nodes.RemoveAt(i);
                    }

                    TreeNode newNode = new TreeNode(item.Name, item.ImageIndex, item.ImageIndex);
                    newNode.Tag = item;

                    parentNode.Nodes.Add(newNode);
                }

                // Recursively synchronize the children.
                SynchronizeCollectionWithNode(parentNode.Nodes[i], 0, item.ChildrenArray);
            }

            for (int j = items.Length + startingIndex; j < parentNode.Nodes.Count; j++)
            {// Clean up the remaining nodes that are no longer valid.
                parentNode.Nodes.RemoveAt(j);
            }

        }


        protected void UpdateUIComponents()
        {
            if (_root == null)
            {// Clean and run.
                Nodes.Clear();
                return;
            }

            TreeNode rootNode;
            if (Nodes.Count > 0 && Nodes[0].Tag == Root)
            {// Existing root.
                rootNode = Nodes[0];
                rootNode.Text = Root.Name;
            }
            else
            {// New root.
                Nodes.Clear();
                rootNode = new TreeNode(Root.Name, Root.ImageIndex, Root.ImageIndex);
                rootNode.Tag = Root;
                Nodes.Add(rootNode);
            }

            SynchronizeCollectionWithNode(rootNode, 0, _root.ChildrenArray);

            ExpandAll();

            return;

            //lock (_simulation.Strategies)
            //{
            //    for (int i = 0; i < _simulation.Strategies.Count; i++)
            //    {
            //        TraderSimulationStrategy strategy = _simulation.Strategies[i];
            //        TreeNode strategyNode;

            //        // Synchronize the simulation.
            //        if (rootNode.Nodes.Count > i && rootNode.Nodes[i].Tag == strategy)
            //        {// Node exists and is the proper one.
            //            strategyNode = rootNode.Nodes[i];
            //        }
            //        else
            //        {// Node needs to be replaced/fixed.
            //            strategyNode = new TreeNode(strategy.Name);
            //            strategyNode.Tag = strategy;
            //            strategyNode.ImageIndex = 1;
            //            strategyNode.SelectedImageIndex = 1;

            //            if (rootNode.Nodes.Count > i)
            //            {
            //                rootNode.Nodes[i] = strategyNode;
            //            }
            //            else
            //            {
            //                rootNode.Nodes.Add(strategyNode);
            //            }
            //        }

            //        // Synchronize the simulation elements.
            //        for (int j = 0; j < strategy.Entities.Count; j++)
            //        {
            //            TraderSimulationEntity entity = strategy.Entities[j];

            //            // Synchronize the simulation.
            //            if (strategyNode.Nodes.Count > i && strategyNode.Nodes[i].Tag == entity)
            //            {// Node exists and is the proper one.
            //            }
            //            else
            //            {// Node needs to be replaced/fixed.

            //                TreeNode entityNode = new TreeNode(entity.Name);
            //                entityNode.Tag = entity;
            //                entityNode.ImageIndex = 2;
            //                entityNode.SelectedImageIndex = 2;

            //                if (strategyNode.Nodes.Count > j)
            //                {
            //                    strategyNode.Nodes[j] = entityNode;
            //                }
            //                else
            //                {
            //                    strategyNode.Nodes.Add(strategyNode);
            //                }
            //            }
            //        }
            //    }
            //    // Delete all the unneeded strategies.
            //    for (int i = _simulation.Strategies.Count; i < rootNode.Nodes.Count; i++)
            //    {
            //        rootNode.Nodes.RemoveAt(i);
            //    }
            //}

        }

        private void AManagedTreeControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.SelectedNode = this.HitTest(new Point(e.X, e.Y)).Node;
        }
    }
}
