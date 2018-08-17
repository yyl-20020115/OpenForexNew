using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;

namespace CommonSupport
{
    /// <summary>
    /// Class stores sensitive information securely.
    /// Works in cooperation with SecureDataManager, since the master password
    /// is only stored once in the manager.
    /// Currently supports working with strings.
    /// </summary>
    [Serializable]
    public class SecureDataEntry
    {
        SecureDataManager _manager;

        Dictionary<string, string> _fields = new Dictionary<string, string>();
        /// <summary>
        /// This is for internal access only, use the methods instead.
        /// </summary>
        internal Dictionary<string, string> FieldsUnsafe
        {
            get { return _fields; }
        }

        /// <summary>
        /// Entries only constructed by manager.
        /// </summary>
        internal SecureDataEntry(SecureDataManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Add a string entry for keeping.
        /// </summary>
        public bool SetField(string entryName, SecureString entryValue)
        {
            string ecryptedData;
            if (_manager.Encrypt(entryValue, out ecryptedData) == false)
            {
                return false;
            }

            lock (this)
            {
                _fields[entryName] = ecryptedData;
            }

            return true;
        }

        /// <summary>
        /// Get value of a field.
        /// </summary>
        public SecureString GetField(string entryName)
        {
            string entryArray;
            lock (this)
            {
                if (_fields.ContainsKey(entryName) == false)
                {
                    return null;
                }
                entryArray = _fields[entryName];
            }
            
            SecureString result;
            if (_manager.Decrypt(entryArray, out result) == false)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Remote field.
        /// </summary>
        /// <param name="entryName"></param>
        /// <returns></returns>
        public bool RemoveField(string entryName)
        {
            lock (this)
            {
                return _fields.Remove(entryName);
            }
        }

        /// <summary>
        /// Clear all fields in entry.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                _fields.Clear();
            }
        }

    }
}
