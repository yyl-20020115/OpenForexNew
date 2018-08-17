using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    public abstract class AManaged
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent"></param>
        public AManaged()
        {
        }
        
        // This needs to be a separate call so that the children can define precisely when to attach to parent.
        protected void Initialize(AManaged parent, string name)
        {
            // Keep this order, the _parent assigned first, then the _parent.AddChild.
            _parent = parent;
            _name = name;

            if (_parent != null)
            {
                _parent.AddChild(this);
            }
        }

        /// <summary>
        /// When cloning, new item is not assigned to original item's parent.
        /// New created item is left to exist freely.
        /// </summary>
        /// <param name="managed"></param>
        protected void CloneTo(AManaged managed)
        {
            managed._name = _name;
            managed._closed = _closed;

        //    foreach(AManaged child in this.Children)
        //    {
        //        AManaged newChild = (AManaged)child.Clone();
        //        newChild._parent = managed;
        //        managed.AddChild(newChild);
        //    }
        }

        int _imageIndex = -1;
        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

        private AManaged _parent;
        private bool _closed = false;

        public AManaged Parent
        {
            get
            {
                return _parent;
            }
        }

        private List<AManaged> _children = new List<AManaged>();
        protected System.Collections.ObjectModel.ReadOnlyCollection<AManaged> Children
        {
            get
            {
                return _children.AsReadOnly();
            }
        }

        public AManaged[] ChildrenArray
        {
            get
            {
                AManaged[] result = new AManaged[_children.Count];
                _children.CopyTo(result);
                return result;
            }
        }

        public bool Closed
        {
            get
            {
                return _closed;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Exception handling error mechanism in place.
        /// </summary>
        /// <param name="child"></param>
        private void AddChild(AManaged child)
        {
            OnPreAddChild(child);
            _children.Add(child);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        private void RemoveChild(AManaged child)
        {
            OnPreRemoveChild(child);
            _children.Remove(child);
        }

        /// <summary>
        /// Trigger the update chain down the hierarcy, with OnUpdate() calls.
        /// </summary>
        public void Update()
        {
            //Profiler.Enter(Profiler.ProfilerEntry.AManaged_Update);

            OnUpdate();

            if (_children != null)
            {
                foreach (AManaged managed in _children)
                {
                    managed.Update();
                }
            }

            //Profiler.Leave(Profiler.ProfilerEntry.AManaged_Update);
        }

        /// <summary>
        /// Close and detach from parent.
        /// </summary>
        public void CloseAndDetach()
        {
            OnCloseAndDetach();

            _closed = true;

            if (_parent != null)
            {// Detach from parent.
                _parent.RemoveChild(this);
            }

            while(_children.Count > 0 )
            {
                _children[0].CloseAndDetach();
            }

            // Detach event from parent last, to allow children to send last events
            //EventEvent = null;
        }

        /// <summary>
        /// CompletionEvent overridables, all before the actual event.
        /// </summary>
        protected virtual void OnUpdate() { }
        protected virtual void OnCloseAndDetach() { }

        protected virtual void OnPreAddChild(AManaged child) { }
        protected virtual void OnPreRemoveChild(AManaged child) { }

        public virtual string Print() { return "AManaged"; }
    }
}
