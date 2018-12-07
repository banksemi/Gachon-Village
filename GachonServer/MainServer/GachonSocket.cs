using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using SQL_Library;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    /// <summary>
    /// 현재 접속중인 소켓들을 관리합니다.
    /// </summary>
    public class GachonSocket
    {
        static Dictionary<string, List<ESocket>> session = new Dictionary<string, List<ESocket>>();

        static Dictionary<ESocket, string> session_string = new Dictionary<ESocket, string>();
        static Object lockobject = new object();
        /// <summary>
        /// 해당 소켓의 로그인 ID를 설정합니다.
        /// </summary>
        /// <param name="socket">새로운 소켓</param>
        /// <param name="id">로그인 ID</param>
        /// <param name="GameUser">게임 유저인가? (게임접속중이면 소켓메세지 전송에 우선순위 부여)</param>
        public static void Connect(ESocket socket, string id, bool GameUser = true)
        {
            lock(lockobject)
            {
                // Socket -> String
                if (!session_string.ContainsKey(socket)) session_string.Add(socket, id);

                // String -> Socket
                if (!session.ContainsKey(id))
                    session[id] = new List<ESocket>();
                if (GameUser)
                  session[id].Insert(0, socket);
                else
                    session[id].Add(socket);

                List<JObject> message = PostSystem.GetMessage(id);
                if (message != null)
                {
                    foreach (JObject json in message)
                    {
                        socket.Send(json);
                    }
                }
            }
        }
        public static void Exit(ESocket socket)
        {
            lock (lockobject)
            {
                // Socket -> String
                if (session_string.ContainsKey(socket))
                {
                    string id = session_string[socket];
                    session_string.Remove(socket);

                    session[id].Remove(socket);
                    if (session[id].Count == 0) session.Remove(id);
                }
            }
        }
        /// <summary>
        /// 해당 ID로 로그인중인 소켓을 모두 반환합니다. 이때는 하나의 소켓만 반환하며, 게임 소켓을 우선시합니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ESocket GetOnlineUser(string id)
        {
            lock (lockobject)
            {
                if (session.ContainsKey(id))
                {
                    return session[id][0];
                }
            }
            return null;
        }
        /// <summary>
        /// 해당 소켓의 로그인 권한을 얻고싶을때 사용합니다.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string GetId(ESocket socket)
        {
            lock (lockobject)
            {
                if (session_string.ContainsKey(socket))
                {
                    return session_string[socket];
                }
            }
            return null;
        }
    }
}
