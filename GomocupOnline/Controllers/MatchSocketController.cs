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
        static string _torunamentPath;

        //static HashSet<string> _watcherDelay = new HashSet<string>();

        static MatchSocketController()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            _torunamentPath = Path.Combine(path, "Tournaments");

            _watcher = new FileSystemWatcher(_torunamentPath);
            _watcher.Changed += _watcher_Changed;
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;
        }

        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath.ToLower();


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


            if (path.EndsWith(".psq"))
            {
                GomokuMatchModel model = new GomokuMatchModel(path);

                string tournamentMatch = path.Substring(_torunamentPath.Length);
                model.FileName = tournamentMatch.Replace("\\", "\\\"");

                JavaScriptSerializer js = new JavaScriptSerializer();
                string jsonMatch = js.Serialize(model);

                var receivers = _receivers.ToArray();
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMatch));

                foreach (AspNetWebSocketContext listener in receivers)
                {
                    try
                    {
                        listener.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        _receivers.Remove(listener);
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
            string jsontest = js.Serialize(model);

            return jsontest;
        }

        private async Task MatchNotify(AspNetWebSocketContext context)
        {
            var filename = context.QueryString["match"];
            Trace.WriteLine(filename);

            WebSocket socket = context.WebSocket;

            _receivers.Add(context);

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
    }
}