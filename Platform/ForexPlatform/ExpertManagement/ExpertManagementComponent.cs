using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;
using CommonFinancial;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace ForexPlatform
{
    /// <summary>
    /// Control allows the tradeEntities (execution/modification) of runtime experts.
    /// </summary>
    [Serializable]
    [UserFriendlyName("Experts")]
    [ComponentManagement(true, false, 10, false)]
    public class ExpertManagementComponent : ManagementPlatformComponent
    {
        List<ExpertInformation> _expertInfos = new List<ExpertInformation>();
        public ReadOnlyCollection<ExpertInformation> ExpertInfosUnsafe
        {
            get { lock (this) { return _expertInfos.AsReadOnly(); } }
        }

        public ExpertInformation[] ExpertInfosArray
        {
            get { lock (this) { return _expertInfos.ToArray(); } }
        }

        Dictionary<Assembly, string> _expertContainingAssembliesAndPaths = new Dictionary<Assembly, string>();

        public Assembly[] ExpertsAssembliesArray
        {
            get { lock (this) { return GeneralHelper.EnumerableToArray<Assembly>(_expertContainingAssembliesAndPaths.Keys); } }
        }

        public delegate void ExpertContainerUpdateDelegate(ExpertInformation container);
        public event ExpertContainerUpdateDelegate AddedExpertContainerEvent;
        public event ExpertContainerUpdateDelegate RemovedExpertContainerEvent;

        public delegate void ExpertAssemblyAddedDelegate(Assembly assembly);
        public event ExpertAssemblyAddedDelegate ExpertAssemblyAddedEvent;
        public event ExpertAssemblyAddedDelegate ExpertAssemblyRemovedEvent;

        /// <summary>
        /// 
        /// </summary>
        public ExpertManagementComponent()
            : base(false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ExpertManagementComponent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            string[] assembliesPaths = (string[])info.GetValue("assembliesPaths", typeof(string[]));

            for (int i = 0; i < assembliesPaths.Length; i++)
            {// Load assemblies.
                try
                {
                    _expertContainingAssembliesAndPaths.Add(Assembly.LoadFrom(assembliesPaths[i]), assembliesPaths[i]);
                }
                catch (Exception ex)
                {
                    SystemMonitor.OperationError("Failed to load assembly [" + assembliesPaths[i] + ", " + ex.Message + "].");
                }
            }

            List<ExpertInformation> expertInfos = (List<ExpertInformation>)info.GetValue("expertContainers", typeof(List<ExpertInformation>));
            foreach (ExpertInformation expertInfo in expertInfos)
            {
                if (expertInfo.IsLocal)
                {
                    if (string.IsNullOrEmpty(expertInfo.FilePath))
                    {
                        SystemMonitor.Error("Local expert info has no file path assigned; info skipped.");
                        continue;
                    }

                    if (File.Exists(expertInfo.FilePath))
                    {// Check if local file and loaded source is the same.
                        expertInfo.LoadLocal(true);
                    }
                    else
                    {// Local file is missing - create it with internal source code.
                        expertInfo.SaveLocal();
                    }
                }

                _expertInfos.Add(expertInfo);
            }
        }

        /// <summary>
        /// Create a new expert based on a local file source and name.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="expertName"></param>
        /// <param name="operationResultMessage"></param>
        /// <returns></returns>
        public bool CreateExpertFromFile(string filePath, string expertName, out string operationResultMessage)
        {
            foreach (ExpertInformation info in ExpertInfosArray)
            {
                if (info.IsLocal && string.IsNullOrEmpty(info.FilePath) == false
                    && info.FilePath.ToLower() == filePath.ToLower())
                {
                    operationResultMessage = "Expert already added.";
                    return false;
                }
            }

            string sourceCode;
            using (StreamReader reader = new StreamReader(filePath))
            {
                sourceCode = reader.ReadToEnd();
            }

            // If this file is coming from the experts folder, we can directly reuse it.
            bool isExpertsFolder = Platform.Settings.GetMappedPath("ExpertsFolder").ToLower().CompareTo(Path.GetDirectoryName(filePath).ToLower()) == 0;
            ExpertInformation expertInfo = ExpertInformation.CreateLocal(Platform, expertName, isExpertsFolder, sourceCode, Guid.NewGuid(), out operationResultMessage);

            // If creation failed, try with changed name.
            int index = 0;
            while (expertInfo == null && index < 25)
            {
                expertInfo = ExpertInformation.CreateLocal(Platform, expertName + " " + index.ToString(), false, sourceCode, Guid.NewGuid(), out operationResultMessage);
                index++;
            }

            if (expertInfo == null)
            {
                operationResultMessage = "Failed to create new expert since due to file name conflict. Rename source file and try again.";
                return false;
            }

            operationResultMessage = "";
            AddExpert(expertInfo);

            return true;
        }

        public new void RaisePersistenceDataUpdatedEvent()
        {
            base.RaisePersistenceDataUpdatedEvent();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("assembliesPaths", GeneralHelper.EnumerableToArray<string>(_expertContainingAssembliesAndPaths.Values));
            info.AddValue("expertContainers", _expertInfos);
        }

        protected override bool OnInitialize(Platform platform)
        {
            bool result = base.OnInitialize(platform);
            ChangeOperationalState(OperationalStateEnum.Operational);
            return result;
        }

        protected override bool OnUnInitialize()
        {
            ChangeOperationalState(OperationalStateEnum.NotOperational);
            return base.OnUnInitialize();
        }

        public ExpertInformation GetContainerByGuid(Guid guid)
        {
            lock (this)
            {
                foreach (ExpertInformation container in _expertInfos)
                {
                    if (container.Guid == guid)
                    {
                        return container;
                    }
                }
            }

            return null;
        }

        public bool AddAssembly(string assemblyPath)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFile(assemblyPath);
            }
            catch(Exception ex)
            {
                SystemMonitor.Error("Failed to load assembly [" + assemblyPath + "; " + ex.Message +"]");
                return false;
            }

            //if (string.IsNullOrEmpty(assemblyGuid))
            //{
            //    SystemMonitor.Error("Assembly has no GUID [" + assembly.FullName + "]");
            //    return false;
            //}

            lock (this)
            {

                if (_expertContainingAssembliesAndPaths.ContainsKey(assembly))
                {
                    return true;
                }

                _expertContainingAssembliesAndPaths.Add(assembly, assemblyPath);
            }

            // Import Managed experts from this assembly.
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(Expert)))
                {
                    ExpertInformation container = ExpertInformation.CreateExternal(type.Name, assembly, type, Guid.NewGuid());
                    AddExpert(container);
                }
            }

            if (ExpertAssemblyAddedEvent != null)
            {
                ExpertAssemblyAddedEvent(assembly);
            }
            return true;
        }

        public bool RemoveAssembly(Assembly assembly)
        {
            
            lock (this)
            {
                if (_expertContainingAssembliesAndPaths.ContainsKey(assembly) == false)
                {
                    SystemMonitor.Error("Failed to remove assembly [" + assembly.FullName + "]");
                    return false;
                }

                SystemMonitor.CheckError(_expertContainingAssembliesAndPaths.Remove(assembly));

                foreach (ExpertInformation container in GeneralHelper.EnumerableToArray<ExpertInformation>(_expertInfos))
                {
                    if (container.ContainingAssembly == assembly)
                    {
                        RemoveExpert(container);
                    }
                }
            }

            if (ExpertAssemblyRemovedEvent != null)
            {
                ExpertAssemblyRemovedEvent(assembly);
            }
            return true;
        }

        public bool AddExpert(ExpertInformation expert)
        {
            lock (this)
            {
                if (expert.Guid == Guid.Empty || GetContainerByGuid(expert.Guid) != null)
                {
                    SystemMonitor.Error("Failed to add expert container [Already added, or Guid is invalid].");
                    return false;
                }

                _expertInfos.Add(expert);
            }

            if (AddedExpertContainerEvent != null)
            {
                AddedExpertContainerEvent(expert);
            }
            return true;
        }

        public bool RemoveExpert(ExpertInformation expertContainer)
        {
            lock (this)
            {
                if (_expertInfos.Remove(expertContainer) == false)
                {
                    return false;
                }
            }
            
            RemovedExpertContainerEvent(expertContainer);
            return true;
        }
    }
}
