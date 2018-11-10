using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using GachonLibrary;
namespace MainServer
{
    static class Function
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
            GachonUser user = GachonUser.GetObject(id, password);
            if (user == null)
            {
                NetworkMessageList.TipMessage(socket, "로그인에 실패했습니다.");
            }
            else
            {
                NetworkMessageList.TipMessage(socket, "로그인에 성공했습니다.");
            }
        }
    }
}
