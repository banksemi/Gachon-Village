using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NetworkLibrary.File;
namespace NetworkLibrary
{
    public class SocketEvent
    {
        public delegate void Connect(ESocket socket);
        public delegate void Receive(ESocket socket, JObject Message);
        public delegate void FileInfoReceive(ESocket socket, JObject Message, NetworkFile file);
        public delegate void Exit(ESocket socket);
    }
}
