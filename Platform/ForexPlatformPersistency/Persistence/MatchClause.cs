using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
    internal struct MatchClause
    {
        public enum OperationEnum
        {
            NA, // Initial clause no conjunction operation.
            AND,
            OR
        }

        OperationEnum _operation;
        public OperationEnum Operation
        {
            get { return _operation; }
        }

        string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
        }

        object _value;
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatchClause(OperationEnum operation, string fieldName, object value)
        {
            _operation = operation;
            _value = value;
            _fieldName = fieldName;
        }
    }
}
