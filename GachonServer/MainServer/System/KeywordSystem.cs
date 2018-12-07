using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQL_Library;
using NetworkLibrary;
using GachonLibrary;

namespace MainServer
{
    public static class KeywordSystem
    {
        /// <summary>
        /// 특정 유저에게 입력된 모든 키워드 리스트를 반환합니다.
        /// </summary>
        /// <param name="user"></param>
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
            // 메세지 전송
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Keyword_Open;
            json["list"] = array;
            user.socket.Send(json);
        }
        /// <summary>
        /// 해당 키워드가 여기에 입력된 게시글을 가르키는지 여부를 판단하는 함수입니다.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="course_name"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static bool CheckValid(string keyword, string course_name, string title)
        {           
            string[] target;
            string targetKeyword;
            bool correspond_course = false;
            int start = keyword.IndexOf('[');
            try
            {
                //특정 과목이 지정되었을 경우
                if (start != -1)
                {
                    target = keyword.Split(']');
                    target[0] = target[0].Trim(); //Target course
                    target[0] = target[0].Substring(start + 1);
                    targetKeyword = target[1].Trim(); //Target keyword

                    //Target Course가 일치할 경우
                    if (course_name.IndexOf(target[0]) != -1)
                    {
                        correspond_course = true;
                    }
                }
                else
                {
                    targetKeyword = keyword.Trim();
                    correspond_course = true;
                }

                if (correspond_course)
                {
                    if (title.IndexOf(targetKeyword) != -1)
                    {
                        return true;
                    }
                }
            }catch (Exception)
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// 가천 라이브러리에서 새로운 게시글이 올라오면 이 함수를 이벤트로 실행시킵니다.
        /// </summary>
        /// <param name="gclass">게시글이 올라온 강의</param>
        /// <param name="postItem">게시글</param>
        public static void NewPost(GachonClass gclass, PostItem postItem)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM keyword");
            JArray array = new JArray();
            string ignore_id = "";
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    if (ignore_id == node.GetString("student_id")) continue;
                    string keyword = node.GetString("keyword");
                    if (CheckValid(keyword, gclass.Title, postItem.Title))
                    {
                        ignore_id = node.GetString("student_id");
                        PostSystem.SendPost(
                            string.Format("[{0}] 새로운 게시글 등록", gclass.Title),
                            string.Format("[게시글 정보]\r\n{0} - {1}\r\n\r\n{2}\r\n\r\nURL : {3}", postItem.Title, postItem.Publisher, postItem.Content, postItem.url),
                            "admin_keyword",
                            ignore_id
                            , false);
                        ESocket socket = GachonSocket.GetOnlineUser(ignore_id);
                        if (socket != null)
                        {
                            if (User.Items.ContainsKey(socket)) User.Items[socket].ToChatMessage("[" + gclass.Title + "] 에 새로운 게시글이 등록되었습니다.", ChatType.System);
                        }
                    }
                }
            }
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
