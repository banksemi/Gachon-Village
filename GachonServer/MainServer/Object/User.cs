using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GachonLibrary;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
using SQL_Library;
using NetworkLibrary.File;
namespace MainServer
{
    public class User : GameObject
    {
        public static Dictionary<ESocket, User> Items = new Dictionary<ESocket, User>();
        public GachonUser gachonAccount { get; private set; }
        public ESocket socket { get; private set; }
        public string ID => gachonAccount.ID;
        public User(ESocket socket, GachonUser user)
        {
            lock(Items)
            {
                foreach(User item in Items.Values)
                {
                    if (item.gachonAccount == user) throw new DuplicationError("중복 로그인");
                }
                this.socket = socket;
                gachonAccount = user;
                Items.Add(socket, this);
                name = gachonAccount.Name;
                group = gachonAccount.StudentNumber.Substring(2, 2);
                skin = "Eve";
            }
        }
        public void Dispose()
        {
            Remove();
            lock (Items)
            {
                Items.Remove(socket);
            }
        }
        public override void Start()
        {
            base.Start();
            // 해당 유저에게도 월드에 있는 다른 오브젝트 표시
            foreach(GameObject item in GameObject.Items.Values)
            {
                if (item != this)
                {
                    JObject json = item.InfoData();
                    json["type"] = NetworkProtocol.NewObject;
                    socket.Send(json);
                }
            }
            // 인벤토리 로드
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM inventory_items WHERE student_id=?id");
            node["id"] = ID;
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    JObject item = new JObject();
                    item["type"] = NetworkProtocol.Inventory_Add;
                    item["no"] = node.GetInt("file_no");
                    item["size"] = node.GetInt("size");
                    item["name"] = node.GetString("name");
                    item["date"] = node.GetDateTime("date");
                    item["owner"] = node.GetString("owner");
                    socket.Send(item);
                }
            }
            NetworkMessageList.TipMessage(socket, "가천 빌리지에 오신것을 환영합니다!");
            ToChatMessage("가천 빌리지에 오신것을 환영합니다!", ChatType.Notice);
            ToChatMessage("[컴퓨터 네트워크] 과목에 새로운 게시글이 등록되었습니다.", ChatType.System);
            ToChatMessage("이승화 : 귓속말 테스트~~~", ChatType.Whisper);
            ToChatMessage("위의 2개의 메세지는 디버그를 위해 출력되었습니다.", ChatType.Notice);
            int NewMessage = PostSystem.GetNewMessageCount(ID);
            if (NewMessage > 0)
            {
                ToChatMessage("[우편함] " + NewMessage + "개의 읽지 않은 우편이 존재합니다!", ChatType.System);
            }
        }
        /// <summary>
        /// 이 유저에게 채팅메세지를 전달합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Type"></param>
        public void ToChatMessage(string message, int Type)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["chattype"] = Type;
            json["message"] = message;
            socket.Send(json);
        }
        public bool HaveItem(int no)
        {
            // 해당 번호의 파일이 실제로 있는지 확인 + 파일 정보 불러오기
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT 'true' FROM inventory WHERE student_id=?id AND file_no=?no");
            node["id"] = ID;
            node["no"] = no;
            using (node.ExecuteReader())
            {
                if (node.Read()) return true;
            }
            return false;
        }
        public bool AddFileItem(int no)
        {
            // 해당 번호의 파일이 실제로 있는지 확인 + 파일 정보 불러오기
            JObject item = FileSystem.GetFileItem(no);
            if (item == null) return false;
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO inventory(student_id, file_no) VALUES (?id, ?no)");
            node["id"] = ID;
            node["no"] = no;
            if (node.ExecuteInsertQuery() < 0) return false;
            socket.Send(item);
            return true;
        }
        public bool RemoveItem(int no)
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "DELETE FROM inventory WHERE student_id=?id and file_no=?no");
            node["id"] = ID;
            node["no"] = no;
            if (node.ExecuteNonQuery() > 0)
            {
                JObject json = new JObject();
                json["type"] = NetworkProtocol.Inventory_Remove;
                json["no"] = no;
                socket.Send(json);
                return true;
            }
            else
                return false;
        }

        public void DownloadItem(int no, string user_path)
        {
            // 인벤토리에 해당 파일이 존재하는지 확인
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM inventory WHERE student_id=?id AND file_no=?no");
            node["id"] = ID;
            node["no"] = no;
            using (node.ExecuteReader())
            {
                if (!node.Read())
                {
                    ToChatMessage("해당 아이템에 대한 권한이 없습니다.", ChatType.Notice);
                }
            }
            // 파일 정보 불러오기
            node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM file where file_no=?no");
            node["no"] = no;
            using (node.ExecuteReader())
            {
                if (node.Read())
                {
                    NServerFile file = new NServerFile(socket,node.GetString("path"));
                    JObject json = new JObject();
                    json["path"] = user_path;
                    socket.SendFile(json, file);
                }
            }

            // 인벤토리에 아이템이 있는지 확인
        }

        public override void Update()
        {
            base.Update();
        }
        public override void ChatMessage(string message, int Type)
        {
            //Dice
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;

            if (message.IndexOf("/주사위") == 0)
            {
                Random rd = new Random();
                json["chattype"] = ChatType.Notice;
                json["message"] = name + "(이)가 주사위를 굴렸습니다~!! 주사위가 " + rd.Next(1, 6).ToString() + " 나왔습니다!";
                NetworkSend.SendAllUser(json);
                return;
            }

            //Whisper
            if ((message.IndexOf("/ㅈ ") == 0) || (message.IndexOf("/w ") == 0) || (message.IndexOf("/귓속말 ") == 0))
            {
                string[] Receiver = message.Split(' ');
                //형식이 맞는지 확인
                if (Receiver.Length > 2)
                {
                    for (int i = 3; i < Receiver.Length; i++)
                    {
                        Receiver[2] += " " + Receiver[i];
                    }
                    string receiverID = GachonUser.GetID(Receiver[1]); //Sender가 입력한 이름을 통해 귓속말 대상 ID를 얻음

                    //대상이 접속해있는지 아닌지 확인
                    foreach (User user in User.Items.Values.ToList())
                    {
                        if (user.ID.Equals(receiverID))
                        {
                            json["chattype"] = ChatType.Whisper;
                            json["message"] = Receiver[2]; //메세지 내용
                            json["no"] = no;
                            json["sender"] = name;
                            user.socket.Send(json);

                            json["group"] = "To " + Receiver[1];
                            //json["sender"] = "["+Receiver[1]+"]"+ name; //Sender에게 [Reiever]Sender: Content 로 보이도록
                            this.socket.Send(json);
                            return;
                        }
                    }
                    json["message"] = "[귓속말] 현재 접속해있지 않는 사용자입니다.";
                }
                else
                {
                    json["message"] = "[귓속말] 잘못된 귓속말 형식 입니다. '/w 대상 내용' 으로 입력해 주세요.";
                }
                json["chattype"] = ChatType.System;
                socket.Send(json);
                return;
            }
            // 해당 영역이 그룹의 영역일때
            string ingroup = InGroup();
            if (ingroup != null)
            {
                List<string> idlist = Study.Items[ingroup].GroupUsers();
                if (idlist.Contains(ID))
                {
                    json["chattype"] = ChatType.Group;
                    json["message"] = message;
                    json["no"] = no;
                    json["sender"] = name;
                    json["group"] = ingroup;
                    foreach (User user in User.Items.Values)
                    {
                        if (idlist.Contains(user.ID))
                        {
                            user.socket.Send(json);
                        }
                    }
                    MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO group_chat(group_name, student_id, data) VALUES (?group, ?id, ?data)");
                    node["group"] = ingroup;
                    node["id"] = ID;
                    node["data"] = message;
                    node.ExecuteInsertQuery();
                }
                else
                {
                    base.ChatMessage(message, Type);
                }
            }
            else
            {
                base.ChatMessage(message, Type);
            }
        }
        /// <summary>
        /// 이 유저가 서있는 위치가 다른 그룹에 포함되는지 반환합니다.
        /// 인풋값에 숫자를 입력하면 그 거리를 기준으로 판단합니다.
        /// </summary>
        /// <returns></returns>
        public string InGroup(float check_d = 17.5f)
        {
            MysqlNode snode = new MysqlNode(private_data.mysqlOption, "SELECT group_name, x, z FROM `group`");
            using (snode.ExecuteReader())
            {
                while (snode.Read())
                {
                    float dx = position.x - snode.GetFloat("x");
                    float dz = position.z - snode.GetFloat("z");
                    float distance = (float)Math.Sqrt(dx * dx + dz * dz);
                    if (distance <= check_d)
                    {
                        return snode.GetString("group_name");
                    }
                }
            }
            return null;
        }
    }
}
