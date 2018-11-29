using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GachonLibrary;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    public class User : GameObject
    {
        public static Dictionary<ESocket, User> Items = new Dictionary<ESocket, User>();
        public GachonUser gachonAccount { get; private set; }
        public ESocket socket { get; private set; }
        public string ID => gachonAccount.ID;
        public User(ESocket socket, GachonUser user)
        {
            lock(Items)
            {
                foreach(User item in Items.Values)
                {
                    if (item.gachonAccount == user) throw new DuplicationError("중복 로그인");
                }
                this.socket = socket;
                gachonAccount = user;
                Items.Add(socket, this);
                name = gachonAccount.Name;
                skin = "Eve";
            }
        }
        public void Dispose()
        {
            Remove();
            lock (Items)
            {
                Items.Remove(socket);
            }
        }
        public override void Start()
        {
            base.Start();
            // 해당 유저에게도 월드에 있는 다른 오브젝트 표시
            foreach(GameObject item in GameObject.Items.Values)
            {
                if (item != this)
                {
                    JObject json = item.InfoData();
                    json["type"] = NetworkProtocol.NewObject;
                    socket.Send(json);
                }
            }
            NetworkMessageList.TipMessage(socket, "가천 빌리지에 오신것을 환영합니다!");
            ToChatMessage("가천 빌리지에 오신것을 환영합니다!", ChatType.Notice);
            ToChatMessage("[컴퓨터 네트워크] 과목에 새로운 게시글이 등록되었습니다.", ChatType.System);
            ToChatMessage("이승화 : 귓속말 테스트~~~", ChatType.Whisper);
            ToChatMessage("위의 2개의 메세지는 디버그를 위해 출력되었습니다.", ChatType.Notice);
            int NewMessage = PostSystem.GetNewMessageCount(ID);
            if (NewMessage > 0)
            {
                ToChatMessage("[우편함] " + NewMessage + "개의 읽지 않은 우편이 존재합니다!", ChatType.System);
            }
        }
        /// <summary>
        /// 이 유저에게 채팅메세지를 전달합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Type"></param>
        public void ToChatMessage(string message, int Type)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["chattype"] = Type;
            json["message"] = message;
            socket.Send(json);
        }
        public override void Update()
        {
            base.Update();
        }
        public override void ChatMessage(string message, int Type)
        {
            //Dice
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;

            if (message.IndexOf("/주사위") == 0)
            {
                Random rd = new Random();
                json["chattype"] = ChatType.Notice;
                json["message"] = name + "(이)가 주사위를 굴렸습니다~!! 주사위가 " + rd.Next(1, 6).ToString() + " 나왔습니다!";
                NetworkSend.SendAllUser(json);
                return;
            }

            //Whisper
            if ((message.IndexOf("/ㅈ") == 0) || (message.IndexOf("/w") == 0) || (message.IndexOf("/귓속말") == 0))
            {
                string[] Receiver = message.Split(' ');
                //형식이 맞는지 확인
                if (Receiver.Length > 2)
                {
                    for (int i = 3; i < Receiver.Length; i++)
                    {
                        Receiver[2] += " " + Receiver[i];
                    }

                    //대상이 접속해있는지 아닌지 확인
                    foreach (User user in User.Items.Values.ToList())
                    {
                        if (user.name.Equals(Receiver[1]))
                        {
                            json["chattype"] = ChatType.Whisper;
                            json["message"] = Receiver[2]; //메세지 내용
                            json["no"] = no;
                            json["sender"] = name;
                            user.socket.Send(json);
                            return;
                        }
                    }
                    json["message"] = "[귓속말] 현재 접속해있지 않는 사용자입니다.";
                }
                else
                {
                    json["message"] = "[귓속말] 잘못된 귓속말 형식 입니다. '/w 대상 내용' 으로 입력해 주세요.";
                }
                json["chattype"] = ChatType.System;
                socket.Send(json);
                return;
            }
            base.ChatMessage(message, Type);
        }
    }
}
