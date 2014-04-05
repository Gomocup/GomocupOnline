using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace GomocupOnline.Controllers
{
    public class MatchSocketController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(MatchNotify);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.SwitchingProtocols);
        }


        private async Task MatchNotify(AspNetWebSocketContext context)
        {
            var filename = context.QueryString["match"];
            Trace.WriteLine(filename);

            WebSocket socket = context.WebSocket;
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (socket.State == WebSocketState.Open)
                {
                    string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    userMessage = "You sent: " + userMessage + " at " + DateTime.Now.ToLongTimeString();

                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));

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