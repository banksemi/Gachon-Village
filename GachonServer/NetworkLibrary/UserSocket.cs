using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary
{
    class UserSocket : ESocket
    {
        public UserSocket(TcpClient client) : base(client)
        {
        }
    }
}
