using System;
using System.Collections.Generic;
using System.Text;

namespace Arbiter
{
    /// <summary>
    /// Information regarding the transportation path of a requestMessage.
    /// </summary>
    [Serializable]
    public class TransportInfo : IEquatable<TransportInfo>, IComparable<TransportInfo>, ICloneable
    {
        List<TransportInfoUnit> _infoUnits = new List<TransportInfoUnit>();
        public TransportInfoUnit? CurrentTransportInfo
        {
            get
            {
                lock (this)
                {
                    if (_infoUnits.Count == 0)
                    {
                        return null;
                    }

                    return _infoUnits[_infoUnits.Count - 1];
                }
            }
        }

        public ArbiterClientId? OriginalSenderId
        {
            get
            {
                if (_infoUnits.Count > 0)
                {
                    return _infoUnits[0].SenderID;
                }

                return null;
            }
        }

        public int TransportInfoCount
        {
            get { lock (this) { return _infoUnits.Count; } }
        }

        List<ArbiterClientId?> _forwardTransportInfo = new List<ArbiterClientId?>();
        public ArbiterClientId? CurrentForwardTransportInfoAddress
        {
            get
            {
                lock (this)
                {
                    if (_forwardTransportInfo.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return _forwardTransportInfo[_forwardTransportInfo.Count - 1];
                    }
                }
            }

        }

        public int ForwardTransportInfoCount
        {
            get { lock (this) { return _forwardTransportInfo.Count; } }
        }

        /// <summary>
        /// 
        /// </summary>
        public TransportInfo()
        {
        }

        /// <summary>
        /// Create info from path.
        /// </summary>
        public TransportInfo(List<ArbiterClientId?> path, ArbiterClientId? receiver)
        {
            _infoUnits.Add(new TransportInfoUnit(Guid.NewGuid(), path[0], receiver));
            for (int i = 0; i < path.Count - 1; i++)
            {
                this._infoUnits.Add(new TransportInfoUnit(Guid.NewGuid(), path[1], path[0]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ArbiterClientId?> CreateRespondingClientList()
        {
            List<ArbiterClientId?> result = new List<ArbiterClientId?>();
            lock (this)
            {
                foreach (TransportInfoUnit unit in _infoUnits)
                {
                    result.Add(unit.SenderID);
                }
            }

            result.Reverse();
            return result;
        }

        /// <summary>
        /// Will check to see if this info and the other info origin from the same initial address.
        /// </summary>
        /// <param name="otherInfo"></param>
        /// <returns></returns>
        public bool CheckOriginalSender(TransportInfo otherInfo)
        {
            lock (this)
            {
                if (otherInfo._infoUnits.Count == 0 || this._infoUnits.Count == 0)
                {
                    return false;
                }

                return otherInfo._infoUnits[0].SenderID.Value.Id.Guid == _infoUnits[0].SenderID.Value.Id.Guid;
            }
        }

        /// <summary>
        /// Clone this instance of TranportInfo.
        /// </summary>
        /// <returns></returns>
        public TransportInfo Clone()
        {
            TransportInfo result = new TransportInfo();
            lock (this)
            {
                result._forwardTransportInfo = new List<ArbiterClientId?>(this._forwardTransportInfo);
                result._infoUnits = new List<TransportInfoUnit>(this._infoUnits);
            }
            return result;
        }

        /// <summary>
        /// Add a new unit.
        /// </summary>
        /// <param name="infoUnit"></param>
        public void AddTransportInfoUnit(TransportInfoUnit infoUnit)
        {
            lock (this)
            {
                _infoUnits.Add(infoUnit);
            }
        }

        /// <summary>
        /// Peek at the top of the tranposrt infos.
        /// </summary>
        /// <returns></returns>
        public TransportInfoUnit? PeekTransportInfo()
        {
            lock (this)
            {
                if (_infoUnits.Count == 0)
                {
                    return null;
                }
                else
                {
                    TransportInfoUnit? result = _infoUnits[_infoUnits.Count - 1];
                    return result;
                }
            }
        }

        /// <summary>
        /// Take the top of the transport infos and remove it from list.
        /// </summary>
        /// <returns></returns>
        public TransportInfoUnit? PopTransportInfo()
        {
            lock (this)
            {
                if (_infoUnits.Count == 0)
                {
                    return null;
                }
                else
                {
                    TransportInfoUnit? result = _infoUnits[_infoUnits.Count - 1];
                    _infoUnits.RemoveAt(_infoUnits.Count - 1);
                    return result;
                }
            }
        }

        public ArbiterClientId? PopForwardTransportInfo()
        {
            lock (this)
            {
                if (_forwardTransportInfo.Count == 0)
                {
                    return null;
                }
                else
                {
                    ArbiterClientId? result = _forwardTransportInfo[_forwardTransportInfo.Count - 1];
                    _forwardTransportInfo.RemoveAt(_forwardTransportInfo.Count - 1);
                    return result;
                }
            }
        }

        /// <summary>
        /// Add an ID of a next transportation step.
        /// </summary>
        /// <param name="info"></param>
        public void AddForwardTransportId(ArbiterClientId? info)
        {
            lock (this)
            {
                _forwardTransportInfo.Add(info);
            }
        }
        
        /// <summary>
        /// Set the IDs of all the transportation steps.
        /// </summary>
        /// <param name="info"></param>
        public void SetForwardTransportInfo(IEnumerable<ArbiterClientId?> info)
        {
            lock (this)
            {
                _forwardTransportInfo = new List<ArbiterClientId?>(info);
                _forwardTransportInfo.Reverse();
            }
        }

        #region IEquatable<TransportInfo> Members

        public bool Equals(TransportInfo other)
        {
            if (this == other)
            {
                return true;
            }
            lock (this)
            {
                for (int i = 0; i < _forwardTransportInfo.Count && i < other._forwardTransportInfo.Count; i++)
                {
                    if (_forwardTransportInfo.Count <= i || other._forwardTransportInfo.Count <= i)
                    {
                        return false;
                    }
                    if (_forwardTransportInfo[i].HasValue != other._forwardTransportInfo[i].HasValue
                        || (_forwardTransportInfo[i].HasValue && _forwardTransportInfo[i].Value.Equals(other._forwardTransportInfo[i].Value) == false))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < _infoUnits.Count && i < other._infoUnits.Count; i++)
                {
                    if (_infoUnits.Count <= i || other._infoUnits.Count <= i)
                    {
                        return false;
                    }
                    if (_infoUnits[i].EqualsSenderAndReceiver(other._infoUnits[i]) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            TransportInfo other = Clone();
            return other;
        }

        #endregion

        #region IComparable<TransportInfo> Members

        public int CompareTo(TransportInfo other)
        {
            if (this.Equals(other))
            {
                return 0;
            }

            return 1;
        }

        #endregion
    }
}
