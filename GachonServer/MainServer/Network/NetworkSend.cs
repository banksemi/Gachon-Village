using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    public static class NetworkSend
    {
        /// <summary>
        /// 게임에 접속중인 유저들에게 JSON 메세지를 전송합니다.
        /// 여기에는 접속을 하지 않았지만, 접속중인(로딩중인) 유저도 포함됩니다.
        /// </summary>
        /// <param name="json">전송할 메세지입니다.</param>
        /// <param name="ignoreuser">특정 유저에게만 메세지를 전송하고싶지 않을경우 이 파라미터를 사용합니다.</param>
        public static void SendAllUser(JObject json, User ignoreuser = null)
        {
            foreach(User user in User.Items.Values.ToList())
            {
                if (ignoreuser != user)
                {
                    user.socket.Send(json);
                }
            }
        }
    }
}
