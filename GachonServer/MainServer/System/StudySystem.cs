using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQL_Library;

namespace MainServer
{
    public class StudySystem
    {
        public static void NewStudy(User user, JObject message)
        {
            string key = ((string)message["name"]).Trim();
            if (key.Length < 2 || key.Length > 14)
            {
                NetworkMessageList.TipMessage(user.socket, "스터디 이름은 2글자 이상 14글자 이내입니다.");
                return;
            }
            // 반경 내에 스터디 그룹이 있는지 체크
            if (user.InGroup(35) != null)
            {
                NetworkMessageList.TipMessage(user.socket,"가까운 거리에 다른 스터디 그룹이 존재합니다.");
                return;
            }
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO course(no,type,name) VALUES (?name,'Group',?name)");
            node["name"] = key;
            int result = node.ExecuteNonQuery();
            if (result == -1)
            {
                NetworkMessageList.TipMessage(user.socket, "이미 같은 이름의 스터디 그룹이 존재합니다.");
                return;
            }
            else if (result <= 0)
            {
                NetworkMessageList.TipMessage(user.socket, "데이터베이스의 예기치 못한 오류가 있습니다.");
                return;
            }
            // 현재 위치를 데이터베이스에 저장
            Vector4 position = user.position;
            node = new MysqlNode(private_data.mysqlOption, "INSERT INTO `group`(group_name,master,x,y,z,q) VALUES (?name,?master,?x,?y,?z,?q)");
            node["name"] = key;
            node["master"] = user.ID;
            node["x"] = position.x;
            node["y"] = position.y;
            node["z"] = position.z;
            node["q"] = position.q;
            node.ExecuteNonQuery();

            node = new MysqlNode(private_data.mysqlOption, "INSERT INTO takes_course(student_id,course_no) VALUES (?id,?key)");
            node["id"] = user.ID;
            node["key"] = key;
            node.ExecuteNonQuery();
            // 성공적으로 만들어진경우
            Study group = new Study(key,user.ID, position);
            group.Start();

            JObject json = new JObject();
            json["type"] = NetworkProtocol.CloseNewStudy;
            user.socket.Send(json);
            NetworkMessageList.TipMessage(user.socket, "스터디 그룹을 성공적으로 만들었습니다!");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("스터디 이름 : " + key);
            sb.AppendLine("그룹 마스터 : " + user.ID);
            sb.AppendLine("");
            sb.AppendLine("새로운 스터디를 개설하신것을 축하드립니다!!!");
            sb.AppendLine("스터디의 펫말을 통해 새로운 그룹원을 초대하고, 그룹 내에서 진행된 대화를 확인할 수 있습니다.");
            sb.AppendLine("");
            sb.AppendLine("앞으로 더 많은 기능이 추가될 예정이니 많은 관심 부탁드립니다~~");
            sb.AppendLine("");
            sb.AppendLine("만약 스터디 그룹에 대해 문의사항이 있다면");
            sb.AppendLine("스터디 알림(admin_group)으로 메세지를 보내주시기 바랍니다.");
            sb.AppendLine("");
            sb.AppendLine("-가천 빌리지-");
            PostSystem.SendPost("새로운 스터디가 만들어졌습니다.", sb.ToString(), "admin_group", user.ID);
        }
    }
}
