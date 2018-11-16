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
            NetworkSend.SendAllUser(json);
        }
    }
}
