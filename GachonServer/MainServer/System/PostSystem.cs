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

        public static void GetPage(ESocket socket, string ID, int page_no)
        {
            if (page_no < 1) page_no = 1;
            int newcount = PostSystem.GetNewMessageCount(ID);
            int count = PostSystem.GetMessageCount(ID);
            int all_page = 1;
            if (count == 0) all_page = 1;
            else all_page = (count - 1) / 5 + 1;
            if (page_no > all_page) page_no = all_page;
            MysqlNode mysqlNode = new MysqlNode(private_data.mysqlOption, "SELECT * FROM post_name where receiver=?id order by date desc limit "+((page_no - 1) * 5) +", 5");
            mysqlNode["id"] = ID;
            JArray array = new JArray();
            using (mysqlNode.ExecuteReader())
            {
                while(mysqlNode.Read())
                {
                    JObject item = new JObject();
                    item["title"] = mysqlNode.GetString("title");
                    string content = WebSupport.ParseSupport.NoEnterString(mysqlNode.GetString("content"));
                    if (content.Length < 20)
                        item["content"] = content;
                    else
                        item["content"] = content.Substring(0, 20) + "...";
                    item["no"] = mysqlNode.GetInt("no");
                    item["sender"] = mysqlNode.GetString("sender_name");
                    DateTime date = mysqlNode.GetDateTime("date");
                    if (date.DayOfYear == DateTime.Now.DayOfYear)
                    {
                        item["date"] = date.ToString("HH:mm:ss");
                    }
                    else
                    {
                        item["date"] = date.ToString("yyyy-MM-dd");
                    }
                    item["read"] = mysqlNode.GetInt("read");
                    array.Add(item);
                }
            }
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Post_Open;
            json["items"] = array;
            json["newcount"] = newcount;
            json["count"] = count;
            json["page"] = page_no;
            json["all_page"] = all_page;
            socket.Send(json);
        }
        public static void GetPage(User user, int page_no)
        {
            GetPage(user.socket, user.ID, page_no);
        }
        public static void GetItem(User user, int no)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM post_name where receiver=?receiver and no=?no");
            node["no"] = no;
            node["receiver"] = user.ID;
            using (node.ExecuteReader())
            {
                if (node.Read() == false)
                {
                    user.ToChatMessage("존재하지 않거나 권한이 없는 우편입니다.", ChatType.System);
                    return;
                }
                if (node.GetInt("read") == 0)
                {
                    // 읽음 표시
                    MysqlNode update = new MysqlNode(private_data.mysqlOption, "UPDATE post SET `read`=1 where no=?no");
                    update["no"] = no;
                    update.ExecuteNonQuery();
                }
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Post_Detail;
                json["title"] = node.GetString("title");
                json["content"] = node.GetString("content");
                json["no"] = no;
                json["sender"] = node.GetString("sender_name");
                json["sender_id"] = node.GetString("sender");
                DateTime date = node.GetDateTime("date");
                if (date.DayOfYear == DateTime.Now.DayOfYear)
                {
                    json["date"] = date.ToString("HH:mm:ss");
                }
                else
                {
                    json["date"] = date.ToString("yyyy-MM-dd");
                }
                user.socket.Send(json);
            }
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
        public static int GetNewMessageCount(string id)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT count(*) as ncount from post where receiver=?id and `read`=0");
            node["id"] = id;
            using (node.ExecuteReader())
            {
                node.Read();
                return node.GetInt("ncount");
            }
        }
        public static int GetMessageCount(string id)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT count(*) as ncount from post where receiver=?id");
            node["id"] = id;
            using (node.ExecuteReader())
            {
                node.Read();
                return node.GetInt("ncount");
            }
        }
        public static void SendPost(string title, string content, string sender, string receiver, bool notice = true)
        {
            DateTime date = DateTime.Now;
            MysqlNode Node = new MysqlNode(private_data.mysqlOption, "INSERT INTO post(title, content, sender, receiver, date) VALUES (?title, ?content, ?sender, ?receiver, ?date)");
            Node["title"] = title;
            Node["content"] = content;
            Node["sender"] = sender;
            Node["receiver"] = receiver;
            Node["date"] = date.ToString("yyyy-MM-dd HH:mm:ss");
            long result = Node.ExecuteInsertQuery();

            // 받는사람이 게임 또는 스마트폰을 통해 접속중인가
            ESocket socket = GachonSocket.GetOnlineUser(receiver);
            if (socket != null)
            {
                if (notice == true)
                {
                    if (User.Items.ContainsKey(socket))
                    {
                        User.Items[socket].ToChatMessage("[우편함] 새로운 메세지가 도착했습니다.", ChatType.System);
                        return;
                    }
                }
                JObject json = new JObject();
                json["type"] = NetworkProtocol.PostAlarm;
                json["no"] = result;
                socket.Send(json);

            }
        }
    }
}
