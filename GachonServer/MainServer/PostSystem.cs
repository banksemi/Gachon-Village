using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQL_Library;
using NetworkLibrary;
namespace MainServer
{
    public static class PostSystem
    {
        private static Dictionary<string, List<JObject>> MessageQueue = new Dictionary<string, List<JObject>>();
        private static void AddQueue(string id, JObject json)
        {
            lock (MessageQueue)
            {
                if (!MessageQueue.ContainsKey(id))
                    MessageQueue.Add(id, new List<JObject>());
            }
            lock (MessageQueue[id])
            {
                MessageQueue[id].Add(json);
            }
        }

        public static void GetPage(User user, int page_no)
        {
            MysqlNode mysqlNode = new MysqlNode(private_data.mysqlOption, "SELECT * FROM post_name where receiver=?id order by date desc limit 5");
            mysqlNode["id"] = user.ID;
            JArray array = new JArray();
            using (mysqlNode.ExecuteReader())
            {
                while(mysqlNode.Read())
                {
                    JObject item = new JObject();
                    item["title"] = mysqlNode.GetString("title");
                    string content = mysqlNode.GetString("content");
                    if (content.Length < 20)
                        item["content"] = content;
                    else
                        item["content"] = content.Substring(0, 20) + "...";
                    item["no"] = mysqlNode.GetInt("no");
                    item["sender"] = mysqlNode.GetString("sender_name");
                    DateTime date = mysqlNode.GetDateTime("date");
                    if (date.DayOfYear == DateTime.Now.DayOfYear)
                    {
                        item["date"] = date.ToString("hh:mm:ss");
                    }
                    else
                    {
                        item["date"] = date.ToString("yyyy-MM-dd");

                    }
                    array.Add(item);
                }
            }
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Post_Open;
            json["items"] = array;
            user.socket.Send(json);
        }

        public static List<JObject> GetMessage(string id)
        {
            if (!MessageQueue.ContainsKey(id)) return null;
            lock (MessageQueue[id])
            {
                if (MessageQueue[id].Count == 0) return null;
                // 받을 메세지가 남아있는경우 Queue에서 메세지를 Pop
                List<JObject> result = new List<JObject>();
                while (MessageQueue[id].Count != 0)
                {
                    result.Add(MessageQueue[id][0]);
                    MessageQueue[id].RemoveAt(0);
                }
                return result;
            }
        }
        public static void SendPost(string title, string content, string sender, string receiver)
        {
            DateTime date = DateTime.Now;
            MysqlNode Node = new MysqlNode(private_data.mysqlOption, "INSERT INTO post(title, content, sender, receiver, date) VALUES (?title, ?content, ?sender, ?receiver, ?date)");
            Node["title"] = title;
            Node["content"] = content;
            Node["sender"] = sender;
            Node["receiver"] = receiver;
            Node["date"] = date;
            Node.ExecuteNonQuery();

            JObject json = new JObject();
            json["type"] = NetworkProtocol.PostAlarm;
            json["title"] = title;
            json["content"] = content;
            json["receiver"] = receiver;
            json["date"] = date;
            // 받는사람이 게임 또는 스마트폰을 통해 접속중인가
            ESocket socket = GachonSocket.GetOnlineUser(receiver);
            if (socket != null)
            {
                socket.Send(json);
            }
            else
            {
                AddQueue(receiver, json);
            }
        }
    }
}
