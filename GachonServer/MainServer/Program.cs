using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GachonLibrary;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using NetworkLibrary;
using NetworkLibrary.File;
using Newtonsoft.Json.Linq;
using SQL_Library;
namespace MainServer
{
    class Program
    {
        static MysqlOption SqlOption = private_data.mysqlOption;
        static void Main(string[] args)
        {
            if (SqlOption.Host == "mysql_host") throw new Exception("private_data에 MYSQL 정보를 입력해주세요.");
            GachonOption.MysqlOption = SqlOption;
            Function.Init_Load();

            GachonClass.NewPost += KeywordSystem.NewPost;
            GachonClass.AutoCrawlingDelay = 3; // (초단위)  10초에 한번씩 최신글을 확인
            GachonClass.StartAutoCrawling = true; // 강의 최신글 크롤링 시작.

            Server server = new Server(1119);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive_Try;
            server.Exit += Server_Exit;
            server.FileInfoReceive += Server_FileInfoReceive;
            new System.Threading.Thread(UpdateThread).Start();
            for(int i = 0; i < 15;i++)
            {
               // PostSystem.SendPost("실시간 알림 테스트" + i, "Queue 테스트", "admin_keyword", "banksemi");
            }
            int i2 = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(4000);
                string title = "실시간 알림 테스트" + i2++;
                Console.WriteLine(title);
                //PostSystem.SendPost(title, "Queue 테스트", "admin_keyword", "banksemi");
            }
        }

        private static void Server_FileInfoReceive(ESocket socket, JObject Message, NetworkFile file)
        {
            string name = FileSystem.GetRandomName(file.FileName);
            file.Success += delegate (NetworkFile files)
            {
                // Mysql에 등록. 알려줌
                long no = FileSystem.FileQuery(name, file.FileName, User.Items[socket]);
                User.Items[socket].AddFileItem((int)no);
            };
            file.Accept(name);
        }

        private static void Server_Exit(ESocket socket)
        {
            Console.WriteLine("누군가 종료함");
            GachonSocket.Exit(socket);
            if (User.Items.ContainsKey(socket))
            {
                User.Items[socket].Dispose();
            }
        }
        private static void Server_Receive_Try(ESocket socket, JObject Message)
        {
            try
            {
                Server_Receive(socket, Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
            }
        }

        private static void Server_Receive(ESocket socket, JObject Message)
        {
            if ((int)Message["type"] >= 1000)
            {
                switch ((int)Message["type"])
                {
                    case AndroidProtocol.TestMessage:
                        Console.WriteLine("안드로이드 테스트 메세지 : " + (string)Message["message"]);
                        break;
                    case AndroidProtocol.Login:
                        Function.Login(socket, (string)Message["id"], (string)Message["password"], false);
                        break;
                    case AndroidProtocol.GroupList:
                        AndroidFunction.SendTakesList(socket);
                        return;
                    case AndroidProtocol.KeywordList:
                        AndroidFunction.KeywordList(socket);
                        break;
                    case AndroidProtocol.KeywordAdd:
                        AndroidFunction.NewKeyword(socket,(string)Message["keyword"]);
                        break;
                    case AndroidProtocol.PostList:
                        if (Message["no"] != null)
                            AndroidFunction.GetPostList(socket, (int)Message["no"]);
                        break;
                    case AndroidProtocol.Info:
                        AndroidFunction.ReturnInfo(socket);
                        break;

                }
            }
            else
            {
                switch ((int)Message["type"])
                {
                    case NetworkProtocol.Login:
                        Function.Login(socket, (string)Message["id"], (string)Message["password"], true);
                        break;
                    case NetworkProtocol.EnterWorld:
                        User.Items[socket].Start();
                        break;
                    case NetworkProtocol.Move:
                        User.Items[socket].Move(new Vector4((float)Message["x"], (float)Message["y"], (float)Message["z"], (float)Message["q"]));
                        break;
                    case NetworkProtocol.Chat:
                        User.Items[socket].ChatMessage((string)Message["message"],ChatType.Normal);
                        break;
                    case NetworkProtocol.Action:
                        Function.NPC_Action((NPC)GameObject.Items[(int)Message["no"]], User.Items[socket]);
                        break;
                    case NetworkProtocol.Post_Open:
                        PostSystem.GetPage(User.Items[socket], (int)Message["page"]);
                        break;
                    case NetworkProtocol.Post_Detail:
                        PostSystem.GetItem(User.Items[socket], (int)Message["no"]);
                        break;
                    case NetworkProtocol.NewStudy:
                        StudySystem.NewStudy(User.Items[socket], Message);
                        break;
                    case NetworkProtocol.Keyword_Remove:
                        KeywordSystem.RemoveItem(User.Items[socket], (string)Message["keyword"]);
                        break;
                    case NetworkProtocol.Keyword_Add:
                        KeywordSystem.AddItem(User.Items[socket], (string)Message["keyword"]);
                        break;
                    case NetworkProtocol.Inventory_Remove:
                        User.Items[socket].RemoveItem((int)Message["no"]);
                        break;
                    case NetworkProtocol.File_Download:
                        User.Items[socket].DownloadItem((int)Message["no"], (string)Message["path"]);
                        break;
                    case NetworkProtocol.Study_SignUp:
                        Study.Items[(string)Message["name"]].SignUpRequest(User.Items[socket]);
                        break;
                    case NetworkProtocol.Study_UI:
                        Study.Items[(string)Message["name"]].OpenMenu(User.Items[socket], (string)Message["tab"]);
                        break;
                    case NetworkProtocol.Study_Member_Request:
                        Study.Items[(string)Message["name"]].Member_Modify(User.Items[socket], Message);
                        break;
                    case NetworkProtocol.Study_SaveChatting:
                        Study.Items[(string)Message["name"]].SaveChatting(User.Items[socket]);
                        break;
                    case NetworkProtocol.Study_FileUpload:
                        Study.Items[(string)Message["group_name"]].FileUpload(User.Items[socket], (int)Message["no"]);
                        break;
                    case NetworkProtocol.Study_FileDownload:
                        Study.Items[(string)Message["group_name"]].FileDownload(User.Items[socket], (int)Message["no"]);
                        break;
                    case NetworkProtocol.SendPost:
                        NetworkMessageList.SendMessageResult(socket, PostSystem.SendPost(socket, Message));
                        break;
                    case NetworkProtocol.GetFileInPost:
                        PostSystem.GetFile(socket, Message);
                        break;
                }
            }
        }
        public static void UpdateThread()
        {
            int delay = 100;
            DateTime time = DateTime.Now;
            while(true)
            {
                if ((DateTime.Now - time).TotalMilliseconds >= delay)
                {
                    try
                    {
                        foreach(GameObject gameObject in GameObject.Items.Values)
                        {
                            gameObject.Update();
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    time =  time.AddMilliseconds(delay);
                }
                System.Threading.Thread.Sleep(5);
            }
        }
        private static void Server_Connect(ESocket socket)
        {
            Console.WriteLine("로그인");
            NetworkMessageList.TipMessage(socket, "서버에 연결되었습니다.");
       }
    }
}
