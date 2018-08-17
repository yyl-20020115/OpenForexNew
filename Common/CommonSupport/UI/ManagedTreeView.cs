using System;
using System.Windows.Forms;

namespace CommonSupport
{
	/// <summary>
	/// Summary description for ManagedTree.
	/// </summary>
	public class ManagedTreeView : TreeView
	{
		public ManagedTreeView()
		{
		}

		IManagedTreeNode _root;
		public IManagedTreeNode Root
		{
			get
			{
				return _root;
			}
			set
			{
				_root = value;
				CreateTree();
			}
		}

		protected void CreateTree()
		{
			this.Nodes.Clear();
			if ( _root != null )
			{
				_root.TreeModifiedEvent += new ModifiedDelegate(NodeModified);
				TreeNode rootNode = new TreeNode();
				UpdateNode(rootNode, _root);
				this.Nodes.Add(rootNode);
			}
			this.ExpandAll();
		}

		public TreeNode GetNodeByTag(TreeNodeCollection nodes, object tag)
		{
			foreach(TreeNode node in nodes)
			{
				if ( node.Tag == tag )
				{
					return node;
				}
				if ( GetNodeByTag(node.Nodes, tag) != null )
				{
					return GetNodeByTag(node.Nodes, tag);
				}
			}
			return null;
		}

		public void NodeModified(IManagedTreeNode node)
		{
			this.SuspendLayout();

			TreeNode treeNode = GetNodeByTag(Nodes, node);
			if ( treeNode != null )
			{
				UpdateNode(treeNode, node);
			}

			this.ExpandAll();
			this.ResumeLayout();
		}

		protected void UpdateNode(TreeNode treeNode, IManagedTreeNode node)
		{
			treeNode.Tag = node;
			treeNode.Text = node.TreeText;
			treeNode.ImageIndex = node.TreeImageIndex;
			treeNode.SelectedImageIndex = node.TreeImageIndex;

			for(int i=0; i<node.TreeChildren.Length; i++)
			{
				if ( treeNode.Nodes.Count > i )
				{// Update child.
					UpdateNode(treeNode.Nodes[i], node.TreeChildren[i]);
					continue;
				}
				
				// New child.
				TreeNode newChild = new TreeNode();
				node.TreeChildren[i].TreeModifiedEvent += new ModifiedDelegate(NodeModified);
				UpdateNode(newChild, node.TreeChildren[i]);
				treeNode.Nodes.Add(newChild);
			}

			for(int i=treeNode.Nodes.Count-1; i>=node.TreeChildren.Length; i--)
			{
				treeNode.Nodes[i].Remove();
			}
		}
    }
}
