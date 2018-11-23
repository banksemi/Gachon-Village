using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Client : ESocket
    {
        public string ip { get; private set; }
        public int port { get; private set; }
        public Client(string ip, int port) : base(new TcpClient(ip,port))
        {
            this.ip = ip;
            this.port = port;
        }
    }
}
