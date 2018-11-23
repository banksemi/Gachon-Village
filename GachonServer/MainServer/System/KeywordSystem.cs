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
    public static class KeywordSystem
    {
        public static void GetList(User user)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM keyword WHERE student_id=?id");
            node["id"] = user.ID;
            JArray array = new JArray();
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    array.Add(node.GetString("keyword"));
                }
            }
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Keyword_Open;
            json["list"] = array;
            user.socket.Send(json);
        }

        public static void RemoveItem(User user, string keyword)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "DELETE FROM keyword WHERE student_id=?id and keyword=?keyword");
            node["id"] = user.ID;
            node["keyword"] = keyword;
            int result = node.ExecuteNonQuery();
            if (result != 1)
            {
                NetworkMessageList.TipMessage(user.socket, "오류로 인해 키워드 삭제가 불가능합니다 (ERROR CODE : " + result + ")");
            }
            else
            {
                user.ToChatMessage("[키워드] \"" + keyword + "\" 가 삭제되었습니다.",ChatType.Notice);
                GetList(user);
            }
        }
        public static void AddItem(User user, string keyword)
        {
            keyword = keyword.Trim();
            if (keyword.Length == 0)
            {
                NetworkMessageList.TipMessage(user.socket, "추가하실 키워드를 입력해주세요.");
                return;
            }
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO keyword(student_id, keyword) VALUES (?id, ?keyword)");
            node["id"] = user.ID;
            node["keyword"] = keyword;
            int result = node.ExecuteNonQuery();
            if (result != 1)
            {
                NetworkMessageList.TipMessage(user.socket, "이미 등록된 키워드입니다.");
            }
            else if (result == 0)
            {
                NetworkMessageList.TipMessage(user.socket, "오류로 인해 키워드 추가가 불가능합니다 (ERROR CODE : " + result + ")");
            }
            else
            {
                user.ToChatMessage("[키워드] \"" + keyword + "\" 가 추가되었습니다.", ChatType.Notice);
                GetList(user);
            }

        }
    }
}
