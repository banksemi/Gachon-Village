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
            GachonOption.MysqlOption = SqlOption;
            Function.Init_Load();

            GachonClass.NewPost += KeywordSystem.NewPost;
            GachonClass.AutoCrawlingDelay = 3; // (초단위)  10초에 한번씩 최신글을 확인
            GachonClass.StartAutoCrawling = true; // 강의 최신글 크롤링 시작.

            Server server = new Server(1119);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            server.FileInfoReceive += Server_FileInfoReceive;
            new System.Threading.Thread(UpdateThread).Start();
            for(int i = 0; i < 15;i++)
            {
               // PostSystem.SendPost("실시간 알림 테스트" + i, "Queue 테스트", "admin_keyword", "banksemi");
            }
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
               // PostSystem.SendPost("실시간 알림 테스트", "Queue 테스트", "admin_keyword", "banksemi");
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
            GachonSocket.Exit(socket);
            if (User.Items.ContainsKey(socket))
            {
                User.Items[socket].Dispose();
            }
        }

        private static void Server_Receive(ESocket socket, Newtonsoft.Json.Linq.JObject Message)
        {
            switch((int)Message["type"])
            {
                case NetworkProtocol.Login:
                    Function.Login(socket, (string)Message["id"], (string)Message["password"]);
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

        static void Debug()
        {
            GachonClass.NewPost += GachonClass_NewPost;
            //가천대 학생 객체 만들기 (학교 아이디와 패스워드로 로그인)
            GachonUser a = GachonUser.GetObject(private_data.id, private_data.password);
            GachonUser c = GachonUser.GetObject(private_data.id, private_data.password);
            // 해당 학생 정보 출력
            Console.WriteLine(a.ToString(true));
            // b.Name b.Department   b.Email   b.Phone 등으로 참조 가능

            //GachonCafe gachonCafe = new GachonCafe("135687");
            // 컴퓨터 네트워크에 2개의 카페를 연결.
            //GachonObjects.AllClass["201809970002"].CombineSite(gachonCafe);
            //GachonObjects.AllClass["201809372002"].CombineSite(new GachonCafe("140663"));
            
            // GachonClass gachonClass = GachonClass.GetObject("소프트웨어", "soft");
            //gachonClass.CombineSite(new NaverCafe("gachon2010"));
            a.CombineClass(GachonClass.GetObject("소프트웨어", "soft"));
            Console.WriteLine("\r\n\r\n서버에 등록된 모든 강의");
            foreach (GachonClass gc in GachonObjects.AllClass.Values)
            {
                Console.WriteLine(gc.ToString());
            }

            Console.WriteLine("\r\n\r\n");
            GachonClass.AutoCrawlingDelay = 10; // (초단위)  3초에 한번씩 최신글을 확인
            GachonClass.StartAutoCrawling = true; // 강의 최신글 크롤링 시작.
            Console.ReadKey(); //다른 키가 눌릴때까지 대기
            // 다른키가 눌렸을 경우 오토크롤링 종료
            GachonClass.StartAutoCrawling = false;
        }
        // 가천대 강의로부터 새로운 게시글이 등록될경우 이 함수가 실행됨.
        private static void GachonClass_NewPost(GachonClass gclass, PostItem postItem)
        {
            Console.WriteLine(String.Format("[알림 - {0}] {1} -> {2}", postItem.posttype.ToString(), gclass.Title, postItem.ToString()));
            Console.WriteLine("\t\t" + postItem.Content_Briefly + "\r\n");
          //  Console.WriteLine("[알림 - " + postItem.posttype.ToString() + "] " + gclass.Title + " -> " + postItem.ToString() + "       || " + postItem.Content_Briefly);
        }
    }
}
