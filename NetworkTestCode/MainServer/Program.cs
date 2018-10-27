using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
namespace MainServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(10000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Server_Receive(ESocket socket, string Message)
        {
            Console.WriteLine(socket.GetHashCode() + " : " + Message);
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
