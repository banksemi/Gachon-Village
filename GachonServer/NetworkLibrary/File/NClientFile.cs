using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace NetworkLibrary.File
{
    public class NClientFile : NetworkFile
    {
        public Client server;

        public NClientFile(Client server, string Path) : base(Path)
        {
            this.server = server;
        }

        public NClientFile(Client server, JObject json) : base(json)
        {
            this.server = server;
        }
        protected override void AcceptFile(string path)
        {
            if (path != null)
            {
                if (upload) throw new Exception("업로드 모드일때는 경로를 수정할 수 없습니다.");
                else this.Path = path;
            }
            ConnectFileServer();
        }
        public void ConnectFileServer()
        {
            new Thread(
                delegate ()
                {
                    TcpClient client;
                    try
                    {
                        client = new TcpClient(server.ip, 8282);
                        NetworkStream ns = client.GetStream();
                        // 헤더 전송 (서버키)
                        byte[] bytes = BitConverter.GetBytes(ServerKey);
                        ns.Write(bytes, 0, 4);
                        ns.Flush();
                        StartEvent(client);
                    }
                    catch (Exception e)
                    {

                    }
                }
                ).Start();
        }
    }
}
