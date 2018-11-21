using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace NetworkLibrary.File
{
    public class NetworkFile
    {
        public bool is_started = false;
        private static int _SecretNumber = 0;
        public static Dictionary<int, NetworkFile> NetFiles = new Dictionary<int, NetworkFile>();
        private static int SecretNumber
        {
            get
            {
                return _SecretNumber++;
            }
        }
        public int No { get; private set; }
        public int ServerKey = -1;
        public string Path { get; private set; }
        public long FileSize { get; private set; }
        public string FileName { get; private set; }
        public bool upload { get; private set; }
        public JObject Information
        {
            get
            {
                JObject jObject = new JObject();
                jObject["size"] = FileSize;
                jObject["name"] = FileName;
                jObject["no"] = No;
                return jObject;
            }
        }

        public event FileAction Start;
        public delegate void FileAction(NetworkFile file);
        public event FileAction Process;
        public event FileAction End;
        public event FileAction Error;
        public NetworkFile(string Path, bool upload)
        {
            this.Path = Path;
            this.upload = upload;
            FileInfo info = new FileInfo(Path);
            FileSize = info.Length;
            FileName = info.Name;
            No = SecretNumber;
            NetFiles.Add(No, this);
        }
        public NetworkFile(JObject json)
        {
            this.upload = false;
            FileSize = (long)json["size"];
            FileName = (string)json["name"];
            No = (int)json["no"];
            ServerKey  = 10000 + SecretNumber;

            NetFiles.Add(ServerKey, this);
        }
        public void StartFileTransmission(ESocket socket)
        {
            JObject jObject = new JObject();
            jObject["type"] = -2;
            jObject["no"] = No;
            jObject["serverkey"] = ServerKey;
            socket.Send(jObject);
        }
        public void StartInSender(int serverkey)
        {
            this.ServerKey = serverkey;
            Console.WriteLine("서버키 : " + ServerKey);
            TcpClient client = new TcpClient("127.0.0.1", 8282);
            NetworkStream ns= client.GetStream();
            // 앞에 1바이트는 업로드 표시, 뒤에 4바이트는 서버키

            byte[] bytes = BitConverter.GetBytes(serverkey);
            ns.WriteByte(1);
            ns.Write(bytes, 0, 4);
            ns.Flush();
            Start?.Invoke(this);

        }
    }
}
