using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
namespace NetworkLibrary
{
    public class Server
    {
        public event SocketEvent.Connect Connect;
        public event SocketEvent.Receive Receive;
        public event SocketEvent.Exit Exit;
        private TcpListener Listener;
        Thread Update_Thread = null;
        public int port { get; private set; }
        public Server(int port)
        {
            this.port = port;
            Listener = new TcpListener(port);
            Listener.Start();
            Update_Thread = new Thread(Update);
            Update_Thread.Start();
        }
        void Update()
        {
            while (true)
            {
                UserSocket client = null;
                try
                {
                    client = new UserSocket(Listener.AcceptTcpClient());
                    client.Connect += Client_Connect;
                    client.Receive += Client_Receive;
                    client.Exit += Client_Exit;
                    client.Start();
                }
                catch(Exception e)
                {

                }
            }
        }

        private void Client_Connect(ESocket socket)
        {
            if (Connect != null) Connect(socket);
        }

        private void Client_Exit(ESocket socket)
        {
            if (Exit != null) Exit(socket);
        }

        private void Client_Receive(ESocket socket, JObject Message)
        {
            if (Receive != null) Receive(socket, Message);
        }
    }
}
