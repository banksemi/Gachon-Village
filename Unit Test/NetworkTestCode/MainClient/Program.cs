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
            Client server = new Client("127.0.0.1",10001);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            server.FileInfoReceive += Server_FileInfoReceive;
            server.Start();


            NetworkFile file = new NClientFile(server, "./3.png");
            file.Process += delegate (NetworkFile file56)
            {
                Console.WriteLine("보낸 바이트" + ((double)file56.FinishByte / (double)file56.FileSize));
            };
            JObject json = new JObject();
            json["himessage"] = "ㅋㅋ";

            Console.WriteLine("서버로 요청을 해볼게!");
            server.SendFile(json, file);

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Server_FileInfoReceive(ESocket socket, JObject Message, NetworkFile file)
        {
            file.Process += delegate (NetworkFile file56)
            {
                Console.WriteLine("받은 바이트" + ((double)file56.FinishByte / (double)file56.FileSize));
            };
            file.Accept("./다운.png");
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
