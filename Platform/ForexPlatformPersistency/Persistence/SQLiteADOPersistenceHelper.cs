using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using CommonSupport;
using System.Xml;

namespace ForexPlatformPersistence
{
    /// <summary>
    /// Helps the operations of persisting classes to a DB.
    /// Only operates on classes (no structs), due to the type params are manipulated and mapped.
    /// </summary>
    public class SQLiteADOPersistenceHelper
    {
		/// <summary>
		/// Class used to operate the Dynamic types table.
		/// </summary>
		public class DynamicTypeInformation : IDBPersistent
		{
			long? _id;

			/// <summary>
			/// Id of the item.
			/// </summary>
			public long? Id
			{
				get { return _id; }
				set { _id = value; }
			}

			volatile string _assemblyQualifiedName = string.Empty;
			/// <summary>
			/// Name of the type.
			/// </summary>
			public string AssemblyQualifiedName
			{
				get { return _assemblyQualifiedName; }
				set { _assemblyQualifiedName = value; }
			}

			/// <summary>
			/// Constructor.
			/// </summary>
			public DynamicTypeInformation()
			{
			}
		}

        volatile string _xmlDataColumnName = "XmlBlob";
        /// <summary>
        /// Used for the parallel xml serialization of objects properties,
        /// usefull for dynamic persistence typing.
        /// </summary>
        public string XmlDataColumnName
        {
            get { return _xmlDataColumnName; }
            set { _xmlDataColumnName = value; }
        }

		volatile string _dynamicTypesTableName = "DynamicTypes";
		/// <summary>
		/// Name of the table containing the dynamic types.
		/// </summary>
		public string DynamicTypesTableName
		{
		  get { return _dynamicTypesTableName; }
		  set { _dynamicTypesTableName = value; }
		}

        volatile string _typeIdColumn = "TypeId";
        /// <summary>
        /// Default name for dynamic type column.
        /// </summary>
        public string TypeIdColumn
        {
			get { return _typeIdColumn; }
			set { _typeIdColumn = value; }
        }

        volatile string _connectionString = string.Empty;

        Dictionary<Type, string> _tablesTypeNames = new Dictionary<Type, string>();
        //Dictionary<Type, Dictionary<string, string>> _defaultTypeNamesMapping = new Dictionary<Type, Dictionary<string, string>>();

		BiDictionary<long, string> _dynamicTypes = new BiDictionary<long, string>();

        int _defaultMutexTimeOut = 30000;

        Mutex _executeMutex = new Mutex();
        Mutex _insertMutex = new Mutex();
        Mutex _selectMutex = new Mutex();
        Mutex _updateMutex = new Mutex();
        Mutex _deleteMutex = new Mutex();
        Mutex _countMutex = new Mutex();
        Mutex _clearMutex = new Mutex();

        /// <summary>
        /// Is the helper ready for operation.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return string.IsNullOrEmpty(_connectionString) == false;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SQLiteADOPersistenceHelper()
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public bool CreateDataBaseFile(string filePath)
        {
            try
            {
                string folder = Path.GetDirectoryName(filePath);
                if (Directory.Exists(folder) == false)
                {
                    DirectoryInfo info = Directory.CreateDirectory(folder);
                    if (info == null)
                    {
                        SystemMonitor.OperationError("Failed to create folder [" + folder + "].");
                        return false;
                    }
                }

                SQLiteConnection.CreateFile(filePath);
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Initialize(string databaseFilePath, bool createIfNotExists)
        {
            if (File.Exists(databaseFilePath) == false)
            {
                if (createIfNotExists)
                {
                    if (CreateDataBaseFile(databaseFilePath) == false)
                    {
                        SystemMonitor.Warning("Failed to create database file [" + databaseFilePath + "].");
                        return false;
                    }
                }
                else
                {
                    SystemMonitor.OperationError("Failed to find database file [" + databaseFilePath + "].");
                    return false;
                }
            }

			_connectionString = "data source=" + Path.GetFullPath(databaseFilePath);

			if (ContainsTable(_dynamicTypesTableName))
			{
				SetupTypeMapping(typeof(DynamicTypeInformation), _dynamicTypesTableName);

				foreach (DynamicTypeInformation information in Select<DynamicTypeInformation>(null, null))
				{
					lock (_dynamicTypes)
					{
						_dynamicTypes.Add(information.Id.Value, information.AssemblyQualifiedName);
					}
				}
			}

            return true;
        }

        public string GetTypeTableName(Type type)
        {
            return _tablesTypeNames[type];
        }

        /// <summary>
        /// Add a new type mapping.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        public void SetupTypeMapping(Type type, string tableName)
        {
            if (type.IsClass == false)
            {
                SystemMonitor.Error("Only class types allowed for mapping.");
                return;
            }

            _tablesTypeNames.Add(type, tableName);
        }

        /// <summary>
        /// Helper, generate SQL connection.
        /// </summary>
        /// <returns></returns>
        protected SQLiteConnection GenerateConnection()
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool ContainsTable(string tableName)
        {
            Dictionary<string, string> tables = GetTablesSchema();
            return tables != null && tables.ContainsKey(tableName);
        }

        /// <summary>
        /// Obtain the schema of the database tables.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTablesSchema()
        {
            if (_selectMutex.WaitOne(_defaultMutexTimeOut) == false)
            {
                SystemMonitor.OperationError("Failed to obtain select mutex.");
                return null;
            }

            DataSet set = new DataSet();

            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        // System information for SQLite is stored in "SQLITE_MASTER" table.
                        command.CommandText = "SELECT * FROM sqlite_master WHERE (type = 'table')";

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                        
                        connection.Open();

                        adapter.SelectCommand = command;
                        
                        adapter.Fill(set);
                    }
                }
            }
            finally
            {
                _selectMutex.ReleaseMutex();
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (set.Tables.Count == 0)
            {
                return null;
            }

            foreach (DataRow row in set.Tables[0].Rows)
            {
                result.Add(row["name"].ToString(), row["sql"].ToString());
            }

            return result;
        }

        /// <summary>
        /// Type persistable-ness is controled with the DBPersistenceAttribute attributes.
        /// Set to class, to define default for classes properties;
        /// Set to attribute to define specifics for this attribute;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowIdField"></param>
        /// <param name="xmlDataFields">Should we gather the XmlData serialization instructed fields.</param>
        /// <param name="requireFieldsExplicitMarking">Will only return properties that are marked with a positive value of the serialize attribute.</param>
        /// <returns></returns>
        public List<PropertyInfo> GetTypePersistableProperties(Type type, bool allowIdField, bool needReadAccess, 
            bool needWriteAccess, bool xmlDataColumnFields)
        {
            List<PropertyInfo> infos = new List<PropertyInfo>();

            DBPersistenceAttribute[] classAttributes = (DBPersistenceAttribute[])type.GetCustomAttributes(typeof(DBPersistenceAttribute), true);
            DBPersistenceAttribute.PersistenceTypeEnum defaultPersistenceType = DBPersistenceAttribute.PersistenceTypeEnum.Default;
            if (classAttributes.Length > 0)
            {
                defaultPersistenceType = classAttributes[0].PersistenceType;
            }

            foreach (PropertyInfo info in type.GetProperties())
            {
                DBPersistenceAttribute.PersistenceTypeEnum infoPersistenceType = defaultPersistenceType;
                DBPersistenceAttribute.PersistenceModeEnum infoPersistenceMode = DBPersistenceAttribute.PersistenceModeEnum.Default;

                bool isXmlDataField = false;

                object[] persistenceAttributes = info.GetCustomAttributes(typeof(DBPersistenceAttribute), true);
                if (persistenceAttributes.Length != 0)
                {// Property has its own attribute specifying persistance.
                    infoPersistenceType = ((DBPersistenceAttribute)persistenceAttributes[0]).PersistenceType;
                    infoPersistenceMode = ((DBPersistenceAttribute)persistenceAttributes[0]).PersistenceMode;
                    isXmlDataField = persistenceAttributes[0] is DBPersistenceToXmlDataAttribute;
                }

                if (infoPersistenceType == DBPersistenceAttribute.PersistenceTypeEnum.None)
                {// Marked not to be persisted.
                    continue;
                }

                if ((allowIdField == false && info.Name.ToLower() == "id") 
                    || (info.CanRead == false && needReadAccess)
                    || (info.CanWrite == false && needWriteAccess)
                    || (info.CanWrite == false && infoPersistenceMode == DBPersistenceAttribute.PersistenceModeEnum.Default))
                {// Id disallowed or field does not have proper read or write.
                    continue;
                }

                if (xmlDataColumnFields != isXmlDataField)
                {
                    continue;
                }

                infos.Add(info);
            }

            return infos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool GetTypeUsesXmlDataColumn(Type type)
        {
            List<PropertyInfo> xmlDataInfos = GetTypePersistableProperties(type, true, true, false, true);
            return xmlDataInfos != null && xmlDataInfos.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetItemXmlDataColumnData<ItemType>(ItemType item)
            where ItemType : class, IDBPersistent
        {
            string result = null;

            List<PropertyInfo> xmlDataInfos = GetTypePersistableProperties(typeof(ItemType), true, true, false, true);

            if (xmlDataInfos == null || xmlDataInfos.Count == 0)
            {
                return result;
            }

            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            XmlElement root = document.CreateElement("Root");
            document.AppendChild(root);

            foreach (PropertyInfo info in xmlDataInfos)
            {
                object value = ObtainPropertyValue(item, info);
                XmlElement element = document.CreateElement(info.Name);
                
                element.SetAttribute("Type", value.GetType().ToString());
                element.SetAttribute("Value", value.ToString());

                //XmlText text = document.CreateTextNode(value.ToString());
                //element.AppendChild(text);

                root.AppendChild(element);
            }

            return document.OuterXml;
        }

        /// <summary>
        /// Wrap those operations to handle cases like nullable, or types that do not persist 
        /// directly like the TimeSpan.
        /// In the case of getting no direct conversions needed.
        /// </summary>
        object ObtainPropertyValue(IDBPersistent item, PropertyInfo info)
        {
            DBPersistenceAttribute[] dbPersistence = (DBPersistenceAttribute[])info.GetCustomAttributes(typeof(DBPersistenceAttribute), true);
            if (dbPersistence.Length > 0)
            {
                SystemMonitor.CheckThrow(dbPersistence.Length == 1 && dbPersistence[0].PersistenceType != DBPersistenceAttribute.PersistenceTypeEnum.None, "Misteken usage of persistence attributes or internal error.");
                if (dbPersistence[0].PersistenceType == DBPersistenceAttribute.PersistenceTypeEnum.Binary)
                {
                    object val = info.GetValue(item, null);
                    if (val == null)
                    {
                        return null;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {// SaveState object persistance as BYTE[].
                        SerializationHelper.Serialize(ms, val);
                        ms.Flush();
                        return ms.GetBuffer();
                    }
                }
            }

            // Default case - no specific persistance instructions given.
            return info.GetValue(item, null);
        }

        /// <summary>
        /// Wrap those operations to handle cases like nullable, or types that do not persist 
        /// directly like the TimeSpan.
        /// </summary>
        bool HandleSetValue(IDBPersistent item, PropertyInfo info, object value)
        {
            Type propertyType = info.PropertyType;
            Type underlyingType = Nullable.GetUnderlyingType(info.PropertyType);
            if (underlyingType != null)
            {// Unwrap nullable properties.
                propertyType = underlyingType;

                if (value == null)
                {// Nullable enums with null values not displayed.
                    info.SetValue(item, null, null);
                    return true;
                }
            }

            object actualValue = value;
            DBPersistenceAttribute[] dbPersistence = (DBPersistenceAttribute[])info.GetCustomAttributes(typeof(DBPersistenceAttribute), true);
            if (dbPersistence.Length > 0)
            {
                SystemMonitor.CheckThrow(dbPersistence.Length == 1 && dbPersistence[0].PersistenceType != DBPersistenceAttribute.PersistenceTypeEnum.None, "Misteken usage of persistence attributes or internal error.");
                if (dbPersistence[0].PersistenceType == DBPersistenceAttribute.PersistenceTypeEnum.Binary)
                {// DeSerialize object persistence from BYTE[].
                    byte[] byteValue = (byte[])value;
                    SystemMonitor.CheckThrow(byteValue != null || byteValue.Length != 0, "Byte value size not expected at deserialization.");
                    using (MemoryStream ms = new MemoryStream(byteValue))
                    {
                        if (SerializationHelper.DeSerialize(ms, out actualValue) == false)
                        {// Failed to deserialize.
                            return false;
                        }
                    }
                }
            }
            else
            if (propertyType == typeof(TimeSpan))
            {
                actualValue = TimeSpan.Parse(value as string);
            }
            else if (propertyType.IsEnum)
            {
                actualValue = Enum.ToObject(propertyType, (int)value);
            }
            else if (propertyType == typeof(Uri))
            {
                actualValue = new Uri(value as string);
            }

            info.SetValue(item, actualValue, null);
            return true;
        }

		/// <summary>
		/// Helper, obtains an id of a dynamic type; or creates if not known.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="create"></param>
		/// <returns></returns>
		long? GetDynamicTypeId(Type type, bool create)
		{
			lock (_dynamicTypes)
			{
				if (_dynamicTypes.ContainsValue(type.AssemblyQualifiedName))
				{
					return _dynamicTypes.GetByValue(type.AssemblyQualifiedName);
				}
			}

			if (create)
			{
				DynamicTypeInformation information = new DynamicTypeInformation() { AssemblyQualifiedName = type.AssemblyQualifiedName };

				if (Insert<DynamicTypeInformation>(information) == false)
				{
					SystemMonitor.Error("Failed to insert dynamic type information entry [" + information.AssemblyQualifiedName + "].");
				}

				lock (_dynamicTypes)
				{
					_dynamicTypes.Add(information.Id.Value, information.AssemblyQualifiedName);
				}

				return information.Id;
			}

			return null;
		}

        /// <summary>
        /// Predefine, helper.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Insert<ItemType>(ItemType item)
            where ItemType : class, IDBPersistent
        {
            return Insert<ItemType>(new ItemType[] { item }, null);
        }

        /// <summary>
        /// Applicable only for classes, structs must use the redefined version.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="item"></param>
        /// <param name="fixedValue"></param>
        /// <returns></returns>
        public bool Insert<ItemType>(ItemType item,
            KeyValuePair<string, object>? fixedValue)
            where ItemType : class, IDBPersistent
        {
            if (fixedValue.HasValue)
            {
                Dictionary<string, object> inputFixedValues = new Dictionary<string, object>();
                inputFixedValues.Add(fixedValue.Value.Key, fixedValue.Value.Value);
                return Insert<ItemType>(new ItemType[] { item }, inputFixedValues);
            }
            else
            {
                return Insert<ItemType>(new ItemType[] { item }, null);
            }
        }

        /// <summary>
        /// Can be used both for classes or structs.
        /// </summary>
        public bool Insert<ItemType>(IEnumerable<ItemType> items,
            KeyValuePair<string, object> fixedValue)
            where ItemType : class, IDBPersistent
        {
            Dictionary<string, object> inputFixedValues = new Dictionary<string, object>();
            inputFixedValues.Add(fixedValue.Key, fixedValue.Value);

            return Insert<ItemType>(items, inputFixedValues);
        }

        /// <summary>
        /// Helper, works with same typed items only.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="items"></param>
        /// <param name="fixedValue"></param>
        /// <returns></returns>
        protected bool PerformInsertDynamicTyped<ItemType>(IEnumerable<ItemType> items, Type itemsActualType,
            KeyValuePair<string, object>? fixedValue)
            where ItemType : class, IDBPersistent
        {
            Dictionary<string, object> fixedValues = new Dictionary<string, object>();
            long? id = GetDynamicTypeId(itemsActualType, true);
            if (id.HasValue == false)
            {
                return false;
            }

            fixedValues.Add(_typeIdColumn, id.Value);

            if (fixedValue.HasValue)
            {
                fixedValues.Add(fixedValue.Value.Key, fixedValue.Value.Value);
            }

            return Insert<ItemType>(items, fixedValues);
        }

        /// <summary>
        /// Helper, allows to insert a dynamic type into the DB; as well as specify fixed values.
        /// *IMPORTANT* if the actual types of the items are different (from each other) a few separate
        /// inserts will be carried out, as well as an additional sorting of items by type, so for 
        /// optimization purposes it is best to evade this.
        /// </summary>
        /// <param name="items">The items to persist.</param>
        /// <param name="sameType">Are all the items, of the exactly same type.</param>
		public bool InsertDynamicTyped<ItemType>(IEnumerable<ItemType> items,
            KeyValuePair<string, object>? fixedValue)
            where ItemType : class, IDBPersistent
        {
            // Obtain a common type for all, if one exists.
            Type commonType = null;
            foreach (ItemType item in items)
            {
                if (commonType == null)
                {
                    commonType = item.GetType();
                }
                else if (commonType != item.GetType())
                {
                    commonType = null;
                    break;
                }
            }

            if (commonType != null)
            {// Same type insertion.
                return PerformInsertDynamicTyped(items, commonType, fixedValue);
            }
            else
            {// Multi type, sort by type before inserting.
                Dictionary<Type, List<ItemType>> itemsByType = new Dictionary<Type, List<ItemType>>();

                foreach (ItemType item in items)
                {
                    Type type = item.GetType();
                    if (itemsByType.ContainsKey(type) == false)
                    {
                        itemsByType.Add(type, new List<ItemType>());
                    }

                    itemsByType[type].Add(item);
                }

                bool result = true;
                foreach (KeyValuePair<Type, List<ItemType>> pair in itemsByType)
                {
                    result = result && PerformInsertDynamicTyped(pair.Value, pair.Key, fixedValue);
                }

                return result;
            }
        }

        /// <summary>
        /// Helper, allows to insert a dynamic type into the DB.
        /// </summary>
        public bool InsertDynamicTyped<ItemType>(ItemType item)
            where ItemType : class, IDBPersistent
        {
            return InsertDynamicTyped<ItemType>(new ItemType[] { item }, null);
        }

        /// <summary>
        /// Can be used both for classes or structs.
        /// Slow (?!), using reflection (Invokes). Can be sped up using this approach:
        /// http://blog.lab49.com/archives/446
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="items"></param>
        /// <param name="fixedValues">Can contain fixed or additional values.</param>
        /// <param name="hasObjectColumn">Object column contains a full serialization of the object.</param>
        /// <param name="nameMap">Pass null to evada mapping.</param>
        /// <returns></returns>
        public bool Insert<ItemType>(IEnumerable<ItemType> items, Dictionary<string, object> fixedValues)
            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);

            string tableName = _tablesTypeNames[typeof(ItemType)];

            List<PropertyInfo> infos = GetTypePersistableProperties(typeof(ItemType), true, true, false, false);

            StringBuilder commandText = new StringBuilder("INSERT INTO " + tableName + " ");

            List<string> additionalValuesNames = new List<string>();
            if (fixedValues != null)
            {
                additionalValuesNames.AddRange(fixedValues.Keys);
            }

            bool typeUsesXmlDataColumn = GetTypeUsesXmlDataColumn(typeof(ItemType));

            foreach (PropertyInfo info in infos.ToArray())
            {
                if (additionalValuesNames.Contains(info.Name))
                {
                    infos.Remove(info);
                }
                //additionalValuesNames.Remove(info.Name);
            }

            {// Column names.
                commandText.Append(" ( ");

                for (int i = 0; i < infos.Count; i++)
                {
                    string name = infos[i].Name;

                    if (fixedValues != null && fixedValues.ContainsKey(name) && fixedValues[name] == null)
                    {
                        continue;
                    }

                    commandText.Append(name + ",");
                }

                // Additional values (Fixed values may contain additional values)
                foreach (string valueName in additionalValuesNames)
                {
                    commandText.Append(valueName + ",");
                }

                if (typeUsesXmlDataColumn)
                {
                    commandText.Append(_xmlDataColumnName + ",");
                }

                if (commandText[commandText.Length - 1] == ',')
                {// Fix final comma (if there is one).
                    commandText.Remove(commandText.Length - 1, 1);
                }

                commandText.Append(" ) ");
            }

            commandText.Append(" VALUES ");

            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            List<PropertyInfo> parametersInfos = new List<PropertyInfo>();

            SQLiteParameter xmlDataColumnParameter = null;

            if (typeUsesXmlDataColumn)
            {
                xmlDataColumnParameter = new SQLiteParameter("@" + _xmlDataColumnName);
            }

            //SQLiteParameter objectParameter = null;

            _insertMutex.WaitOne();

            try
            {

                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        {// Command parameter.
                            commandText.Append(" ( ");

                            for (int j = 0; j < infos.Count; j++)
                            {
                                string name = infos[j].Name;

                                if (fixedValues != null && fixedValues.ContainsKey(name) && fixedValues[name] == null)
                                {
                                    continue;
                                }

                                string paramName = "@Param" + infos[j].Name;

                                commandText.Append(paramName + ",");

                                SQLiteParameter parameter = new SQLiteParameter(paramName);
                                command.Parameters.Add(parameter);
                                parameters.Add(parameter);
                                parametersInfos.Add(infos[j]);
                            }

                            foreach (string name in additionalValuesNames)
                            {
                                commandText.Append("@ParamFixed" + name + ",");
                                command.Parameters.Add(new SQLiteParameter("@ParamFixed" + name, fixedValues[name]));
                            }

                            if (typeUsesXmlDataColumn)
                            {
                                commandText.Append(xmlDataColumnParameter.ParameterName + ",");
                                command.Parameters.Add(xmlDataColumnParameter);
                            }

                            if (commandText[commandText.Length - 1] == ',')
                            {// Fix final comma (if there is one).
                                commandText.Remove(commandText.Length - 1, 1);
                            }

                            commandText.Append(")");
                        }

                        command.CommandText = commandText.ToString();

                        connection.Open();
                        using (SQLiteTransaction mytransaction = connection.BeginTransaction())
                        {
                            foreach (ItemType item in items)
                            {
                                SystemMonitor.CheckThrow(item.Id.HasValue == false, "Inserting an item with ID already assigned.");

                                for (int j = 0; j < parameters.Count; j++)
                                {
                                    parameters[j].Value = ObtainPropertyValue(item, parametersInfos[j]);
                                }

                                if (typeUsesXmlDataColumn)
                                {
                                    xmlDataColumnParameter.Value = GetItemXmlDataColumnData<ItemType>(item);
                                }

                                //if (objectParameter != null)
                                //{
                                //    using (MemoryStream stream = new MemoryStream())
                                //    {
                                //        SerializationHelper.Serialize(stream, item);
                                //        objectParameter.Value = stream.GetBuffer();
                                //    }
                                //}

                                if (command.ExecuteNonQuery() != 1)
                                {
                                    SystemMonitor.Error("Command query execution error.");
                                }

                                long insertId = GetLastInsertRowId(connection, tableName);
                                item.Id = insertId;
                            }

                            mytransaction.Commit();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("Failed to insert item [" + typeof(ItemType).ToString() + "].", ex);
            }
            finally
            {
                _insertMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return true;
        }

        /// <summary>
        /// Useful for executing create table commands.
        /// </summary>
        /// <param name="command"></param>
        public object ExecuteCommand(string commandText)
        {
            TracerHelper.TraceEntry(commandText);

            object result = null;

            _executeMutex.WaitOne();

            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = commandText;
                        connection.Open();

                        using (SQLiteTransaction mytransaction = connection.BeginTransaction())
                        {
                            result = command.ExecuteNonQuery();
                            mytransaction.Commit();
                        }
                    }
                }
            }
            finally
            {
                _executeMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateToDB<ItemType>(ItemType item)
                                            where ItemType : class, IDBPersistent
        {
            return UpdateToDB<ItemType>(item, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateToDB<ItemType>(ItemType item,
                                            Dictionary<string, object> fixedValues)
                                            where ItemType : class, IDBPersistent
        {
            return UpdateToDB<ItemType>(new ItemType[] { item }, fixedValues);
        }

        /// <summary>
        /// Slow (?!), using reflection (Invokes). Invokes take about 20% of the time.
        /// Can be sped up using this approach:
        /// http://blog.lab49.com/archives/446
        /// </summary>
        public bool UpdateToDB<ItemType>(IEnumerable<ItemType> items,
                                            Dictionary<string, object> fixedValues)
                                            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);

            string tableName = _tablesTypeNames[typeof(ItemType)];

            List<PropertyInfo> infos = GetTypePersistableProperties(typeof(ItemType), false, true, false, false);

            StringBuilder commandText = new StringBuilder("UPDATE " + tableName + " ");

            List<string> additionalValuesNames = new List<string>();
            if (fixedValues != null)
            {
                additionalValuesNames.AddRange(fixedValues.Keys);
            }

            foreach (PropertyInfo info in infos)
            {
                additionalValuesNames.Remove(info.Name);
            }


            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            List<PropertyInfo> parametersInfos = new List<PropertyInfo>();

            //SQLiteParameter objectParameter = null;

            _updateMutex.WaitOne();

            try
            {

                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        {// Column names.
                            commandText.Append(" SET ");

                            for (int i = 0; i < infos.Count; i++)
                            {
                                string name = infos[i].Name;

                                if (fixedValues != null && fixedValues.ContainsKey(name) && fixedValues[name] == null)
                                {
                                    continue;
                                }

                                commandText.Append(name + " = ");

                                string paramName = "@Param" + infos[i].Name;

                                commandText.Append(paramName + ",");
                                SQLiteParameter parameter = new SQLiteParameter(paramName);
                                command.Parameters.Add(parameter);
                                parameters.Add(parameter);
                                parametersInfos.Add(infos[i]);
                            }

                            // Additional fixed values (Fixed values may contain additional values)
                            foreach (string name in additionalValuesNames)
                            {
                                commandText.Append(name + " = ");
                                commandText.Append("@ParamFixed" + name + ",");
                                command.Parameters.Add(new SQLiteParameter("@ParamFixed" + name, fixedValues[name]));
                            }

                            if (commandText[commandText.Length - 1] == ',')
                            {// Fix final comma (if there is one).
                                commandText.Remove(commandText.Length - 1, 1);
                            }

                            //commandText.Append(" ) ");
                        }

                        SQLiteParameter whereIdParam = new SQLiteParameter("@ParamId");
                        command.Parameters.Add(whereIdParam);
                        commandText.Append(" WHERE Id = " + whereIdParam.ParameterName);

                        command.CommandText = commandText.ToString();
                        connection.Open();
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            foreach(ItemType item in items)
                            {
                                SystemMonitor.CheckThrow(item.Id.HasValue, "Updating an item with no ID already assigned.");

                                for (int j = 0; j < parameters.Count; j++)
                                {
                                    parameters[j].Value = ObtainPropertyValue(item, parametersInfos[j]);
                                }

                                //if (objectParameter != null)
                                //{
                                //    using (MemoryStream stream = new MemoryStream())
                                //    {
                                //        SerializationHelper.Serialize(stream, item);
                                //        objectParameter.Value = stream.GetBuffer();
                                //    }
                                //}

                                whereIdParam.Value = item.Id;
                                if (command.ExecuteNonQuery() != 1)
                                {
                                    SystemMonitor.Error("Command query execution error.");
                                }
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
            finally
            {
                _updateMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ClearTable<ItemType>()
            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);
            
            string tableName = _tablesTypeNames[typeof(ItemType)];
            
            _clearMutex.WaitOne();
            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        connection.Open();

                        command.CommandText = "DELETE FROM " + tableName;
                        int result = command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                _clearMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return true;
        }

        /// <summary>
        /// Delete corresponding item from DB.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Delete<ItemType>(ItemType item)
            where ItemType : class, IDBPersistent
        {
            return Delete<ItemType>(new ItemType[] { item });
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Delete<ItemType>(IEnumerable<ItemType> items)
            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);
            string tableName = _tablesTypeNames[typeof(ItemType)];
            
            _deleteMutex.WaitOne();
            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        connection.Open();

                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            SQLiteParameter whereIdParam = new SQLiteParameter("@ParamId");
                            command.Parameters.Add(whereIdParam);

                            command.CommandText = "DELETE FROM " + tableName + " WHERE Id = " + whereIdParam.ParameterName;

                            foreach (IDBPersistent persistent in items)
                            {
                                SystemMonitor.CheckThrow(persistent.Id.HasValue, "Deleting an item with no ID already assigned.");

                                whereIdParam.Value = persistent.Id;
                                int result = command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }

                    }
                }
            }
            finally
            {
                _deleteMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return true;
        }

        /// <summary>
        /// Needs to be on the same connection the insert was in.
        /// </summary>
        /// <returns></returns>
        long GetLastInsertRowId(SQLiteConnection connection, string tableName)
        {
            TracerHelper.TraceEntry(tableName);

            long value;
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT last_insert_rowid() FROM " + tableName;
                value = (long)command.ExecuteScalar();
            }

            TracerHelper.TraceExit();
            return value;
        }

        /// <summary>
        /// Check if the object already exists in the DB (any version of the object).
        /// </summary>
        public bool IsPersisted<ItemType>(ItemType item)
            where ItemType : class, IDBPersistent
        {
            return Count<ItemType>(new MatchExpression("Id", item.Id)) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Count<ItemType>(MatchExpression matchExpression)
            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);

            string tableName = _tablesTypeNames[typeof(ItemType)];

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT count(*) FROM " + tableName);

            if (matchExpression != null && matchExpression.ClauseCount > 0)
            {
                commandText.Append(" WHERE ");
            }

            _countMutex.WaitOne();
            SQLiteCommand command;
            long result = 0;
            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (command = new SQLiteCommand(connection))
                    {
                        if (matchExpression != null)
                        {
                            matchExpression.SetupCommandParameters(command, commandText);
                        }

                        command.CommandText = commandText.ToString();

                        connection.Open();
                        result = (long)command.ExecuteScalar();
                    }
                }
            }
            finally
            {
                _countMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return result;
        }

        /// <summary>
        /// Single selection.
        /// </summary>
        /// <returns></returns>
        public bool SelectScalar<ItemType>(ItemType item, MatchExpression matchExpression)
            where ItemType : class, IDBPersistent
        {
            DataSet set = Select(new string[] { GetTypeTableName(typeof(ItemType)) }, matchExpression, 1);
            if (set != null && set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
            {
                return UpdateItemValues<ItemType>(item, GetTypePersistableProperties(typeof(ItemType), true, true, true, false), set.Tables[0].Rows[0]);
            }
            return false;
        }

        /// <summary>
        /// Single selection.
        /// </summary>
        /// <returns></returns>
        public ItemType SelectScalar<ItemType>(MatchExpression matchExpression)
            where ItemType : class, IDBPersistent, new()
        {
            List<ItemType> results = Select<ItemType>(matchExpression, 1);

            if (results.Count > 0)
            {
                return results[0];
            }
            return null;
        }

        protected bool UpdateItemValues<ItemType>(ItemType item, List<PropertyInfo> infos, DataRow row)
            where ItemType : class, IDBPersistent
        {
            foreach (PropertyInfo info in infos)
            {
                string name = info.Name;
                if (row[name] != DBNull.Value)
                {
                    if (HandleSetValue(item, info, row[name]) == false)
                    {
                        return false;
                    }
                }
            }

            SystemMonitor.CheckError(item.Id.HasValue, "Updated item was not assigned Id.");
            return true;
        }

        /// <summary>
        /// This will create object instances based on the type column names. All instances types must be child
        /// types to the BaseItemType. 
        /// This is useful to store multiple types with same base type in a single table.
        /// </summary>
        public List<ItemBaseType> SelectDynamicType<ItemBaseType>(MatchExpression matchExpression, int? limit)
            where ItemBaseType : class, IDBPersistent
        {
            List<ItemBaseType> results = new List<ItemBaseType>();

            string tableName = _tablesTypeNames[typeof(ItemBaseType)];
            List<PropertyInfo> infos = this.GetTypePersistableProperties(typeof(ItemBaseType), true, true, true, false);
            
            DataSet set = Select(new string[] { tableName }, matchExpression, limit);
            foreach (DataRow row in set.Tables[0].Rows)
            {
				if (row.IsNull(_typeIdColumn) &&
					 string.IsNullOrEmpty(_typeIdColumn))
                {
                    SystemMonitor.OperationError("Type id column not found.");
                    return null;
                }

				long typeId = (long)row[_typeIdColumn];

				Type type;

				lock (_dynamicTypes)
				{
					if (_dynamicTypes.ContainsKey(typeId) == false)
					{
						SystemMonitor.OperationError("Failed to find dynamic type, id [" + typeId + "].");
						return null;
					}

					type = Type.GetType(_dynamicTypes[typeId]);
				}


				if (type == null)
                {
					SystemMonitor.OperationError("Type provided was not found.");
                    return null;
                }

				if (type.IsSubclassOf(typeof(ItemBaseType)) == false &&
                    type.GetInterface(typeof(ItemBaseType).Name) == null)
                {
                    SystemMonitor.OperationError("Base type not corresponding.");
                    return null;
                }

                ConstructorInfo constructor = type.GetConstructor(new Type[] { });
                if (constructor == null)
                {
                    SystemMonitor.Error("Type parameterless constructor not found.");
                    return null;
                }

				ItemBaseType item = (ItemBaseType)constructor.Invoke(null);
                if (UpdateItemValues(item, infos, row))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        /// Helper for using objects that were serialized in the dataDelivery Column field of their corresponding rows.
        /// </summary>
        /// <typeparam name="ItemBaseType"></typeparam>
        /// <param name="matchExpression"></param>
        /// <param name="dataColumnName">Name of the column that contains the blob - serialized information.</param>
        /// <param name="limit"></param>
        /// <param name="failedSerializations">May be null, to skip. A list of the rows Ids containing the rows that failed to deserialize.</param>
        /// <returns></returns>
        public List<ItemBaseType> SelectSerializedType<ItemBaseType>(MatchExpression matchExpression,
            string dataColumnName, int? limit, List<long> failedSerializations)
            where ItemBaseType : class, IDBPersistent
        {
            List<ItemBaseType> results = new List<ItemBaseType>();

            string tableName = _tablesTypeNames[typeof(ItemBaseType)];
            List<PropertyInfo> infos = this.GetTypePersistableProperties(typeof(ItemBaseType), true, true, true, false);
            DataSet set = Select(new string[] { tableName }, matchExpression, limit);
            foreach (DataRow row in set.Tables[0].Rows)
            {
                ItemBaseType item = null;
                if (row[dataColumnName] == System.DBNull.Value)
                {
                    SystemMonitor.Error("Row has no data assigned in data column.");
                    continue;
                }

                object value;
                if (SerializationHelper.DeSerialize((byte[])row[dataColumnName], out value) == false)
                {// Item failed to deserialize, error already reported, so just continue.
                    if (failedSerializations != null)
                    {
                        failedSerializations.Add((long)row["Id"]);
                    }
                    continue;
                }

                item = (ItemBaseType)value;

                if (UpdateItemValues(item, infos, row))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        /// Select only columns from a given type table.
        /// Allows to extract parts only of the information for an object.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="matchExpression"></param>
        /// <param name="columnsNames"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<object[]> SelectColumns<ItemType>(MatchExpression matchExpression, string[] columnsNames, int? limit)
            where ItemType : class, IDBPersistent
        {
            List<ItemType> results = new List<ItemType>();

            string tableName = _tablesTypeNames[typeof(ItemType)];
            //List<PropertyInfo> infos = this.GetTypePersistableProperties(typeof(ItemType), true, true, true);
            DataSet set = Select(new string[] { tableName }, matchExpression, limit);

            List<object[]> result = new List<object[]>();
            foreach (DataRow row in set.Tables[0].Rows)
            {
                List<object> objects = new List<object>();
                for (int i = 0; i < columnsNames.Length; i++)
                {
                     objects.Add(row[columnsNames[i]]);
                }

                result.Add(objects.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Extract type instances from storage, matching the provided criterias.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="matchExpression">Pass null to include all items in select.</param>
        /// <param name="limit">Pass null to specify no limit.</param>
        /// <returns></returns>
        public List<ItemType> Select<ItemType>(MatchExpression matchExpression, int? limit)
            where ItemType : class, IDBPersistent, new()
        {
            List<ItemType> results = new List<ItemType>();

            string tableName = _tablesTypeNames[typeof(ItemType)];
            List<PropertyInfo> infos = this.GetTypePersistableProperties(typeof(ItemType), true, true, true, false);
            DataSet set = Select(new string[] { tableName }, matchExpression, limit);
            foreach (DataRow row in set.Tables[0].Rows)
            {
                ItemType item = new ItemType();
                if (UpdateItemValues(item, infos, row))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSet Select(string[] tablesNames, MatchExpression matchExpression, int? limit)
        {
            string names = "";
            for (int i = 0; i < tablesNames.Length; i++)
            {
                names += tablesNames[i];
                if (i != tablesNames.Length - 1)
                {
                    names += ",";
                }
            }

            TracerHelper.TraceEntry(names);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT * FROM " + names);

            if (matchExpression != null && matchExpression.ClauseCount > 0)
            {
                commandText.Append(" WHERE ");
            }
            
            _selectMutex.WaitOne();

            DataSet set = new DataSet();

            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        if (matchExpression != null)
                        {
                            matchExpression.SetupCommandParameters(command, commandText);
                        }

                        if (limit.HasValue)
                        {
                            commandText.Append(" LIMIT " + limit.Value);
                        }

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter();

                        command.CommandText = commandText.ToString();
                        connection.Open();

                        adapter.SelectCommand = command;
                        adapter.Fill(set);
                    }
                }
            }
            finally
            {
                _selectMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return set;
        }

        /// <summary>
        /// If match expression is null will delete all entries in table.
        /// </summary>
        public int Delete<ItemType>(MatchExpression matchExpression)
            where ItemType : class, IDBPersistent
        {
            TracerHelper.TraceEntry(typeof(ItemType).Name);

            string tableName = _tablesTypeNames[typeof(ItemType)];

            StringBuilder commandText = new StringBuilder();
            commandText.Append("DELETE FROM " + tableName);

            if (matchExpression != null && matchExpression.ClauseCount > 0)
            {
                commandText.Append(" WHERE ");
            }

            _deleteMutex.WaitOne();
            int result;
            try
            {
                using (SQLiteConnection connection = GenerateConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        if (matchExpression != null)
                        {
                            matchExpression.SetupCommandParameters(command, commandText);
                        }

                        connection.Open();
                        command.CommandText = commandText.ToString();
                        result = command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                _deleteMutex.ReleaseMutex();
            }

            TracerHelper.TraceExit();
            return result;
        }

        ///// <summary>
        ///// Helper.
        ///// </summary>
        //static DataTable GetTableValuesById(string tableName, long[] ids, int limit)
        //{
        //    StringBuilder commandText = new StringBuilder();
        //    commandText.Append("SELECT * FROM " + tableName + " WHERE Id IN (");

        //    for (int i = 0; i < ids.Length; i++)
        //    {
        //        if (i != ids.Length - 1)
        //        {
        //            commandText.Append(ids[i].ToString() + ",");
        //        }
        //        else
        //        {
        //            commandText.Append(ids[i].ToString() + ")");
        //        }
        //    }

        //    if (limit > 0)
        //    {
        //        commandText.Append(" LIMIT " + limit.ToString());
        //    }

        //    DataSet set = new DataSet();
        //    SQLiteDataAdapter adapter = new SQLiteDataAdapter();

        //    using (SQLiteConnection connection = GenerateConnection())
        //    {
        //        connection.Open();
        //        using (SQLiteTransaction mytransaction = connection.BeginTransaction())
        //        {
        //            using (SQLiteCommand mycommand = new SQLiteCommand(connection))
        //            {
        //                mycommand.CommandText = commandText.ToString();

        //                adapter.SelectCommand = mycommand;
        //                adapter.Fill(set);

        //                return set.Tables[0];
        //            }
        //        }
        //        //mytransaction.Commit();
        //    }
        //}

    }
}
