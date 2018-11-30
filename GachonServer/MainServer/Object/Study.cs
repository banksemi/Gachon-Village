using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
using System.IO;
namespace MainServer
{
    class Study : NPC
    {
        public string key;
        public string master;
        public static new Dictionary<string, Study> Items = new Dictionary<string, Study>();
        public Study(string name, string master, Vector4 position)
        {
            skin = "Group Sign";
            key = name;
            this.name = name;
            this.master = master;
            this.group = "그룹";
            this.function = "그룹 상호작용";
            this.position = position;
            Items.Add(key, this);
        }

        public void OpenMenu(User user, string Tab = "Main")
        {
            // 해당 유저의 권한 체크
            int level = GetLevel(user);
            if (level == -1)
            {
                // 가입 윈도우 띄우기
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_SignUp;
                json["name"] = name;
                json["ui"] = true;
                user.socket.Send(json);
            }
            else if (level == 0)
            {
                user.ToChatMessage("해당 그룹에 이미 가입 요청을 보냈습니다.", ChatType.Notice);
            }
            else if (level == 1)
            {
                // 메뉴에 따른 내용 전달
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_UI;
                json["name"] = key;
                json["tab"] = Tab;
                if (Tab == "Main")
                {
                    // 메인 메뉴
                }
                else if (Tab == "Member")
                {
                    JArray array = new JArray();
                    MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM group_member WHERE group_id=?id");
                    node["id"] = key;
                    using (node.ExecuteReader())
                    {
                        while (node.Read())
                        {
                            JObject item = new JObject();
                            item["level"] = node.GetInt("level");
                            item["no"] = node.GetString("studentnumber");
                            item["name"] = node.GetString("name");
                            item["department"] = node.GetString("department");
                            item["student_id"] = node.GetString("student_id");
                            item["email"] = node.GetString("email");
                            array.Add(item);
                        }
                    }
                    json["items"] = array;
                }
                else if (Tab == "File")
                {

                }
                else if (Tab == "Chat")
                {
                    json["items"] = GetChattingData(20);
                }
                user.socket.Send(json);
            }
        }
        public string GetChattingData(int limit = 1000)
        {
            string data = "";
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM group_chat_report where group_name=?id ORDER BY no DESC limit ?line");
            node["line"] = limit;
            node["id"] = key;
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    data = "[" + node.GetDateTime("date").ToString("yyyy-MM-dd HH:mm:ss") + "] " + node.GetString("who") + " : " + node.GetString("data") +  "\r\n" + data;
                }
            }
            return data;
        }
        public void SignUpRequest(User user)
        {
            if (GetLevel(user) == -1)
            {
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO takes_course(student_id,course_no,level) VALUES (?id,?name,?level)");
                node["id"] = user.ID;
                node["name"] = key;
                node["level"] = 0;
                node.ExecuteInsertQuery();

                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_SignUp;
                json["ui"] = false;
                user.socket.Send(json);
                user.ToChatMessage(key + "그룹에 가입 신청을 완료했습니다.", ChatType.Notice);
                PostSystem.SendPost("그룹 가입 신청", "스터디 그룹\r\n[[-]" + key + "]\r\n\r\n가입 신청자\r\n" + user.name + " (" + user.gachonAccount.StudentNumber + ")\r\n\r\n새로운 회원가입 요청이 있습니다. 해당 그룹의 구역에서 가입 요청을 처리할 수 있습니다.", "admin_group", master);

            }
        }
        public int GetLevel(User user)
        {
            return GetLevel(user.ID);
        }
        public int GetLevel(string user_id)
        {
            // 해당 유저의 권한 체크
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT level FROM takes_course WHERE course_no=?group_name AND student_id=?id");
            node["group_name"] = key;
            node["id"] = user_id;
            using (node.ExecuteReader())
            {
                if (node.Read())
                {
                    return node.GetInt("level");
                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// 해당 그룹에 속한 모든 유저의 ID를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public List<string> GroupUsers()
        {
            List<string> idlist = new List<string>();
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT student_id FROM takes_course WHERE course_no=?groupid and level>0");
            node["groupid"] = name;
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    idlist.Add(node.GetString("student_id"));
                }
            }
            return idlist;
        }

        public void SaveChatting(User user)
        {
            if (GetLevel(user) <= 0)
            {
                NetworkMessageList.TipMessage(user.socket, "권한이 부족합니다.");
                return;
            }
            string FileName = FileSystem.GetRandomName(user.ID + key);
            File.AppendAllText(FileName, GetChattingData());

            long no = FileSystem.FileQuery(FileName, DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", user);
            user.AddFileItem((int)no);
            NetworkMessageList.TipMessage(user.socket, "해당 그룹의 채팅 내용이 인벤토리의 txt파일로 저장되었습니다!");
        }

        public void Member_Modify(User user, JObject json)
        {
            if (user.ID == (string)json["id"])
            {
                NetworkMessageList.TipMessage(user.socket, "자기 자신은 탈퇴시킬 수 없습니다. 메인메뉴 탭을 이용해주세요.");
                return;
            }
            if (master != user.ID)
            {
                NetworkMessageList.TipMessage(user.socket, "해당 그룹의 마스터만 이 작업을 수행할 수 있습니다.");
                return;
            }
            MysqlNode node;
            int old_level = -100;
            if ((bool)json["positive"]) // 가입 요청 수락
            {
                node = new MysqlNode(private_data.mysqlOption, "UPDATE takes_course SET level=?level WHERE course_no=?group_name AND student_id=?id AND level=?old_level");
                node["old_level"] = 0;
                node["level"] = 1;
            }
            else
            {
                old_level = GetLevel((string)json["id"]);
                node = new MysqlNode(private_data.mysqlOption, "DELETE FROM takes_course WHERE course_no=?group_name AND student_id=?id");
            }
            node["group_name"] = key;
            node["id"] = (string)json["id"];
            int result = node.ExecuteNonQuery();
            if (result > 0)
            {
                OpenMenu(user, "Member");
                NetworkMessageList.TipMessage(user.socket, "요청이 정상적으로 처리되었습니다.");
                if ((bool)json["positive"])
                {
                    PostSystem.SendPost("그룹에 가입되었습니다.", "스터디 그룹 [[-]" + name + "] 에 가입되었습니다!\r\n\r\n해당 그룹에서 많은 활동 부탁드립니다.", master, (string)json["id"]);
                }
                else
                {
                    if (old_level == 1)
                        PostSystem.SendPost("그룹에서 강퇴되었습니다.", "스터디 그룹 [[-]" + name + "] 에서 강퇴되었음을 알립니다.", master, (string)json["id"]);
                    else
                        PostSystem.SendPost("그룹 가입 신청이 거절되었습니다.", "스터디 그룹 [[-]" + name + "] 에서 가입 신청이 거절됨을 알립니다.", master, (string)json["id"]);
                }
            }
            else
            {
                NetworkMessageList.TipMessage(user.socket, "오류로 인해 처리되지 않았습니다.");
            }
        }
    }
}

