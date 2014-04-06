using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XSockets.Client40;
using XSockets.Client40.Common.Event.Arguments;

namespace SocketUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if( args.Length == 0)
            {
                Console.WriteLine("specify config as argument.");
                Console.ReadKey();
                return;
            }

            UploaderConfig config = new UploaderConfig(args[0]);

            XSocketClient client = new XSocketClient(config.Url, "*");

            client.OnOpen += client_OnOpen;
            client.OnError += client_OnError;
            client.OnClose += client_OnClose;
            client.OnMessage += client_OnMessage;
            
            client.Open();

            //z nejakeho dovdotu nedostanu udalost OnOpen, ale nemohu hned posilat, pripojeni chviku trva
            Thread.Sleep(500);

            Notifier notifier = new Notifier(config, client);
            notifier.RunAsync();

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        static void client_OnMessage(object sender, TextArgs e)
        {
            throw new NotImplementedException();
        }

        static void client_OnClose(object sender, EventArgs e)
        {
            Trace.WriteLine("socket closed.");

        }

        static void client_OnOpen(object sender, EventArgs e)
        {
            Trace.WriteLine("socket opened.");           
        }

        static void client_OnError(object sender, TextArgs e)
        {
            Trace.WriteLine("socket error " + e.data);
        }
    }
}
