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
using Newtonsoft.Json.Linq;
namespace MainServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(1119);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            while (true)
                System.Threading.Thread.Sleep(1000);
        }

        private static void Server_Exit(ESocket socket)
        {
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

            GachonCafe gachonCafe = new GachonCafe("135687");
            // 컴퓨터 네트워크에 2개의 카페를 연결.
            GachonObjects.AllClass["201809970002"].CombineSite(gachonCafe);
            // GachonObjects.AllClass["201809372002"].CombineSite(new GachonCafe("140663"));

            GachonClass gachonClass = GachonClass.GetObject("소프트웨어", "soft");
            gachonClass.CombineSite(new NaverCafe("gachon2010"));
            gachonClass.CombineTakeUser(a);
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
