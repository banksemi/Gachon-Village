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
        TcpClient tcpClient = null;
        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        public ESocket(TcpClient client)
        {
            tcpClient = client;
            NS = client.GetStream();
            SR = new StreamReader(NS, Encoding.UTF8); // Get message
            SW = new StreamWriter(NS, Encoding.UTF8); // Send message
        }
        public void Start()
        {

        }
        public void Send(string message)
        {

        }
        public void Dispose()
        {
            try { if (SR != null) SR.Dispose(); } catch { }
            try { if (SW != null) SW.Dispose(); } catch { }
            try { if (NS != null) NS.Dispose(); } catch { }
            try { if (tcpClient != null) tcpClient.Dispose(); } catch { }
        }
    }
}
