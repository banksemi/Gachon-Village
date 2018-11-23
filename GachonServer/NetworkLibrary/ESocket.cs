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
    public class ESocket
    {
        public event SocketEvent.Connect Connect;
        public event SocketEvent.Receive Receive;
        public event SocketEvent.FileInfoReceive FileInfoReceive;
        public event SocketEvent.Exit Exit;
        public bool isClosed { get; private set; }
        TcpClient tcpClient = null;
        NetworkStream NS = null;
        StreamReader SR = null;
        StreamWriter SW = null;
        Thread thread;
        public ESocket(TcpClient client)
        {
            tcpClient = client;
        }
        public ESocket (string ip, int port)
        {
            tcpClient = new TcpClient(ip, port);
        }
        public void Start()
        {
            thread = new Thread(delegate () {

                NS = tcpClient.GetStream();
                SR = new StreamReader(NS, Encoding.UTF8); // Get message
                SW = new StreamWriter(NS, Encoding.UTF8); // Send message
                if (Connect != null) Connect(this);
                Update();
            });
            thread.Start();
        }
        public void Update()
        {
            while(true)
            {
                String message = null;
                try
                {
                    message = SR.ReadLine();
                    if (message == null) break;
                    JObject json = JObject.Parse(message);

                    if (Receive != null)
                    {
                        if ((int)json["type"] == -1) // 처음에 물어보는 과정 (클라 -> 서버 업로드 요청, 서버 -> 클라 다운로드 명령)
                        {
                            NetworkFile file = null;
                            if (this is Client) // 클라이언트단에서 처리하는 메세지
                            {
                                file = new NClientFile((Client)this, (JObject)json["file"]);
                            }
                            else if (this is UserSocket) // 서버단에서 처리하는 메세지
                            {
                                file = new NServerFile(this, (JObject)json["file"]);
                            }
                            FileInfoReceive(this, (JObject)json["message"], file);
                        }
                        else if((int)json["type"]==-2)
                        {
                            
                            if (this is Client) // 클라이언트단에서 처리하는 메세지
                            {
                                Console.WriteLine((int)json["no"]);
                                NClientFile file = (NClientFile)NetworkFile.NetFiles[(int)json["no"]];
                                Console.WriteLine(json.ToString());
                                file.ServerKey = (int)json["serverkey"];
                                Console.WriteLine((int)file.ServerKey);
                                file.ConnectFileServer();
                            }
                            //NetworkFile.NetFiles[(int)json["no"]].StartInSender((int)json["serverkey"]);
                        }
                        else
                        {
                            Receive(this, json);
                        }
                    }
                }
                catch(IOException e) // e.InnerException.GetType().Name == SocketException
                {
                    // 소켓통신에서 오류가 발생할경우 커넥션 강제 종료 (Ex 연결이 끊긴경우)
                    break;
                }
            }
            Dispose();
        }
        public bool Send(JObject message)
        {
            try
            {
                lock (SW)
                {
                    SW.WriteLine(JsonConvert.SerializeObject(message));
                    SW.Flush();
                }
                return true;
            }
            catch (SocketException e)
            {
                // 소켓통신 에러일경우 
                Dispose();
            }
            catch (System.ObjectDisposedException e)
            {
                // 오브젝트가 종료된경우, 더이상 메세지를 보내지 않고 무시함.
            }
            return false;
        }
        public void SendFile(JObject message, NetworkFile file)
        {
            JObject jObject = new JObject();
            jObject["type"] = -1;
            jObject["message"] = message;
            jObject["file"] = file.Information;
            Send(jObject);
        }
        public void Dispose()
        {
            lock (this)
            {
                if (Exit != null) Exit(this);
                isClosed = true;
                try { if (SR != null) SR.Dispose(); } catch { }
                try { if (SW != null) SW.Dispose(); } catch { }
                try { if (NS != null) NS.Dispose(); } catch { }
                try { if (tcpClient != null) tcpClient.Close(); } catch { }
                try { if (thread != null) thread.Abort(); } catch { }
            }
        }
    }
}
