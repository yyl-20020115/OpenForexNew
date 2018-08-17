using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// Used to uniquely identify a client.
    /// </summary>
    [Serializable]
    public struct ArbiterClientId : IEquatable<ArbiterClientId>, IComparable<ArbiterClientId>, IComparable<ComponentId>, IEquatable<ComponentId>
    {
        ComponentId _id;
        /// <summary>
        /// Actual common identification structure.
        /// </summary>
        public ComponentId Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Name of the client (stored inside Id).
        /// Exported for ease of debug and usability.
        /// </summary>
        public string ClientName
        {
            get
            {
                return _id.Name;
            }

            set
            {
                _id.Name = value; 
            }
        }

        /// <summary>
        /// Tag is used in a special place - to match the server session ID to an actual ArbiterClientID;
        /// </summary>
        volatile string _sessionTag;

        /// <summary>
        /// Tag is used in a special place - to match the server session ID to an actual ArbiterClientID;
        /// </summary>
        public string SessionTag
        {
            get { return _sessionTag; }
            set { _sessionTag = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _id.IsEmpty;
            }
        }

        /// <summary>
        /// Empty ID. Stands for none assigned.
        /// </summary>
        public static ArbiterClientId Empty = new ArbiterClientId(string.Empty, null, null) { _id = new ComponentId() { Guid = Guid.Empty } };

        [NonSerialized]
        volatile IArbiterClient _optionalReference;
        /// <summary>
        /// This reference is for optimization purposes. It is only present on sending on local arbiter.
        /// </summary>
        public IArbiterClient OptionalReference
        {
            get { return _optionalReference; }
        }

        #region Custom Operator

        /// <summary>
        /// 
        /// </summary>
        public static bool operator ==(ArbiterClientId a, ArbiterClientId b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator !=(ArbiterClientId a, ArbiterClientId b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals((ArbiterClientId)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArbiterClientId(string name, Type identifiedType, IArbiterClient optionalReference)
        {
            _optionalReference = optionalReference;
            _sessionTag = string.Empty;
            _id = new ComponentId() { Guid = Guid.NewGuid(), Name = name, IdentifiedComponentType = identifiedType };
        }

        #region IEquatable<ArbiterClientID> Members
        /// <summary>
        /// Equality is parialy compared (Guid only).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ArbiterClientId other)
        {
            return Id.Equals(other.Id);
        }

        #endregion

        #region IComparable<ArbiterClientId> Members

        /// <summary>
        /// Compare is done partial (Guid only).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ArbiterClientId other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion

        #region IComparable<ComponentId> Members

        public int CompareTo(ComponentId other)
        {
            return Id.CompareTo(other);
        }

        #endregion

        #region IEquatable<ComponentId> Members

        public bool Equals(ComponentId other)
        {
            return Id.Equals(other);
        }

        #endregion
    }
}
