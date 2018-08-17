using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CommonSupport
{
    /// <summary>
    /// Class allows to trace trace item information to a text file.
    /// </summary>
    public class FileTracerItemSink : TracerItemSink
    {
        FileWriterHelper _fileWriter = new FileWriterHelper();

        /// <summary>
        /// Full path to the file that is to receive the trace information.
        /// </summary>
        public string FilePath
        {
            get { return _fileWriter.InitialFilePath; }
            set { Initialize(value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileTracerItemSink(Tracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Detailed constructor.
        /// </summary>
        public FileTracerItemSink(Tracer tracer, string filePath)
            : base(tracer)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Perform the sink initialization, open file etc.
        /// </summary>
        protected void Initialize(string filePath)
        {
            _fileWriter.Initialize(filePath);
        }

        /// <summary>
        /// Free any resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Received an item, sink it to file.
        /// </summary>
        protected override bool OnReceiveItem(TracerItem item, bool isFilteredOutByTracer, bool isFilteredOutBySink)
        {
            return _fileWriter.WriteLine(item.PrintFileLine());
        }
    }
}
