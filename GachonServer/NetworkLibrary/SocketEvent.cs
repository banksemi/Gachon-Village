using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary
{
    public class SocketEvent
    {
        public delegate void Connect(ESocket socket);
        public delegate void Receive(ESocket socket, string Message);
        public delegate void Exit(ESocket socket);
    }
}
