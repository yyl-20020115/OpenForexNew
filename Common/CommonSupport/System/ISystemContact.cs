//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonSupport
//{
//    public interface ISystemContact
//    {
//        //static readonly public SystemVariables Variables = new SystemVariables();

//        ///// <summary>
//        ///// Control the global diagnostics mode; when set it 
//        ///// will not only log, but also raise occuring errors 
//        ///// and warnings directly to top level attention.
//        ///// </summary>
//        //static public volatile bool GlobalDiagnosticsMode = false;

//        /// <summary>
//        /// Convert a set of parameters to string.
//        /// </summary>
//        public static string ParametersToString(params string[] parameters);

//        /// <summary>
//        /// Check condition, if false, report a NotImplementedCritical condition.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="parameters"></param>
//        public static void CheckNotImplementedCritical(bool condition, params string[] parameters)
//        {
//            if (condition == false)
//            {
//                NotImplementedCritical(parameters);
//            }
//        }

//        /// <summary>
//        /// Report a piece of code is not implemented.
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void NotImplementedError(params string[] parameters)
//        {
//            Error("Error: Not Implemented [" + ParametersToString(parameters) + "]");
//        }

//        /// <summary>
//        /// Report a piece of code is not implemented, however missing functionality is not critical to operation.
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void NotImplementedWarning(params string[] parameters)
//        {
//            Warning("Warning: Not Implemented [" + ParametersToString(parameters) + "]");
//        }

//        /// <summary>
//        /// Throws a NotImplementedException exception.
//        /// </summary>
//        public static void NotExpectedCritical(params string[] parameters)
//        {
//            throw new NotImplementedException("This call was not expected [" + ParametersToString(parameters) + "].");
//        }

//        /// <summary>
//        /// Report a piece of code is not implemented, and missing functionality is critical to operation.
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void NotImplementedCritical(params string[] parameters)
//        {
//            throw new NotImplementedException("The calling method is not implemented [" + ParametersToString(parameters) + "].");
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void NotSupported(params string[] parameters)
//        {
//            throw new NotImplementedException("The calling method is not supported [" + ParametersToString(parameters) + "].");
//        }

//        /// <summary>
//        /// Report an invalid call.
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void InvalidCall(params string[] parameters)
//        {
//            throw new Exception("The calling method is not valid to call [" + ParametersToString(parameters) + "].");
//        }

//        /// <summary>
//        /// Perform a check on a condition. 
//        /// If the conditions is *false* set error.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="errorMessage"></param>
//        public static void CheckError(bool condition)
//        {
//            CheckError(condition, string.Empty);
//        }

//        /// <summary>
//        /// Perform a check on a condition. If the conditions is *false* set error with specified message.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="errorMessage"></param>
//        public static void CheckError(bool condition, string errorMessage)
//        {
//            if (condition == false)
//            {
//                Error(errorMessage, TracerItem.PriorityEnum.VeryHigh);
//            }
//        }

//        /// <summary>
//        /// Check given condition, if false, report operation error with given message.
//        /// Operation errors are a lighter version of a error, and may be expected to 
//        /// occur during normal operation of the application (for. ex. a given non critical 
//        /// resource was not retrieved, operation has timed out etc.)
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="errorMessage"></param>
//        public static void CheckOperationError(bool condition, string errorMessage)
//        {
//            if (condition == false)
//            {
//                OperationError(errorMessage);
//            }
//        }

//        /// <summary>
//        /// Chech given condition, if false, report operation warning with given message.
//        /// Operation warnings are a lighter version of a warning, and may be expected to 
//        /// occur during normal operaiton of the application.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="message"></param>
//        public static void CheckOperationWarning(bool condition, string message)
//        {
//            if (condition == false)
//            {
//                OperationWarning(message);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public static void CheckOperationWarning(bool condition, string message, TracerItem.PriorityEnum priority)
//        {
//            if (condition == false)
//            {
//                OperationWarning(message, priority);
//            }
//        }

//        /// <summary>
//        /// Check condition and report a warning, if condition not met.
//        /// Warnings serve to notify that something in operation of application has gone
//        /// wrong, however the error is not critical.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="parameters"></param>
//        public static void CheckWarning(bool condition, params string[] parameters)
//        {
//            if (condition == false)
//            {
//                Warning("Warning: " + ParametersToString(parameters));
//            }
//        }

//        /// <summary>
//        /// Will report only if condition fails.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="reportMessage"></param>
//        public static void CheckReport(bool condition, string reportMessage)
//        {
//            if (condition == false)
//            {
//                Report(reportMessage);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public static void Report(string reportMessage)
//        {
//            Report(reportMessage, TracerItem.PriorityEnum.Trivial);
//        }

//        /// <summary>
//        /// Report a simplme message to the report management system.
//        /// </summary>
//        /// <param name="reportMessage"></param>
//        public static void Report(string reportMessage, TracerItem.PriorityEnum priority)
//        {
//            TracerHelper.Trace(TracerItem.TypeEnum.Report, reportMessage, priority);
//        }

//        /// <summary>
//        /// Report operation warning; it is a normal occurence in the work of the system. It can be caused
//        /// for example by the lack of access to a resource or some error in a data stream.
//        /// </summary>
//        /// <param name="warningMessage"></param>
//        public static void OperationWarning(string warningMessage)
//        {
//            OperationWarning(warningMessage, TracerItem.PriorityEnum.High);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public static void OperationWarning(string warningMessage, TracerItem.PriorityEnum priority)
//        {
//            TracerHelper.Trace(TracerItem.TypeEnum.Warning | TracerItem.TypeEnum.Operation , warningMessage, priority);

//            if (GlobalDiagnosticsMode && priority > TracerItem.PriorityEnum.Medium)
//            {
//                Debug.Fail(warningMessage);
//            }
//        }

        
//        /// <summary>
//        /// A Warning notifies that in some part of the systems operation a recovarable error has occured.
//        /// </summary>
//        /// <param name="warningMessage"></param>
//        public static void Warning(string warningMessage)
//        {
//            TracerHelper.TraceWarning(warningMessage);
            
//            if (GlobalDiagnosticsMode)
//            {
//                Debug.Fail(warningMessage);
//            }
//        }

//        /// <summary>
//        /// Helper, converts exception details to string message.
//        /// </summary>
//        public static string ProcessExceptionMessage(string errorDetails, Exception exception)
//        {
//            if (string.IsNullOrEmpty(errorDetails) == false)
//            {
//                return errorDetails + ", " + GeneralHelper.GetExceptionMessage(exception);
//            }

//            return GeneralHelper.GetExceptionMessage(exception);
//        }

//        /// <summary>
//        /// Report operation error from exception.
//        /// </summary>
//        /// <param name="ex"></param>
//        public static void OperationError(string errorDetails, Exception exception)
//        {
//            OperationError(ProcessExceptionMessage(errorDetails, exception));
//        }

//        /// <summary>
//        /// Report operation error; it is a lighter version of a error, and may be expected to 
//        /// occur during normal operation of the application (for. ex. a given non critical 
//        /// resource was not retrieved, operation has timed out etc.)
//        /// </summary>
//        /// <param name="errorMessage"></param>
//        public static void OperationError(string errorMessage)
//        {
//            OperationError(errorMessage, TracerItem.PriorityEnum.High);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public static void OperationError(string errorMessage, TracerItem.PriorityEnum priority)
//        {
//            TracerHelper.Trace(TracerItem.TypeEnum.Operation | TracerItem.TypeEnum.Error, errorMessage, priority);

//            if ((GlobalDiagnosticsMode && priority > TracerItem.PriorityEnum.Medium)
//                || priority == TracerItem.PriorityEnum.Critical)
//            {
//                Debug.Fail(errorMessage);
//            }
//        }

//        /// <summary>
//        /// Report an serious error. Those errors are usually a sign something in the work
//        /// of the application has gone seriously wrong, and operation can not continue
//        /// properly (for ex. unexpected exception, access to critical resources etc.)
//        /// </summary>
//        /// <param name="errorMessage"></param>
//        public static void Error(string errorMessage)
//        {
//            Error(errorMessage, TracerItem.PriorityEnum.High);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="errorMessage"></param>
//        public static void Error(string errorMessage, TracerItem.PriorityEnum priority)
//        {
//            TracerHelper.Trace(TracerHelper.Tracer, TracerItem.TypeEnum.Error, errorMessage, priority);
//            if (GlobalDiagnosticsMode)
//            {
//                Debug.Fail(errorMessage);
//            }
//        }

//        /// <summary>
//        /// Helper, redefine with exception consumption.
//        /// </summary>
//        /// <param name="errorMessage"></param>
//        /// <param name="exception"></param>
//        public static void Error(string errorMessage, Exception exception)
//        {
//            errorMessage = ProcessExceptionMessage(errorMessage, exception);

//            TracerHelper.TraceError(errorMessage);
//            if (GlobalDiagnosticsMode)
//            {
//                Debug.Fail(errorMessage);
//            }
//        }

            
//        /// <summary>
//        /// Chech condition, if false, throw an exception with parameters message.
//        /// </summary>
//        /// <param name="condition"></param>
//        /// <param name="parameters"></param>
//        public static void CheckThrow(bool condition, params string[] parameters)
//        {
//            if (condition == false)
//            {
//                Throw(parameters);
//            }
//        }

//        /// <summary>
//        /// Throw a general exception with parameters as message.
//        /// </summary>
//        /// <param name="parameters"></param>
//        public static void Throw(params string[] parameters)
//        {
//            throw new Exception(ParametersToString(parameters));
//        }
//    }
//}

//    }
//}
