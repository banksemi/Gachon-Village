using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using Newtonsoft.Json.Linq;

namespace Server
{
    class User
    {
        public string name;
    }
    class Program
    {
        static Dictionary<ESocket, User> list = new Dictionary<ESocket, User>();
        static void Main(string[] args)
        {
            NetworkLibrary.Server server = new NetworkLibrary.Server(1000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Server_Connect(ESocket socket)
        {
            list.Add(socket, new User());
            list[socket].name = socket.GetHashCode().ToString();
        }
        private static void Server_Receive(ESocket socket, JObject Message)
        {
            if ((int)Message["type"] == 0)
            {
                JObject json = new JObject();
                json["type"] = 0;
                json["message"] = list[socket].name + " : " + Message["message"];
                foreach (ESocket item in list.Keys)
                {
                    item.Send(json);
                }
                Console.WriteLine(json["message"]);
            }
        }
        private static void Server_Exit(ESocket socket)
        {
            list.Remove(socket);
        }
    }
}
