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
        public static void Login(ESocket socket, string id, string password, bool GameLogin = true)
        {
            #region 입력값이 비어있는지 체크
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
            #endregion
            // 해당 입력값을 기준으로 GachonUser.GetObject 함수를 실행시킵니다.
            // 유효한 로그인일경우 GachonUser 클래스를 통해 실제 가천대 웹사이트와 세션이 연결되며,
            // 유효하지 않은 로그인일 경우 NULL값을 반환합니다.
            GachonUser gachonAccount = GachonUser.GetObject(id, password);
            if (gachonAccount == null)
            {
                NetworkMessageList.TipMessage(socket, "로그인에 실패했습니다.");
                return;
            }
            // 만약 게임에서 이 로그인을 요청한 경우 (안드로이드가 아닌)
            if (GameLogin)
            {
                // 게임 오브젝트로 관리될 새로운 User 클래스를 만들고, 소켓과 GachonUser 객체를 연결시킵니다.
                User user = null;
                try
                {
                    user = new User(socket, gachonAccount);
                }
                catch (DuplicationError e) // User 클래스는 같은 GachonUser 에 대해 중복으로 생성될수 없기때문에 발생하는 에러입니다.
                {
                    NetworkMessageList.TipMessage(socket, "이 계정은 다른 클라이언트에서 접속중입니다.");
                    return;
                }
                // 유저의 위치를 임의값으로 설정 (시작 포인트)
                user.position = new Vector4(-69.30f, 5.33f, 47.17f, 0f);
                
                // 접속 성공 메세지 전송
                JObject json = new JObject();
                json["type"] = NetworkProtocol.EnterWorld;
                json["no"] = user.no; // 플레이어를 나타내는 객체가 무엇인지 알려준다. (서버에서 관리되는 고유 번호)
                socket.Send(json);
            }
            else
            {
                // 로그인 성공 메세지를 보내준다.
                JObject json = new JObject();
                json["type"] = AndroidProtocol.Login;
                json["data"] = id + ":" + password;
                socket.Send(json);
            }
            // 가천 소켓에 이 세션을 연결시킨다.
            GachonSocket.Connect(socket, id, true);
        }
    }
}
