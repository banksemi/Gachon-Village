using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using GachonLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    static partial class Function
    {
        public static void Login(ESocket socket, string id, string password)
        {
            if (id.Trim() == "")
            {
                NetworkMessageList.TipMessage(socket, "아이디를 입력해주세요.");
                return;
            }
            if (password.Trim() == "")
            {
                NetworkMessageList.TipMessage(socket, "비밀번호를 입력해주세요.");
                return;
            }
            GachonUser gachonAccount = GachonUser.GetObject(id, password);
            if (gachonAccount == null)
            {
                NetworkMessageList.TipMessage(socket, "로그인에 실패했습니다.");
                return;
            }
            User user = null;
            try
            {
                user = new User(socket, gachonAccount);
            }
            catch (DuplicationError e)
            {
                NetworkMessageList.TipMessage(socket, "이 계정은 다른 클라이언트에서 접속중입니다.");
                return;
            }
            GachonSocket.Connect(socket, id, true);
            user.position = new Vector4(-69.30f, 5.33f, 47.17f, 0f);
            JObject json = new JObject();
            json["type"] = NetworkProtocol.EnterWorld;
            json["no"] = user.no; // 플레이어를 나타내는 객체가 무엇인지 알려준다.
            socket.Send(json);
        }
    }
}
