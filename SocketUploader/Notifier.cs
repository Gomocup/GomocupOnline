using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using XSockets.Client40;
using XSockets.Client40.Common.Event.Interface;

namespace SocketUploader
{
    public class Notifier
    {
        UploaderConfig _config;
        XSocketClient _client;

        List<FileSystemWatcher> _watcher = new List<FileSystemWatcher>();

        public Notifier(UploaderConfig config, XSocketClient client)
        {
            _config = config;
            _client = client;            
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(50);

            string source = e.FullPath.ToLower();
            string target = _config.Items
                .Where(item => item.SourceFileName == source)
                .Select(item => item.TargetFileName)
                .First();

            try
            {
                BinaryData data = null;

                //mutex, ktery pouziva piskvorkna ukladani psq
                using (Mutex mutex = new Mutex(false, @"Global\" + source.ToLower().Replace('\\',':')))
                {
                    if (mutex.WaitOne(500, false))
                    {
                        data = new BinaryData(source);
                    }                   
                    else
                    {
                        Trace.WriteLine("Mutex timeout to  " + source);
                        return;
                    }
                }                

                Trace.WriteLine("Sending " + source + " as " + target);
                _client.Send(target);

                _client.Send(data);
            }
            catch(IOException ex)
            {
                Trace.WriteLine(DateTime.Now + " reading file failed");

                Trace.WriteLine(ex.Message);
            }      
            catch(Exception)
            {
                Trace.WriteLine(DateTime.Now + " sending file failed");

                try
                {
                    _client = new XSocketClient(_config.Url, "*");
                    Thread.Sleep(1000);
                    _client.Open();                    
                }
                catch
                {
                    Trace.WriteLine("reconnect failed");
                    return;
                }
            }
        }

        public void RunAsync()
        {
            foreach (UploaderItem item in _config.Items)
            {
                Trace.WriteLine("Listening " + item.SourceFileName + " as " + item.TargetFileName);

                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = Path.GetDirectoryName(item.SourceFileName);
                watcher.Filter = Path.GetFileName(item.SourceFileName);                
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += watcher_Changed;
                watcher.EnableRaisingEvents = true;
            }
        }
    }

    class BinaryData : IBinaryArgs
    {
        IList<byte> _data = new List<byte>();
        string _path;

        public BinaryData(string path)
        {
            _path = path;
            _data = File.ReadAllBytes(path).ToList();
        }

        public IList<byte> data
        {
            get { return _data; }
            set { _data = value; }
        }

        public string @event
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
