using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace CommonSupport
{
    /// <summary>
    /// Class stores centralized security information (master password, etc.)
    /// </summary>
    [Serializable]
    public class SecureDataManager : IDeserializationCallback
    {
        #region Static and Const

        static readonly SecureString SecureMasterPasswordTestPhraze = GeneralHelper.StringToSecureString("%26SdqsT$5^!sdfdwVAas#42");

        #endregion

        [NonSerialized]
        RijndaelManaged _rijndaelCipher;

        /// <summary>
        /// A serialized evidence of previously used master password.
        /// </summary>
        string _masterPasswordEncryptedTestPhraze = null;
        
        [NonSerialized]
        volatile SecureString _masterPassword = null;
        
        /// <summary>
        /// Is the data encrypted with a master password.
        /// </summary>
        public bool MasterPasswordPresent
        {
            get { return _masterPasswordEncryptedTestPhraze != null; }
        }

        /// <summary>
        /// Has the master password been already set.
        /// </summary>
        public bool MasterPasswordSet
        {
            get { return _masterPassword != null; }
        }

        //byte[] MasterPasswordArray
        //{
        //    get
        //    {
        //        SecureString masterPassword = _masterPassword;
        //        if (masterPassword != null)
        //        {
        //            return GeneralHelper.SecureStringToByte(masterPassword);
        //        }
        //        else 
        //        {
        //            return null;
        //        }
        //    }
        //}

        Dictionary<ComponentId, SecureDataEntry> _componentsEntries = new Dictionary<ComponentId, SecureDataEntry>();

        /// <summary>
        /// 
        /// </summary>
        public SecureDataManager()
        {
            _rijndaelCipher = new RijndaelManaged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnDeserialization(object sender)
        {
            _rijndaelCipher = new RijndaelManaged();
            _masterPassword = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterPassword"></param>
        /// <returns></returns>
        public bool SetMasterPassword(SecureString masterPassword)
        {
            lock (this)
            {
                if (_masterPasswordEncryptedTestPhraze == null)
                {
                    _masterPassword = masterPassword;
                    return Encrypt(SecureMasterPasswordTestPhraze, out _masterPasswordEncryptedTestPhraze);
                }
                else
                {
                    if (TestMasterPassword(masterPassword))
                    {
                        _masterPassword = masterPassword;
                        return true;
                    }
                }
            }

            SystemMonitor.OperationWarning("Master password already set.");

            // Master password already set.
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        void RecodeEntries()
        {
        }

        /// <summary>
        /// Test if master password matches the passed parameter.
        /// </summary>
        public bool TestMasterPassword(SecureString masterPassword)
        {
            if (_masterPasswordEncryptedTestPhraze == null)
            {
                return false;
            }

            // Already password has been set, make sure this matches it.
            string masterPasswordEnrypted;
            Encrypt(masterPassword, SecureMasterPasswordTestPhraze, out masterPasswordEnrypted);
            return masterPasswordEnrypted == _masterPasswordEncryptedTestPhraze;
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="existingPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangeMasterPassword(SecureString newPassword)
        {
            lock (this)
            {
                if (_masterPasswordEncryptedTestPhraze != null)
                {// Current data is already encoded.
                    if (_masterPassword == null)
                    {
                        SystemMonitor.OperationWarning("Can not change master password if previous password present but not set yet.");
                        return false;
                    }
                }
                else
                {
                    _masterPassword = newPassword;
                    return Encrypt(SecureMasterPasswordTestPhraze, out _masterPasswordEncryptedTestPhraze);
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="valueArray"></param>
        /// <returns></returns>
        public bool Encrypt(SecureString key, SecureString value, out string encryptedValue)
        {
            string password = GeneralHelper.SecureStringToString(key);
            string encryptableValue = GeneralHelper.SecureStringToString(value);

            byte[] plainText = System.Text.Encoding.Unicode.GetBytes(encryptableValue);
            byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
            
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);

            // Creates a symmetric encryptor object.
            ICryptoTransform encryptor = _rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            byte[] cipherBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Defines a stream that links data streams to cryptographic transformations.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainText, 0, plainText.Length);

                    // Writes the final state and clears the buffer.
                    cryptoStream.FlushFinalBlock();

                    cipherBytes = memoryStream.ToArray();
                }
            }

            encryptedValue = Convert.ToBase64String(cipherBytes);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedValue"></param>
        /// <param name="key"></param>
        /// <param name="decryptedValue"></param>
        /// <returns></returns>
        public bool Decrypt(string encryptedValue, SecureString key, out SecureString decryptedValue)
        {
            decryptedValue = null;
            string Password = GeneralHelper.SecureStringToString(key);

            string decryptedText;
            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedValue);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
                
                //Making of the key for decryption
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
                
                //Creates a symmetric Rijndael decryptor object.
                ICryptoTransform Decryptor = _rijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainText = new byte[encryptedData.Length];
                        int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

                        //Converting to string
                        decryptedText = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                    }
                }
            }
            catch(Exception ex)
            {
                SystemMonitor.OperationError("Failed to decrypt, reason [" + ex.Message + "]");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueArray"></param>
        /// <returns></returns>
        public bool Encrypt(SecureString value, out string valueArray)
        {
            valueArray = null;
            lock (this)
            {
                if (_masterPassword == null)
                {
                    return false;
                }

                return Encrypt(_masterPassword, value, out valueArray);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Decrypt(string encryptedValue, out SecureString value)
        {
            value = null;
            lock (this)
            {
                if (_masterPassword == null)
                {
                    return false;
                }
                else
                {
                    return Decrypt(encryptedValue, _masterPassword, out value);
                }
            }
        }


    }
}
