using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using NetworkLibrary.File;
namespace NetworkLibrary
{
    public class Server
    {
        public event SocketEvent.Connect Connect;
        public event SocketEvent.Receive Receive;
        public event SocketEvent.Exit Exit;
        public event SocketEvent.FileInfoReceive FileInfoReceive;
        private TcpListener Listener;
        private TcpListener FileListener;
        Thread Update_Thread = null;
        Thread FileUpdate_Thread = null;
        public int port { get; private set; }
        public Server(int port)
        {
            this.port = port;
            Listener = new TcpListener(port);
            Listener.Start();
            FileListener = new TcpListener(8282);
            FileListener.Start();
            Update_Thread = new Thread(Update);
            Update_Thread.Start();
            FileUpdate_Thread = new Thread(FileSocketUpdate);
            FileUpdate_Thread.Start();
        }
        void Update()
        {
            while (true)
            {
                UserSocket client = null;
                try
                {
                    client = new UserSocket(Listener.AcceptTcpClient());
                    client.Connect += Client_Connect;
                    client.Receive += Client_Receive;
                    client.FileInfoReceive += FileInfoReceive;
                    client.Exit += Client_Exit;
                    client.Start();
                }
                catch (Exception e)
                {

                }
            }
        }
        void FileSocketUpdate()
        {
            while (true)
            {
                TcpClient client = null;
                try
                {
                    
                    client = FileListener.AcceptTcpClient();
                    new Thread(new ThreadStart(
                        delegate
                        {
                            Console.WriteLine("누군가 파일서버에 커넥션을 요청 :)");
                            NetworkStream ns = client.GetStream();
                            if (ns.ReadByte() == 1)
                            {
                                byte[] aa = new byte[4];
                                ns.Read(aa, 0, 4);
                                int Serverkey = BitConverter.ToInt32(aa, 0);
                                Console.WriteLine("이사람의 서버키는 " + Serverkey + "이었음.");
                                NetworkFile file = NetworkFile.NetFiles[Serverkey];
                                if (file.is_started)
                                {
                                    client.Close();
                                }
                                Console.WriteLine("나는 " + file.FileSize+
                                    "바이트의 " +
                                    file.FileName +
                                    "파일을 " +
                                    "받을 준비가 되었다.");

                            }
                        }
                        )
                        ).Start();
                }
                catch (Exception e)
                {

                }
            }
        }

        private void Client_Connect(ESocket socket)
        {
            if (Connect != null) Connect(socket);
        }

        private void Client_Exit(ESocket socket)
        {
            if (Exit != null) Exit(socket);
        }

        private void Client_Receive(ESocket socket, JObject Message)
        {
            if (Receive != null) Receive(socket, Message);
        }
        private void Client_FileInfoReceive(ESocket socket, JObject Message, NetworkFile file)
        {
            if (FileInfoReceive != null) FileInfoReceive(socket, Message, file);
        }
    }
}
