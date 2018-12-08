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
        /// <summary>
        /// 이 객체가 Mysql에 접근할때 사용하는 고유키입니다. Name과 같습니다.
        /// </summary>
        public string key;
        /// <summary>
        /// 이 스터디 그룹의 마스터(그룹장)입니다.
        /// </summary>
        public string master;
        /// <summary>
        /// 스터디 객체들의 집합입니다. GameObject.Items 변수를 숨깁니다.
        /// </summary>
        public static new Dictionary<string, Study> Items = new Dictionary<string, Study>();
        /// <summary>
        /// 이 그룹에 속한 구성원 수입니다.
        /// </summary>
        public int Count
        {
            get
            {
                return Users.Count;
            }
        }
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
        /// <summary>
        /// 해당 유저가 이 스터디그룹에 상호작용을 요청했을때 실행되는 함수입니다.
        /// </summary>
        /// <param name="user">상호작용을 요청한 유저</param>
        /// <param name="Tab">윈도우 Tab (Main, Member, File, Chat)</param>
        public void OpenMenu(User user, string Tab = "Main")
        {
            // 해당 유저의 권한 체크
            int level = GetLevel(user);

            // 가입 정보를 찾을수 없는 경우 (-1)
            if (level == -1)
            {
                // 가입 윈도우 띄우기
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_SignUp;
                json["name"] = name;
                json["ui"] = true;
                user.socket.Send(json);
            }
            else if (level == 0) // 가입 정보는 있지만 마스터의 수락을 받지 못한 경우 (0)
            {
                user.ToChatMessage("해당 그룹에 이미 가입 요청을 보냈습니다.", ChatType.Notice);
            }
            else if (level == 1) // 해당 그룹의 구성원인 경우
            {
                // 메뉴에 따른 내용 전달
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_UI;
                json["name"] = key;
                json["tab"] = Tab;
                if (Tab == "Main") // 메인 메뉴인 경우
                {
                    // 마스터 이름, 그룹개설일, 멤버수 정보를 반환 메세지에 담는다.
                    MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM group_info WHERE group_name=?id");
                    node["id"] = key;
                    using (node.ExecuteReader())
                    {
                        if (node.Read())
                        {
                            json["master_name"] = node.GetString("master_name");
                            json["master_id"] = node.GetString("master_id");
                            json["start_date"] = node.GetDateTime("start_date");
                            json["count"] = node.GetInt("member");
                        }
                    }
                }
                else if (Tab == "Member") // 멤버 메뉴일 경우
                {
                    // 멤버 정보를 반환 메세지에 담는다.
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
                    // 파일 목록을 메세지에 담는다.
                    json["items"] = Files;
                }
                else if (Tab == "Chat")
                {
                    // 채팅기록중 마지막 20줄을 반환 메세지에 담는다.
                    json["items"] = GetChattingData(20);
                }
                user.socket.Send(json);
            }
        }
        /// <summary>
        /// 해당 그룹의 채팅 기록을 가져옵니다.
        /// </summary>
        /// <param name="limit">끝에서부터 몇번째까지 가져올지 설정합니다.</param>
        /// <returns></returns>
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
        /// <summary>
        /// 유저가 가입 신청을 할때 실행되는 함수입니다.
        /// </summary>
        /// <param name="user">가입 신청할 유저</param>
        public void SignUpRequest(User user)
        {
            // 해당 유저가 이 그룹에 대해 아무런 정보가 없을때
            if (GetLevel(user) == -1)
            {
                // 그룹 구성원 목록에 레벨 0으로 추가
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO takes_course(student_id,course_no,level) VALUES (?id,?name,?level)");
                node["id"] = user.ID;
                node["name"] = key;
                node["level"] = 0;
                node.ExecuteInsertQuery();

                // 가입 완료 메세지 전송
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Study_SignUp;
                json["ui"] = false;
                user.socket.Send(json);
                user.ToChatMessage(key + "그룹에 가입 신청을 완료했습니다.", ChatType.Notice);

                // 마스터에게 가입 메세지 전송
                PostSystem.SendPost("그룹 가입 신청", "스터디 그룹\r\n[" + key + "]\r\n\r\n가입 신청자\r\n" + user.name + " (" + user.gachonAccount.StudentNumber + ")\r\n\r\n새로운 회원가입 요청이 있습니다. 해당 그룹의 구역에서 가입 요청을 처리할 수 있습니다.", "admin_group", master);
            }
        }
        /// <summary>
        /// 이 그룹에서 해당 유저의 레벨(그룹에서의 레벨)을 반환합니다.
        /// </summary>
        /// <param name="user">유저 객체</param>
        /// <returns>-1 (가입 안함), 0(가입 대기 중), 1(그룹원)</returns>
        public int GetLevel(User user)
        {
            return GetLevel(user.ID);
        }
        /// <summary>
        /// 이 그룹에서 해당 유저의 레벨(그룹에서의 레벨)을 반환합니다.
        /// </summary>
        /// <param name="user_id">아이디</param>
        /// <returns>-1 (가입 안함), 0(가입 대기 중), 1(그룹원)</returns>
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
        public List<string> Users
        {
            get
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
        /// <summary>
        /// 해당 그룹의 파일을 모두 반환합니다.
        /// </summary>
        public JArray Files
        {
            get
            {
                JArray jArray = new JArray();
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM group_file_info WHERE group_name=?key");
                node["key"] = key;
                using (node.ExecuteReader())
                {
                    while (node.Read())
                    {
                        JObject json = new JObject();
                        json["no"] = node.GetInt("file_no");
                        json["name"] = node.GetString("name");
                        json["size"] = node.GetInt("size");
                        json["owner"] = node.GetString("owner");
                        json["upload_user"] = node.GetString("upload_user");
                        json["file_date"] = node.GetDateTime("date");
                        json["upload_date"] = node.GetDateTime("upload_date");
                        jArray.Add(json);
                    }
                }
                return jArray;
            }
        }
        /// <summary>
        /// 이 그룹이 특정 파일을 가지고 있는지 권한을 체크합니다.
        /// </summary>
        /// <param name="no">파일 번호 (Mysql)</param>
        /// <returns></returns>
        public bool HaveItem(int no)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM group_file_info WHERE group_name=?key AND file_no=?no");
            node["key"] = key;
            node["no"] = no;
            using (node.ExecuteReader())
            {
                if (node.Read()) return true;
            }
            return false;
        }
        /// <summary>
        /// 특정 유저에게 그동안의 채팅 기록을 인벤토리로 전송합니다.
        /// </summary>
        /// <param name="user">파일을 받을 유저</param>
        public void SaveChatting(User user)
        {
            // 권한 체크
            if (GetLevel(user) <= 0)
            {
                NetworkMessageList.TipMessage(user.socket, "권한이 부족합니다.");
                return;
            }
            // 파일 이름 . 그룹 이름에는 특수문자가 들어있을 수 있음 그렇기때문에 파일 이름에 그룹 이름을 추가하지 않음.
            string FileName = FileSystem.GetRandomName(user.ID + key);
            File.AppendAllText(FileName, GetChattingData()); // 파일 쓰기

            // 서버에 새로운 파일 등록
            long no = FileSystem.FileQuery(FileName, DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", user);
            // 유저에게 해당 파일을 인벤토리로 전송
            user.AddFileItem((int)no);
            NetworkMessageList.TipMessage(user.socket, "해당 그룹의 채팅 내용이 인벤토리의 TXT 파일로 저장되었습니다!");
        }
        /// <summary>
        /// 특정 유저가 파일을 그룹에 추가합니다. (파일은 이미 업로드된 상태)
        /// </summary>
        /// <param name="user">파일을 올릴 유저</param>
        /// <param name="no">파일 번호 (Mysql)</param>
        public void FileUpload(User user, int no)
        {
            // 해당 유저의 권한 체크
            int level = GetLevel(user);
            if (GetLevel(user) < 1)
            {
                user.ToChatMessage("해당 그룹에 파일을 업로드할 권한이 없습니다.", ChatType.Notice);
                return;
            }
            if (!user.HaveItem(no)) // 해당 유저가 그 아이템(파일)을 정말 가지고 있는가
            {
                NetworkMessageList.TipMessage(user.socket, "해당 파일이 인벤토리에 없어서 업로드에 실패했습니다.");
                return;
            }
            // Mysql에 파일 정보 추가
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO group_file(group_name, file_no, upload_user) VALUES (?group_name, ?file_no, ?user)");
            node["group_name"] = key;
            node["file_no"] = no;
            node["user"] = user.ID;
            int result = node.ExecuteNonQuery();
            if (result > 0)
            {
                NetworkMessageList.TipMessage(user.socket, "파일을 스터디에 성공적으로 업로드했습니다.");
                OpenMenu(user, "File");
            }
            else
            {
                NetworkMessageList.TipMessage(user.socket, "예기치 못한 오류로 업로드에 실패했습니다.");
            }
        }
        /// <summary>
        /// 이 그룹에서 파일을 해당 유저의 인벤토리로 전송하고, 메세지를 표시합니다.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="no"></param>
        public void FileDownload(User user, int no)
        {
            int level = GetLevel(user);
            if (GetLevel(user) < 1)
            {
                user.ToChatMessage("해당 그룹에서 파일을 다운로드할 권한이 없습니다.", ChatType.Notice);
                return;
            }
            if (user.HaveItem(no))
            {
                NetworkMessageList.TipMessage(user.socket, "같은 파일이 이미 인벤토리에 존재합니다.");
                return;
            }
            if (!HaveItem(no))
            {
                NetworkMessageList.TipMessage(user.socket, "요청하신 파일이 이 그룹에 존재하지 않습니다. (이미 삭제되었을 가능성이 있습니다)");
                return;
            }
            bool result = user.AddFileItem(no);
            if (result)
            {
                NetworkMessageList.TipMessage(user.socket, "인벤토리에 파일이 복사되었습니다.");
            }
            else
            {
                NetworkMessageList.TipMessage(user.socket, "예기치 못한 오류로 인해 파일을 인벤토리에 복사하지 못했습니다.");
            }
        }
        /// <summary>
        /// 마스터가 특정 유저의 권한을 변경하는 경우 이 함수를 실행합니다.
        /// </summary>
        /// <param name="user">변경을 시도하는 주체입니다.(권한이 있어야함)</param>
        /// <param name="json">변경 내용이 담긴 JObject입니다 ( (string)id, (bool)positive - 수락여부)</param>
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
            int old_level = GetLevel((string)json["id"]);; // 이전의 레벨을 기억한다.
            if ((bool)json["positive"]) // 가입 요청 수락
            {
                node = new MysqlNode(private_data.mysqlOption, "UPDATE takes_course SET level=?level WHERE course_no=?group_name AND student_id=?id AND level=?old_level");
                node["old_level"] = 0;
                node["level"] = 1;
            }
            else
            {
                node = new MysqlNode(private_data.mysqlOption, "DELETE FROM takes_course WHERE course_no=?group_name AND student_id=?id");
            }
            node["group_name"] = key;
            node["id"] = (string)json["id"];
            int result = node.ExecuteNonQuery(); // MYSQL 실행
            if (result > 0) // 성공한경우
            {
                OpenMenu(user, "Member"); // 변경 주체의 윈도우를 갱신시킨다. (새로고침)
                NetworkMessageList.TipMessage(user.socket, "요청이 정상적으로 처리되었습니다.");
                if ((bool)json["positive"]) // 긍정적인 요청
                {
                    PostSystem.SendPost("그룹에 가입되었습니다.", "스터디 그룹 [" + name + "] 에 가입되었습니다!\r\n\r\n해당 그룹에서 많은 활동 부탁드립니다.", master, (string)json["id"]);
                }
                else // 부정적인 요청 (탈퇴)
                {
                    if (old_level == 1) // 이전에 해당 유저가 회원이었을 경우
                        PostSystem.SendPost("그룹에서 강퇴되었습니다.", "스터디 그룹 [" + name + "] 에서 강퇴되었음을 알립니다.", master, (string)json["id"]);
                    else // 이전에 해당 유저가 회원이 아니었던 경우
                        PostSystem.SendPost("그룹 가입 신청이 거절되었습니다.", "스터디 그룹 [" + name + "] 에서 가입 신청이 거절됨을 알립니다.", master, (string)json["id"]);
                }
            }
            else
            {
                NetworkMessageList.TipMessage(user.socket, "오류로 인해 처리되지 않았습니다.");
            }
        }
    }
}

