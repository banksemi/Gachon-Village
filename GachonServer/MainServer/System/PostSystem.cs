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
                json["content"] = string.Format("[000000]{0}[-]", json["content"]); // BBCode를 이용해 글씨가 검정색임을 나타낸다.

                if (node.GetString("file") != null) // 스트링으로 null 체크
                {
                    MysqlNode filenode = new MysqlNode(private_data.mysqlOption, "SELECT name FROM file WHERE file_no=?no");
                    filenode["no"] = node.GetInt("file");
                    using (filenode.ExecuteReader())
                    {
                        if (filenode.Read())
                        {
                            json["content"] = string.Format("[000000]첨부 파일[-]\r\n[url={0}][u][B284FF]{1}[-][/u][/url]\r\n\r\n{2}", "file-" + filenode["no"],filenode.GetString("name"),json["content"]);
                        }
                    }
                }
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
        public static void SendPost(string title, string content, string sender, string receiver, bool notice = true, int file_no = -1)
        {
            DateTime date = DateTime.Now;
            MysqlNode Node = new MysqlNode(private_data.mysqlOption, "INSERT INTO post(title, content, sender, receiver, date, file) VALUES (?title, ?content, ?sender, ?receiver, ?date, ?file)");
            Node["title"] = title;
            Node["content"] = content;
            Node["sender"] = sender;
            Node["receiver"] = receiver;
            Node["date"] = date.ToString("yyyy-MM-dd HH:mm:ss");
            if (file_no != -1)
                Node["file"] = file_no;
            else
                Node["file"] = null;
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

        public static void GetFile(ESocket socket, JObject message)
        {
            // 파일 다운로드는 게임에서만 지원
            if (User.Items.ContainsKey(socket))
            {
                User user = User.Items[socket];

                // 요청한 파일, 우편이 실제 있는지 확인
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT 'true' FROM post WHERE no=?no and file=?file_no and receiver=?id");
                node["no"] = (int)message["post_no"];
                node["file_no"] = (int)message["file_no"];
                node["id"] = user.ID;
                using (node.ExecuteReader())
                {
                    if (node.Read())
                    {
                        if (user.AddFileItem((int)message["file_no"]))
                        {

                            NetworkMessageList.TipMessage(socket, "우편함의 첨부파일이 인벤토리에 추가되었습니다.");
                        }
                        else
                        {
                            NetworkMessageList.TipMessage(socket, "파일이 이미 인벤토리에 존재합니다.");
                        }
                    }
                    else
                    {
                        NetworkMessageList.TipMessage(socket, "잘못된 요청입니다.");
                    }
                }
            }
        }

        public static bool SendPost(ESocket socket, JObject message)
        {
            // 유효성 검사
            string id = GachonSocket.GetId(socket);
            if (id == null)
            {
                NetworkMessageList.TipMessage(socket, "로그인 권한을 얻을 수 없습니다. 다시 접속해주세요.");
                return false;
            }
            string title = ((string)message["title"]).Trim();
            if (string.IsNullOrEmpty(title))
            {
                NetworkMessageList.TipMessage(socket, "우편 제목을 입력해주세요.");
                return false;
            }
            string content = ((string)message["content"]);
            if (string.IsNullOrEmpty(content))
            {
                content = ""; // 우편 내용은 없어도 가능
            }
            string receiver = ((string)message["receiver"]);
            if (string.IsNullOrEmpty(receiver))
            {
                NetworkMessageList.TipMessage(socket, "받을 사람을 입력해주세요.");
                return false;
            }
            string receiver_id = GachonLibrary.GachonUser.GetID(receiver);
            if (receiver_id == null)
            {
                NetworkMessageList.TipMessage(socket, "데이터베이스에서 해당 유저를 찾을 수 없습니다. (가천빌리지에 한번이라도 로그인 해야함)");
                return false;
            }
            else if (receiver_id == "")
            {
                NetworkMessageList.TipMessage(socket, "수신자가 중복으로 존재합니다. 이름 뿐만 아니라 학번 또는 아이디 정보를 입력하여 받는 사람을 정확히 지정해주세요.");
                return false;
            }
            if (User.Items.ContainsKey(socket)) // 게임 접속 유저일경우
            {
                User user = User.Items[socket];
                int file_no = -1;
                if (message["file"] != null)
                {
                    file_no = (int)message["file"];
                    if (!user.HaveItem(file_no))
                    {
                        NetworkMessageList.TipMessage(socket, "해당 파일에 대한 권한이 없습니다. 인벤토리를 확인해주세요.");
                        return false;
                    }
                }
                SendPost(title, content, id, receiver_id, true, file_no);
                NetworkMessageList.TipMessage(socket, "우편을 성공적으로 전송하였습니다.");
                GetPage(user, 1); // 우편 리스트를 보며주는 UI로 연결
                return true;
            }
            else // 게임 유저가 아닐경우 (안드로이드)
            {
                SendPost(title, content, id, receiver_id);
                return true;
            }
        }
    }
}
