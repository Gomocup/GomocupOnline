using GomocupOnline.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.WebSockets;

namespace GomocupOnline.Controllers
{
    public class MatchSocketController : ApiController
    {
        static List<AspNetWebSocketContext> _receivers = new List<AspNetWebSocketContext>();

        static FileSystemWatcher _watcher;
        static string _tournamentPath;
        static string _tournamentOnlinePath;
        static string[] _ext = new string[] {".psq", ".html", ".txt" };

        //static HashSet<string> _watcherDelay = new HashSet<string>();

        static MatchSocketController()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            _tournamentPath = Path.Combine(path, "Tournaments").ToLower();
            _tournamentOnlinePath = Path.Combine(_tournamentPath, "online");

            _watcher = new FileSystemWatcher(_tournamentPath);
            _watcher.Changed += _watcher_Changed;
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;
        }

        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath.ToLower();

            Thread.Sleep(100);

            ////multiple events filter
            //if (_watcherDelay.Contains(path))
            //{
            //    Trace.WriteLine(DateTime.Now.ToString() + " event discarded " + path);
            //    return;
            //}
            //lock (_watcherDelay)
            //{
            //    _watcherDelay.Add(path);
            //}
            //Thread.Sleep(5000);
            //lock (_watcherDelay)
            //{
            //    _watcherDelay.Remove(path);
            //}
            //Trace.WriteLine(DateTime.Now.ToString() + " event passed " + path);


            if (_ext.Any(extension => path.EndsWith(extension)))
            {
                if (path.StartsWith(_tournamentOnlinePath))
                {
                    try
                    {
                        SendToAll(path);
                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }
        }    

        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(MatchNotify);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.SwitchingProtocols);
        }

        string JsonGetMatch(string tournamentMatch)
        {
            if (tournamentMatch == null || tournamentMatch.Contains(".."))
                throw new ArgumentException();

            string path = TournamentController.GetMatchPath(tournamentMatch);
            GomokuMatchModel model = new GomokuMatchModel(path);
            model.FileName = tournamentMatch.Replace("\\", "\\\"");

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(model);            
        }

        static ResultTable JsonResultTableModel(string path)
        {
            ResultTable model = new ResultTable();

            string dir = Path.GetDirectoryName(path);            
            model.FileName = Path.GetFileName(dir);

            string html = File.ReadAllText(path);
            int start = html.IndexOf("<TABLE");
            int end = html.IndexOf("</BODY>");
            model.Table = html.Substring(start, end - start);

            return model;        
        }

        private async Task MatchNotify(AspNetWebSocketContext context)
        {
            var filename = context.QueryString["match"];
            Trace.WriteLine(filename);

            WebSocket socket = context.WebSocket;
            _receivers.Add(context);

            SendAllData(context);

            //return;
            while (true) //tohle je dulezite, aby se Socket nedisposoval
            {               

                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    //userMessage = "You sent: " + userMessage + " at " + DateTime.Now.ToLongTimeString();

                    string jsonMatch = JsonGetMatch(filename);

                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMatch));

                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }


        private static void SendToAll(string path)
        {
            AspNetWebSocketContext[] receivers = _receivers.ToArray();
            
            ArraySegment<byte>? buffer = CreateObjectBuffer(path);
            if (buffer == null)
                return;

            foreach (AspNetWebSocketContext listener in receivers)
            {
                try
                {
                    listener.WebSocket.SendAsync(buffer.Value, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch
                {
                    _receivers.Remove(listener);
                }
            }
        }

        private void SendAllData(AspNetWebSocketContext socketcontext)
        {
            //string[] allPsq = Directory.GetFiles(_tournamentOnlinePath, "*.psq", SearchOption.AllDirectories);

            var files = Directory.EnumerateFiles(_tournamentOnlinePath, "*.*", SearchOption.AllDirectories)
            .Where(s => _ext.Any(extension => s.ToLower().EndsWith(extension))).ToArray();


            foreach (var file in files)
            {
                try
                {
                    ArraySegment<byte>? buffer = CreateObjectBuffer(file);
                    if (buffer == null)
                        continue;

                    try
                    {
                        socketcontext.WebSocket.SendAsync(buffer.Value, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        _receivers.Remove(socketcontext);
                        return;
                    }

                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex);
                    continue;
                }              
            }
        }

        private static ArraySegment<byte>? CreateObjectBuffer(string path)
        {
            object jsonModel = null;
            if (path.ToLower().EndsWith(".psq"))
            {

                GomokuMatchModel model = new GomokuMatchModel(path);
                string tournamentMatch = path.Substring(_tournamentPath.Length);
                model.FileName = tournamentMatch.Replace("\\", "_");

                jsonModel = model;
            }
            else if (path.ToLower().EndsWith(".html"))
            {
                jsonModel = JsonResultTableModel(path);
            }
            else
            {
                return null;
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(jsonModel);

            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        }

       
    }
}