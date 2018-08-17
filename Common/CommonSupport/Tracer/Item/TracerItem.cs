using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;

namespace CommonSupport
{
    /// <summary>
    /// Base class for Tracer Items. Each item represents a call to the tracer
    /// system to record a given event. Items are processed, filtered, "sinked"
    /// into other destinations etc.
    /// </summary>
    [Serializable]
    public class TracerItem
    {
        /// <summary>
        /// Also combinations possible, like Operation-Error, or Operation-Warning
        /// </summary>
        public enum TypeEnum
        {
            MethodEntry = 1,
            MethodExit = 2,
            Trace = 4,
			Report = 8,
			System = 16,
            Warning = 32,
			Error = 64,
            Operation = 1024
        }

        /// <summary>
        /// Non-combinationary, each item must have assigned priority.
        /// </summary>
        public enum PriorityEnum
        {
            Trivial = 1,
            Minimum = Trivial,
            VeryLow = 3,
            Low = 5,
            Medium = 10,
            Default = Medium,
            High = 20,
            VeryHigh = 25,
            Critical = 50
        }

        volatile PriorityEnum _priority = PriorityEnum.Medium;
        public PriorityEnum Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        volatile TypeEnum[] _itemTypes = new TypeEnum[] { };
        /// <summary>
        /// A single tracer item can have multiple combined types assigned to it.
        /// </summary>
        public TypeEnum[] Types
        {
            get { return _itemTypes; }
        }

        TypeEnum _fullType;
        /// <summary>
        /// May contain multiple values.
        /// </summary>
        public TypeEnum FullType
        {
            get { return _fullType; }
        }

        DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
        }

        long _applicationTick;
        /// <summary>
        /// The time the item was created, in ticks from start of application.
        /// </summary>
        public long ApplicationTick
        {
            get { return _applicationTick; }
            set { _applicationTick = value; }
        }

        /// <summary>
        /// May be null.
        /// </summary>
        public virtual Assembly Assembly
        {
            get { return null; }
        }

        volatile string _message;
        public string Message
        {
            get { return _message; }
        }

        string[] _attachedObjectsPrints = new string[] { };
        public string[] AttachedObjectsPrints
        {
            get { return _attachedObjectsPrints; }
        }

        long _index = -1;
        /// <summary>
        /// Index is assigned by tracer.
        /// </summary>
        public long Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TracerItem(TypeEnum itemType, PriorityEnum priority, string message)
        {
            _priority = priority;
            _fullType = itemType;
            _itemTypes = GeneralHelper.GetCombinedEnumValues(itemType).ToArray();
            _dateTime = DateTime.Now;
            _applicationTick = GeneralHelper.ApplicationStopwatchTicks;
            _message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public TracerItem(TypeEnum itemType, DateTime time, PriorityEnum priority, string message)
        {
            _priority = priority;
            _fullType = itemType;
            _itemTypes = GeneralHelper.GetCombinedEnumValues(itemType).ToArray();
            _dateTime = time;
            _applicationTick = GeneralHelper.ApplicationStopwatchTicks;
            _message = message;
        }

        /// <summary>
        /// Attach prints of objects related to this tracer item. This is not mandatory.
        /// </summary>
        public void AttachObjectsPrints(ITracerObject[] tracerObjects)
        {
            if (tracerObjects == null || tracerObjects.Length == 0)
            {
                return;
            }

            _attachedObjectsPrints = new string[tracerObjects.Length];
            for (int i = 0; i < tracerObjects.Length; i++)
            {
                _attachedObjectsPrints[i] = tracerObjects[i].Print();
            }
        }

        /// <summary>
        /// Obtain the prefix part for a print of an item.
        /// </summary>
        /// <returns></returns>
        string GetPrintPrefix()
        {
            switch (_fullType)
            {
                case TracerItem.TypeEnum.MethodEntry:
                    return ">>";
                case TracerItem.TypeEnum.MethodExit:
                    return "<<";
                case TracerItem.TypeEnum.Trace:
                    break;
                default:
                    return "[" + GeneralHelper.ToString(_itemTypes, "_").ToUpper() + "]";
            }

            return string.Empty;
        }

        /// <summary>
        /// Print item information into one line string.
        /// </summary>
        /// <returns></returns>
        public virtual string PrintPrefix(char separator)
        {
            return string.Format("{0}" + separator + "{1}" + separator+ "{2}", _index.ToString("000000"), 
                _dateTime.ToString("dd/MM/yyyy HH:mm:ss.FFFFF"/*GeneralHelper.UniversalNumberFormatInfo*/), GetPrintPrefix());
        }

        public virtual string PrintMessage()
        {
            return _message;
        }

        public static TracerItem ParseFileItem(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            try
            {
                string[] substrings = line.Split('|');

                if (substrings.Length < 4)
                {
                    SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", Not enough substrings generated].");
                    return null;
                }

                TracerItem.TypeEnum fullType = (TracerItem.TypeEnum)int.Parse(substrings[0]);
                TracerItem.PriorityEnum priority = (TracerItem.PriorityEnum)int.Parse(substrings[1]);

                long index = 0;
                long.TryParse(substrings[2], out index);

                DateTime time;
                try
                {
                    string dateTime = substrings[3];
                    string[] dateTimeParts = dateTime.Split('/');
                    string[] subParts = dateTimeParts[2].Split(' ');
                    TimeSpan timeSpan = TimeSpan.Parse(subParts[1]);

                    time = new DateTime(int.Parse(subParts[0]), int.Parse(dateTimeParts[1]),
                        int.Parse(dateTimeParts[0]), timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                }
                catch (Exception ex2)
                {
                    SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", " + ex2.Message + "].");
                    time = DateTime.MinValue;
                }

                return new TracerItem(fullType, time, priority, index.ToString() + "  " + substrings[substrings.Length - 1]);
            }
            catch (Exception ex)
            {
                SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", " + ex.Message + "].");
                return null;
            }
        }

        public virtual string PrintFileLine()
        {
            return ((int)FullType).ToString() + '|'+ ((int)Priority).ToString() + '|' + PrintPrefix('|') + '|' + PrintMessage();
        }


    }

}
