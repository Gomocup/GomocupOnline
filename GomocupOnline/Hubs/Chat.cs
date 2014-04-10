using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.IO;
namespace SignalRChat
{
    public class ChatHub : Hub
    {
        static string _chatLog;
        static object _locker = new object();

        static ChatHub()
        {
            string path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            _chatLog = Path.Combine(path, "chat.txt");
        }

        public void Send(string name, string message)
        {
            string messageLine = string.Format("«{0}»{1}", name, message);

            lock (_locker)
            {
                File.AppendAllText(_chatLog, messageLine);
            }

            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}