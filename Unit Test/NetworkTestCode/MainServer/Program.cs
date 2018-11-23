using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using NetworkLibrary.File;
using Newtonsoft.Json.Linq;
using NetworkLibrary.File;
namespace MainServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(10001);
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
            Console.WriteLine(Message.ToString());
            file.Accept("./U" + file.FileName);
            if (file.FileName == "3.png")
            {
                file.End += delegate (NetworkFile file5)
                {
                    Console.WriteLine("3번 파일 전송 끝");
                    NServerFile newfile = new NServerFile(socket, "./U7.png");
                    newfile.Process += delegate (NetworkFile file56)
                    {
                        Console.WriteLine("받은 바이트" + ((double)newfile.FinishByte / (double)newfile.FileSize));
                    };
                    socket.SendFile(Message, newfile);
                };
            }
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
