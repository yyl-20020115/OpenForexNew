using System;
using System.IO;
using System.Windows.Forms;
using CommonSupport;

namespace AutonomousTracer
{
    public partial class FormMain : Form
    {
        string _filePath = null;

        FileSystemWatcher _watcher = new FileSystemWatcher();

        long _lastFilePos = 0;

        FileStream _fs = null;

        Tracer _tracer = new Tracer();

        StreamReader _reader = null;

        /// <summary>
        /// 
        /// </summary>
        public FormMain()
        {
            InitializeComponent();

            TracerItemKeeperSink sink = new TracerItemKeeperSink(_tracer);
            sink.MaxItems = 1000000;
            _tracer.Add(sink);

            _watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            tracerControl1.Tracer = _tracer;
        }


        /// <summary>
        /// 
        /// </summary>
        private void toolStripButtonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Assemblies (*.log; *.txt)|*.log;*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _tracer.Clear(false);

                _filePath = ofd.FileName;
                _watcher.Path = Path.GetDirectoryName(_filePath);
                _watcher.Filter = Path.GetFileName(_filePath);

                _watcher.EnableRaisingEvents = true;


                if (_fs != null)
                {
                    _fs.Dispose();
                }

                _lastFilePos = 0;

                toolStripLabelFile.Text = _filePath;

                toolStripButtonUpdate_Click(this, EventArgs.Empty);
            }
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_fs == null
                || e.ChangeType == WatcherChangeTypes.Deleted
                || e.ChangeType == WatcherChangeTypes.Created)
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }

                if (_fs != null)
                {
                    _fs.Dispose();
                    _fs = null;
                }

                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    return;
                }
                
                try
                {
                    _fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    _reader = new StreamReader(_fs);
                }
                catch (Exception ex)
                {
                    SystemMonitor.Error(ex.Message);
                    _reader = null;
                    _fs.Dispose();
                    _fs = null;
                    return;
                }
            }

            if (_fs == null || _fs.CanRead == false)
            {
                return;
            }

            if (_fs.Length < _lastFilePos - 10)
            {// File was rewritten start from beggining.
                _lastFilePos = 0;
            }

            _fs.Seek(_lastFilePos, SeekOrigin.Begin);

            string line = _reader.ReadLine();
            while (line != null)
            {
                TracerItem item = TracerItem.ParseFileItem(line);
                if (item != null)
                {
                    _tracer.Add(item);
                }
                //ParseLine(line);
                line = _reader.ReadLine();
            }

            _lastFilePos = _fs.Position;
        }


        //void ParseLine(string line)
        //{
            //if (string.IsNullOrEmpty(line))
            //{
            //    return;
            //}

            //try
            //{
            //    string[] substrings = line.Split('|');

            //    if (substrings.Length < 4)
            //    {
            //        SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", Not enough substrings generated].");
            //        return;
            //    }

            //    TracerItem.TypeEnum type = (TracerItem.TypeEnum)Enum.Parse(typeof(TracerItem.TypeEnum), substrings[0]);

            //    long index = 0;
            //    long.TryParse(substrings[1], out index);

            //    DateTime time;
            //    try
            //    {
            //        string dateTime = substrings[2];
            //        string[] dateTimeParts = dateTime.Split('/');
            //        string[] subParts = dateTimeParts[2].Split(' ');
            //        TimeSpan timeSpan = TimeSpan.Parse(subParts[1]);

            //        time = new DateTime(int.Parse(subParts[0]), int.Parse(dateTimeParts[0]),
            //            int.Parse(dateTimeParts[1]), timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            //    }
            //    catch(Exception ex2)
            //    {
            //        SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", " + ex2.Message + "].");
            //        time = DateTime.MinValue;
            //    }

            //    //if (DateTime.TryParse(substrings[2], out time) == false)
            //    //{
            //    //    time = DateTime.MinValue;
            //    //}

            //    TracerItem item = new TracerItem(type, time, index.ToString() + "  " + substrings[substrings.Length - 1]);
            //    _tracer.Add(item);

            //}
            //catch(Exception ex)
            //{
            //    SystemMonitor.OperationError("Failed to parse tracer item line [" + line + ", " + ex.Message + "].");
            //}
        //}

        private void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            GeneralHelper.FireAndForget(delegate()
            {
                _watcher_Changed(null, new FileSystemEventArgs(WatcherChangeTypes.Created, _watcher.Path, _watcher.Filter));
                WinFormsHelper.BeginFilteredManagedInvoke(tracerControl1, tracerControl1.UpdateUI);
            });
        }


    }
}
