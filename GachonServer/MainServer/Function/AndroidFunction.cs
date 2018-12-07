using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    class AndroidFunction
    {
        public static void KeywordList(ESocket socket)
        {
            string id = GachonSocket.GetId(socket);
            if (id != null)
            {
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM keyword WHERE student_id=?id");
                node["id"] =id;
                JArray array = new JArray();
                using (node.ExecuteReader())
                {
                    while (node.Read())
                    {
                        array.Add(node.GetString("keyword"));
                    }
                }
                JObject json = new JObject();
                json["type"] = AndroidProtocol.KeywordList;
                json["list"] = array;
                socket.Send(json);
            }
        }
        public static void NewKeyword(ESocket socket, string keyword)
        {
            string id = GachonSocket.GetId(socket);
            if (id != null)
            {
                keyword = keyword.Trim();
                if (keyword.Length == 0)
                {
                    NetworkMessageList.TipMessage(socket, "추가하실 키워드를 입력해주세요.");
                    return;
                }
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO keyword(student_id, keyword) VALUES (?id, ?keyword)");
                node["id"] = id;
                node["keyword"] = keyword;
                int result = node.ExecuteNonQuery();
                if (result != 1)
                {
                    NetworkMessageList.TipMessage(socket, "이미 등록된 키워드입니다.");
                }
                else if (result == 0)
                {
                    NetworkMessageList.TipMessage(socket, "오류로 인해 키워드 추가가 불가능합니다 (ERROR CODE : " + result + ")");
                }
                else
                {
                    NetworkMessageList.TipMessage(socket, "[키워드] \"" + keyword + "\" 가 추가되었습니다.");
                    KeywordList(socket);
                }
            }
        }
        public static void SendTakesList(ESocket socket)
        {
            string id = GachonSocket.GetId(socket);
            if (id != null)
            {
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM takes_course JOIN course ON takes_course.course_no=course.no WHERE student_id=?id");
                node["id"] = id;
                JArray group = new JArray();
                JArray course = new JArray();
                using (node.ExecuteReader())
                {
                    while(node.Read())
                    {
                        if (node.GetString("type") == "Class")
                        {
                            course.Add(node.GetString("name"));
                        }
                        else if (node.GetString("type") == "Group")
                        {

                            group.Add(node.GetString("name"));
                        }
                    }
                }

                JObject json = new JObject();
                json["type"] = AndroidProtocol.GroupList;
                json["group"] = group;
                json["class"] = course;
                socket.Send(json);
            }
        }
        public static void GetPostList(ESocket socket, int no)
        {
            string id = GachonSocket.GetId(socket);
            if (id != null)
            {
                MysqlNode mysqlNode = new MysqlNode(private_data.mysqlOption, "SELECT * FROM post_name where receiver=?id and no > ?no order by date");
                mysqlNode["no"] = no;
                mysqlNode["id"] = id;
                JArray array = new JArray();
                using (mysqlNode.ExecuteReader())
                {
                    while (mysqlNode.Read())
                    {
                        JObject item = new JObject();
                        item["title"] = mysqlNode.GetString("title");
                        item["content"] = mysqlNode.GetString("content");
                        item["no"] = mysqlNode.GetInt("no");
                        item["sender"] = mysqlNode.GetString("sender_name");
                        item["sender_id"] = mysqlNode.GetString("sender");
                        item["date"] = (mysqlNode.GetDateTime("date")).ToString("yyyy-MM-dd HH:mm:ss");
                        array.Add(item);
                    }
                }

                JObject json = new JObject();
                json["type"] = AndroidProtocol.PostList;
                json["items"] = array;
                socket.Send(json);
                return;
            }
        }
    }
}
