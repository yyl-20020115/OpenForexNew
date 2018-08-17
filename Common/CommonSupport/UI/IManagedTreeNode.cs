using System;

namespace CommonSupport
{
	public delegate void ModifiedDelegate(IManagedTreeNode node);

	/// <summary>
	/// Summary description for ITreeNode.
	/// </summary>
	public interface IManagedTreeNode
	{
		string TreeText
		{
			get;
		}

		IManagedTreeNode[] TreeChildren
		{
			get;
		}

		event ModifiedDelegate TreeModifiedEvent;

		int TreeImageIndex
		{
			get;
		}
	}
}
