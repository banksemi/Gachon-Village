using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
using NetworkLibrary.File;
namespace MainServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(10000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.FileInfoReceive += Server_FileInfoReceive;
            server.Exit += Server_Exit;
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Server_FileInfoReceive(ESocket socket, JObject Message, NetworkFile file)
        {
            file.StartFileTransmission(socket);
            Console.WriteLine(Message.ToString());
        }

        private static void Server_Receive(ESocket socket, JObject Message)
        {
            Console.WriteLine(socket.GetHashCode() + " : " + Message["test"]);
            // return
            socket.Send(Message);
        }

        private static void Server_Exit(ESocket socket)
        {
            Console.WriteLine(socket.GetHashCode() + " exit");
        }

        private static void Server_Connect(ESocket socket)
        {
            Console.WriteLine(socket.GetHashCode() + " connected");
        }
    }
}
