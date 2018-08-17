using System;
using System.Collections.Generic;
using System.Text;
using CommonSupport;
using System.Data.Common;
using System.Data.SQLite;

namespace CommonSupport
{
    /// <summary>
    /// A single match expression can contain multiple causes.
    /// A match expression represents the condition used to search items for in a Database.
    /// </summary>
    public class MatchExpression
    {
        List<MatchClause> _clauses = new List<MatchClause>();

        public int ClauseCount
        {
            get { return _clauses.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatchExpression()
        {
        }

        /// <summary>
        /// Single clause expression
        /// </summary>
        public MatchExpression(string fieldName, object value)
        {
            MatchClause clause = new MatchClause(MatchClause.OperationEnum.NA, fieldName, value);
            _clauses.Add(clause);
        }

        /// <summary>
        /// The initial clause has no AND or OR, since it is at this moment alone. The following clauses
        /// specify relations to previous clauses.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void AddInitialClause(string fieldName, object value)
        {
            SystemMonitor.CheckThrow(ClauseCount == 0);
            _clauses.Add(new MatchClause(MatchClause.OperationEnum.NA, fieldName, value));
        }

        /// <summary>
        /// Make sure this is not the initial clause, since operation (AND) pre-pended to the current operation.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void AddANDClause(string fieldName, object value)
        {
            SystemMonitor.CheckThrow(ClauseCount != 0);
            _clauses.Add(new MatchClause(MatchClause.OperationEnum.AND, fieldName, value));
        }

        /// <summary>
        /// Make sure this is not the initial clause, since operation (OR) pre-pended to the current operation.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void AddORClause(string fieldName, object value)
        {
            SystemMonitor.CheckThrow(ClauseCount != 0);
            _clauses.Add(new MatchClause(MatchClause.OperationEnum.OR, fieldName, value));
        }

        public void SetupCommandParameters(DbCommand command, StringBuilder commandText)
        {
            for (int i = 0; i < _clauses.Count; i++)
            {
                MatchClause clause = _clauses[i];
                SQLiteParameter parameter = new SQLiteParameter("@" + clause.FieldName, clause.Value);

                if (i != 0 && clause.Operation != MatchClause.OperationEnum.NA)
                {
                    if (clause.Operation == MatchClause.OperationEnum.AND)
                    {
                        commandText.Append(" AND ");
                    }
                    else if (clause.Operation == MatchClause.OperationEnum.OR)
                    {
                        commandText.Append(" OR ");
                    }
                }

                commandText.Append(" (" + clause.FieldName + " = " + parameter.ParameterName + ")");
                command.Parameters.Add(parameter);
            }
        }

    }
}
