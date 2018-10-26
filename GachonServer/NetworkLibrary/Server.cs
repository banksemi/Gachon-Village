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
                ESocket client = null;
                try
                {
                    client = new ESocket(Listener.AcceptTcpClient());
                    client.Start();
                }
                catch(Exception e)
                {

                }
            }
        }
    }
}
