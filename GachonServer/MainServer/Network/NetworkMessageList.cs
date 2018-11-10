using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    class NetworkMessageList
    {
        public static void TipMessage(ESocket socket, string message)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.TipMessage;
            json["message"] = message;
            socket.Send(json);
        }
    }
}
