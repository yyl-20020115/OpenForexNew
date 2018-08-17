using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CommonSupport
{
    /// <summary>
    /// Serves as a basis for multiple types of identificantion classes.
    /// The concept is to provide a unified way of undentifying elements/instances etc.
    /// This is the top of the component hierarchy.
    /// </summary>
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public struct ComponentId : IComparable<ComponentId>, IEquatable<ComponentId>
    {
        /// <summary>
        /// Guid of item.
        /// </summary>
        Guid _guid;
        /// <summary>
        /// 
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        
        volatile string _name;
        /// <summary>
        /// Name of item.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Make sure to keep this non serialized, since its also currently part of the Arbiter mechanism
        /// and if send over trough to another module that does not have reference to all current types,
        /// arbiter clients etc. that are on the path of the message, an exception will occur on deserializing
        /// the message on the other side. To solve this the serialization procedure of the Message must,
        /// when sending to a remote location must make sure to remote these types here.
        /// </summary>
        [NonSerialized]
        Type _identifiedComponentType;
        
        /// <summary>
        /// Optional reference to the type that this Id identifies.
        /// </summary>
        public Type IdentifiedComponentType
        {
            get { return _identifiedComponentType; }
            set { _identifiedComponentType = value; }
        }

        public static ComponentId Empty
        {
            get { return new ComponentId(Guid.Empty, string.Empty, null); }
        }

        public bool IsEmpty
        {
            get { return this.Guid == Guid.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator ==(ComponentId a, ComponentId b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator !=(ComponentId a, ComponentId b)
        {
            return !(a == b);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals((ComponentId)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        public ComponentId(Guid guid, string name, Type componentType)
        {
            _guid = guid;
            _name = name;
            _identifiedComponentType = componentType;
        }

        /// <summary>
        /// Print general information related to this object.
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            if (_identifiedComponentType == null)
            {
                return Name + "[" + Guid.ToString() + "]";
            }
            else
            {
                return Name + "[" + _identifiedComponentType.Name + ", " + Guid.ToString() + "]";
            }
        }

        #region IComparable<Id> Members

        /// <summary>
        /// Compare is done partial (Guid only).
        /// </summary>
        public int CompareTo(ComponentId other)
        {
            return Guid.CompareTo(other.Guid);
        }

        #endregion

        #region IEquatable<Id> Members

        /// <summary>
        /// Equal is done partial (Guid only).
        /// </summary>
        public bool Equals(ComponentId other)
        {
            return (Guid.Equals(other.Guid));
        }

        #endregion
    }

}
