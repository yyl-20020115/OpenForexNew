using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using CommonSupport;

namespace Arbiter
{
    /// <summary>
    /// 
    /// </summary>
    public class ArbiterXmlLoggingClient : ArbiterClientBase
    {
        [NonSerialized]
        FileStream _fileStream;

        bool _outputToDebugConsole = false;
        public bool OutputToDebugConsole
        {
            get { return _outputToDebugConsole; }
            set { _outputToDebugConsole = value; }
        }

        string _fileName;
        
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ArbiterXmlLoggingClient(string fileName)
            : base("ArbiterXmlLoggingClient[" + fileName + "]", true)
        {
            _fileName = fileName;
            Initialize();
        }

        private void Initialize()
        {
            _messageFilter.Enabled = false;

            try
            {
                _fileStream = new FileStream(_fileName, FileMode.Append, FileAccess.Write);
                using (XmlTextWriter writer = new XmlTextWriter(_fileStream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    writer.WriteStartElement("Root");
                    writer.WriteEndElement();
                }
                // The writer closed the stream anyhow, so we need to reopen it.
                _fileStream.Close();

                _fileStream = new FileStream(_fileName, FileMode.Append, FileAccess.Write);

                ////If WriteProcessingInstruction is used as above,
                ////Do not use WriteEndElement() here
                ////xmlWriter.WriteEndElement();
                ////it will cause the &ltRoot></Root> to be &ltRoot />
                //_xmlWriter.Flush();
            }
            catch (Exception ex)
            {
                CommonSupport.SystemMonitor.Error("Failed to create logging file [" + ex.ToString() + "].");
            }
        }

        /// <summary>
        /// Custom XML logging.
        /// </summary>
        /// <param name="entity"></param>
        //public override void ReceiveExecution(ExecutionEntity entity)
        //{
        //    if (_xmlDoc == null || _xmlWriter == null)
        //    {
        //        return;
        //    }

        //    TraceHelper.Trace("Arbiter::ReceiveExecution");

        //    try
        //    {
        //        XmlText messageText = _xmlDoc.CreateTextNode("MessageNode");
        //        messageText.Value = entity.Message.ToString();

        //        XmlElement messageNode = _xmlDoc.CreateElement("Message");
        //        messageNode.AppendChild(messageText);
        //        messageNode.SetAttribute("Time", System.DateTime.Now.ToString());

        //        messageNode.WriteTo(_xmlWriter);
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemMonitor.Error("Error occured while serializing logger requestMessage [" + ex.ToString() + "].");
        //    }

        //    _xmlWriter.Flush();
        //}


        public override void ReceiveExecution(ExecutionEntity entity)
        {
            ReceiveDirectCall(entity.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public override void ReceiveExecutionWithReply(ExecutionEntityWithReply entity)
        {// Logging does not need to be talked to directly.
            throw new Exception("UnImplemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversation"></param>
        public override void ReceiveConversationTimedOut(Conversation conversation)
        {// Logging does not send requests, to receive time outs.
            throw new Exception("UnImplemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Message ReceiveDirectCall(Message message)
        {
            try
            {
                //Type type = entity.Message.GetType();
                //object t = Activator.CreateInstance(type);

                XmlSerializer s = new XmlSerializer(message.GetType());

                s.Serialize(_fileStream, message);
            }
            catch (Exception e)
            {
                SystemMonitor.Error("Serialization failure [" + e.Message + "]");
            }

            return null;
        }

    }
}
