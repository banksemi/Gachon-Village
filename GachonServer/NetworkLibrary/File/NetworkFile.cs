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
        /// <summary>
        /// 업로드 모드로 작성할 새로운 파일입니다.
        /// </summary>
        /// <param name="Path"></param>
        public NetworkFile(string Path)
        {
            this.Path = Path;
            this.upload = true;
            FileInfo info = new FileInfo(Path);
            FileSize = info.Length;
            FileName = info.Name;
            No = NewNumber;
            ServerKey = No;
            NetFiles.Add(No, this);
        }
        /// <summary>
        /// 다운로드 요청이 온 새로운 파일입니다.
        /// </summary>
        /// <param name="json"></param>
        public NetworkFile(JObject json)
        {
            this.upload = false;
            FileSize = (long)json["size"];
            FileName = (string)json["name"];
            if (this is NClientFile)
            {
                No = NewNumber;
                ServerKey = (int)json["no"];
                NetFiles.Add(No, this);
            }
            else
            {
                No = (int)json["no"];
                ServerKey = NewNumber;
                NetFiles.Add(ServerKey, this);
            }
        }
        private void StartUpload(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            // 파일보내기
            BinaryReader realFile = null;
            try
            {
                realFile = new BinaryReader(System.IO.File.Open(Path, FileMode.Open));
                int count = (int)(FileSize / 1024) + 1;
                for (int i = 0; i < count; i++)
                {
                    byte[] bs = realFile.ReadBytes(1024);
                    ns.Write(bs, 0, bs.Length);
                    ns.Flush();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (realFile != null) realFile.Close();
                ns.Close();
                socket.Close();
            }
        }
        private void StartDownload(TcpClient socket)
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

                filestream.Write(temp_byte, 0, r);
                remained -= r;
            }
            filestream.Flush();
            filestream.Close();
            socket.Close();
        }
        /// <summary>
        /// 서로간의 파일 전용 소켓이 만들어졌고, 헤더정보가 교환됬을때 실행되는 함수입니다. 라이브러리 내부에서만 사용됩니다.
        /// </summary>
        /// <param name="client"></param>
        public void StartEvent(TcpClient client)
        {
            try
            {
                Console.WriteLine("[NetworkFile : "+No+"] " + ServerKey + " : 정상 시작");
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
        protected abstract void AcceptFile(string path);
        /// <summary>
        /// 파일을 다운로드에 동의합니다.
        /// </summary>
        /// <param name="path"></param>
        public void Accept(string path)
        {
            if (upload == true) throw new Exception("업로드 주체는 상대방의 동의없이 전송을 시작할 수 없습니다.");
            AcceptFile(path);
        }
    }
}
