using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NetworkLibrary;
using NetworkLibrary.File;
namespace MainClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client server = new Client("127.0.0.1",10000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            server.Start();


            NetworkFile file = new NetworkFile("./123.png", true);
            JObject json = new JObject();
            json["himessage"] = "ㅋㅋ";
            file.Start += delegate (NetworkFile networkFile)
            {
                Console.WriteLine("난 시작했단걸 알아챘어!");
            };
            file.Process += delegate (NetworkFile networkFile)
            {
                Console.WriteLine(networkFile.FileName + "을 보내는 중이야");
            };
            file.End += delegate (NetworkFile networkFile)
            {
                Console.WriteLine(networkFile.FileName + "을 보내는 중이야");
            };

            Console.WriteLine("서버로 요청을 해볼게!");
            server.SendFile(json, file);

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Server_Receive(ESocket socket, JObject Message)
        {
            Console.WriteLine(socket.GetHashCode() + " : " + Message);
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
