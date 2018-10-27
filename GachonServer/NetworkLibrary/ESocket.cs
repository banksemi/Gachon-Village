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
    public class ESocket
    {
        public event SocketEvent.Connect Connect;
        public event SocketEvent.Receive Receive;
        public event SocketEvent.Exit Exit;
        TcpClient tcpClient = null;
        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        Thread thread;
        public ESocket(TcpClient client)
        {
            tcpClient = client;
        }
        public ESocket (string ip, int port)
        {
            tcpClient = new TcpClient(ip, port);
        }
        public void Start()
        {
            thread = new Thread(delegate () {

                NS = tcpClient.GetStream();
                SR = new StreamReader(NS, Encoding.UTF8); // Get message
                SW = new StreamWriter(NS, Encoding.UTF8); // Send message
                if (Connect != null) Connect(this);
                Update();
            });
            thread.Start();
        }
        public void Update()
        {
            while(true)
            {
                String message = null;
                message = SR.ReadLine();
                if (message == null) break;
                if (Receive != null) Receive(this, message);
            }
            Dispose();
        }
        public void Send(string message)
        {
            SW.WriteLine(message);
        }
        public void Dispose()
        {
            if (Exit != null) Exit(this);
            try { if (SR != null) SR.Dispose(); } catch { }
            try { if (SW != null) SW.Dispose(); } catch { }
            try { if (NS != null) NS.Dispose(); } catch { }
            try { if (tcpClient != null) tcpClient.Dispose(); } catch { }
            try { if (thread != null) thread.Abort(); } catch { }
        }
    }
}
