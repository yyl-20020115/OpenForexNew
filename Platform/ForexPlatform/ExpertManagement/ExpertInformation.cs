using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using CommonFinancial;
using CommonSupport;
using System.IO;

namespace ForexPlatform
{
    /// <summary>
    /// Contains information (including source) of typical local experts,
    /// as well as experts imported from DLLs.
    /// </summary>
    [Serializable]
    public class ExpertInformation
    {
        Type _containingAssemblyExpertType;
        /// <summary>
        /// Applicable in external contained experts only.
        /// </summary>
        public Type ContainingAssemblyExpertType
        {
            get { return _containingAssemblyExpertType; }
        }

        Assembly _containingAssembly;
        /// <summary>
        /// May be empty/null. Applicable in external contained experts only.
        /// </summary>
        public Assembly ContainingAssembly
        {
            get { return _containingAssembly; }
        }

        bool _isExternal;
        
        /// <summary>
        /// External experts are imported from external assemblies and provide no source code.
        /// </summary>
        public bool IsExternal
        {
            get { return _isExternal; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLocal
        {
            get { return !_isExternal; }
        }

        volatile bool _isSavedLocally = true;
        public bool IsSavedLocally
        {
            get { return _isSavedLocally; }
        }

        volatile private string _filePath = string.Empty;
        /// <summary>
        /// Applicable for experts having a corresponding (.cs) file.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        volatile string _sourceCode;
        /// <summary>
        /// May be empty/null.
        /// </summary>
        [Browsable(false)]
        public string SourceCode
        {
            get { return _sourceCode; }
            
            set 
            {
                if (_sourceCode != value)
                {
                    _isSavedLocally = false;
                    _sourceCode = value;
                }
            }
        }

        Guid _guid;
        public Guid Guid
        {
            get { return _guid; }
        }

        volatile string _author = string.Empty;
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        volatile string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        volatile string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ExpertInformation()
        {
        }

        /// <summary>
        /// Construct.
        /// </summary>
        public static ExpertInformation CreateLocal(Platform platform, string name, bool overrideExisting, string sourceCode, Guid guid, out string operationResultMessage)
        {
            string path = Path.Combine((string)platform.Settings.GetMappedPath("ExpertsFolder"), name + ".cs");

            if (overrideExisting == false && File.Exists(path))
            {
                operationResultMessage = "Local File with this name already exists.";
                return null;
            }

            operationResultMessage = "Created";

            ExpertInformation result = new ExpertInformation();
            result.Name = name;
            result._isExternal = false;
            result._sourceCode = sourceCode;
            result._filePath = path;
            result._guid = guid;

            SystemMonitor.CheckError(result.SaveLocal(), "Failed to save expert locally.");

            return result;
        }

        /// <summary>
        /// Load expert source code from local file.
        /// </summary>
        /// <param name="checkLoadOnly">Only perform check if source and file are the same and update SaveLocally variable.</param>
        public bool LoadLocal(bool checkLoadOnly)
        {
            if (IsExternal)
            {
                SystemMonitor.Error("This expert is external, can not save locally.");
                return false;
            }

            string sourceCode;
            try
            {
                using (FileStream fs = File.OpenRead(_filePath))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sourceCode = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("Failed to load local expert file [" + _filePath + "] [" + ex.Message + " ]");
                return false;
            }

            if (checkLoadOnly == false)
            {
                _sourceCode = sourceCode;
            }

            _isSavedLocally = _sourceCode.CompareTo(sourceCode) == 0;
            return true;
        }

        /// <summary>
        /// Save expert source code to local file.
        /// </summary>
        public bool SaveLocal()
        {
            if (IsExternal)
            {
                SystemMonitor.Error("This expert is external, can not save locally.");
                return false;
            }

            try
            {
                using (FileStream fs = File.Create(_filePath))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (string.IsNullOrEmpty(_sourceCode) == false)
                        {
                            sw.Write(_sourceCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError(ex.Message);
                _isSavedLocally = false;
                return false;
            }

            _isSavedLocally = true;
            return true;
        }

        /// <summary>
        /// Delete local file for this information.
        /// </summary>
        /// <returns></returns>
        public bool DeleteLocal()
        {
            if (IsExternal || string.IsNullOrEmpty(_filePath))
            {
                SystemMonitor.Error("This expert is external or already deleted path, can not save locally.");
                return false;
            }

            try
            {
                File.Delete(_filePath);
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("Failed to delete file [" + _filePath + "][" + ex.Message + "].");
            }

            _filePath = string.Empty;

            return true;
        }

        /// <summary>
        /// Construct.
        /// </summary>
        public static ExpertInformation CreateExternal(string name, Assembly containingAssembly, 
            Type containingAssemblyType, Guid guid)
        {
            ExpertInformation result = new ExpertInformation();

            result.Name = name;
            result._isExternal = true;
            result._isSavedLocally = true;
            result._containingAssembly = containingAssembly;
            result._containingAssemblyExpertType = containingAssemblyType;
            result._guid = guid;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Type GetExpertType(ref string operationResultMessage)
        {
            if (IsExternal)
            {
                return ContainingAssemblyExpertType;
            }

            // We need to compile runtime.
            Dictionary<string, int> messages;
            Assembly assembly = CompilationHelper.CompileSourceToAssembly(_sourceCode, out messages);

            if (assembly == null)
            {
                operationResultMessage = "Failed to compile expert source code.";
                return null;
            }

            List<Type> expertTypes = new List<Type>();
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(PlatformManagedExpert)))
                {
                    expertTypes.Add(type);
                }
            }

            if (expertTypes.Count == 0)
            {
                operationResultMessage = "Failed to find expert type in expert source code.";
                return null;
            }

            if (expertTypes.Count > 1)
            {
                operationResultMessage = "More than one expert type found in source code, creating instance of the first one found.";
            }

            return expertTypes[0];
        }

    }
}
