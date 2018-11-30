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

        public void OpenMenu(User user)
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
                user.ToChatMessage(key + "그룹에 가입 신청을 완료했습니다.",ChatType.Notice);
                PostSystem.SendPost("그룹 가입 신청", "스터디 그룹\r\n[[-]" + key + "]\r\n\r\n가입 신청자\r\n" + user.name + " (" + user.gachonAccount.StudentNumber + ")\r\n\r\n새로운 회원가입 요청이 있습니다. 해당 그룹의 구역에서 가입 요청을 처리할 수 있습니다.", "admin_group", master);
                
            }
        }
        public int GetLevel(User user)
        {
            // 해당 유저의 권한 체크
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT level FROM takes_course WHERE course_no=?group_name AND student_id=?id");
            node["group_name"] = key;
            node["id"] = user.ID;
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
    }
}
