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
    public abstract class NetworkFile
    {
        public bool is_started = false;
        private static int _SecretNumber = 0;
        public static Dictionary<int, NetworkFile> NetFiles = new Dictionary<int, NetworkFile>();
        private static int NewNumber
        {
            get
            {
                return _SecretNumber++;
            }
        }
        public int No { get; private set; }
        public int ServerKey = -1;
        public string Path { get; protected set; }
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
        public delegate void FileAction(NetworkFile file);
        public event FileAction Start;
        public event FileAction Process;
        public event FileAction End;
        public event FileAction Error;
        public void ErrorEvent()
        {
            Error?.Invoke(this);
        }
        public void EndEvent()
        {
            End?.Invoke(this);
        }
        public void ProcessEvent()
        {
            Process?.Invoke(this);
        }
        public NetworkFile(string Path)
        {
            this.Path = Path;
            this.upload = true;
            FileInfo info = new FileInfo(Path);
            FileSize = info.Length;
            FileName = info.Name;
            No = NewNumber;
            NetFiles.Add(No, this);
        }
        public NetworkFile(JObject json)
        {
            this.upload = false;
            FileSize = (long)json["size"];
            FileName = (string)json["name"];
            No = (int)json["no"];
            ServerKey  = 10000 + NewNumber;

            NetFiles.Add(ServerKey, this);
        }
        public void StartUpload(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            // 파일보내기
            BinaryReader realFile = new BinaryReader(System.IO.File.Open(Path, FileMode.Open));
            int count = (int)(FileSize / 1024) + 1;
            for (int i = 0; i < count; i++)
            {
                byte[] bs = realFile.ReadBytes(1024);
                ns.Write(bs, 0, bs.Length);
                ns.Flush();
            }
            ns.Close();
            socket.Close();
        }
        public void StartDownload(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            FileStream filestream = new FileStream(Path, FileMode.Create);
            long remained = FileSize;
            while (remained > 0)
            {
                int temp = 1024;
                if (remained < 1024) temp = (int)remained;
                byte[] temp_byte = new byte[1024];
                int r = ns.Read(temp_byte, 0, temp);

                filestream.Write(temp_byte, 0, temp);
                remained -= temp;
            }
            filestream.Flush();
            filestream.Close();
            socket.Close();
        }
        public void StartEvent(TcpClient client)
        {
            try
            {
                Console.WriteLine("[NetworkFile] " + ServerKey + " : 정상 시작");
                Start?.Invoke(this);
                if (upload == true)
                {
                    StartUpload(client);
                }
                else
                {
                    StartDownload(client);
                }
                Console.WriteLine("[NetworkFile] " + ServerKey + " : 완료");
                End?.Invoke(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("[NetworkFile] " + ServerKey + " : " + e.Message);
                Error?.Invoke(this);
            }
        }
    }
}
