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
    public class GachonSocket
    {
        static Dictionary<string, List<ESocket>> session = new Dictionary<string, List<ESocket>>();

        static Dictionary<ESocket, string> session_string = new Dictionary<ESocket, string>();
        static Object lockobject = new object();
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
    }
}
