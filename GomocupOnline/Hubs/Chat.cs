using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.IO;
using System.Threading.Tasks;
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
            string messageLine = string.Format("«{0}»{1}\r\n", name, message);

            lock (_locker)
            {
                File.AppendAllText(_chatLog, messageLine);
            }

            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        public override Task OnConnected()
        {        
            Task task = base.OnConnected();

            System.Threading.Thread.Sleep(500);

            string history = string.Empty;
            lock (_locker)
            {
                if (File.Exists(_chatLog))
                    history = File.ReadAllText(_chatLog);
            }
            if (history.Length > 0)
            {
                Message[] messages = Parse(history);
                foreach (var item in messages)
                {
                    Clients.Client(Context.ConnectionId).addNewMessageToPage(item.Name, item.Text);                    
                }
            }
            return task;
        }

        Message[] Parse(string text)
        {
            string[] messages = text.Split(new char[]{'«'}, StringSplitOptions.RemoveEmptyEntries);

            Message[] ret = new Message[messages.Length];

            for (int i = 0; i < messages.Length; i++)
            {
                string[] messageParts = messages[i].Split(new char[] { '»' }, StringSplitOptions.None);

                ret[i] = new Message()
                {
                    Name = messageParts[0],
                    Text = messageParts[1].Trim(),
                };
            }
            return ret;
        }

    }

    public class Message
    {
        public string Name;
        public string Text;
    }
}