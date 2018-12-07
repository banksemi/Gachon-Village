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
        /// <summary>
        /// 현재 게임에 접속중인 모든 유저들의 리스트입니다. 자동으로 추가되고 삭제됩니다.
        /// </summary>
        public static new Dictionary<ESocket, User> Items = new Dictionary<ESocket, User>();
        /// <summary>
        /// 이 유저와 연결된 가천대 세션입니다.
        /// </summary>
        public GachonUser gachonAccount { get; private set; }
        /// <summary>
        /// 이 유저와 연결된 Socket입니다.
        /// </summary>
        public ESocket socket { get; private set; }
        /// <summary>
        /// 가천대 ID, 즉 로그인 ID를 반환합니다.
        /// </summary>
        public string ID => gachonAccount.ID;
        public User(ESocket socket, GachonUser user)
        {
            // 쓰레드 충돌을 막기위한 lock
            lock(Items)
            {
                // 이미 같은 아이디로 로그인한 유저가 있으면 에러 발생
                foreach(User item in Items.Values)
                {
                    if (item.gachonAccount == user) throw new DuplicationError("중복 로그인");
                }
                this.socket = socket;
                gachonAccount = user;
                Items.Add(socket, this);
                name = gachonAccount.Name;
                group = gachonAccount.StudentNumber.Substring(2, 2); // 이름 앞에 뜨는 그룹을 학번으로 표시 ( 201735861 -> 17)
                skin = "Eve"; // 기본 스킨
            }
        }
        /// <summary>
        /// 이 유저가 로그아웃을때 실행되는 함수입니다. 서버에서 관리하는 객체 목록에서 이 객체를 삭제합니다.
        /// 그러나 GachonUser 는 알림 서비스를 위해 로그인 상태를 유지합니다.
        /// </summary>
        public void Dispose()
        {
            Remove();
            lock (Items)
            {
                Items.Remove(socket);
            }
        }
        /// <summary>
        /// 플레이어가 로딩이 끝났을때 다른 플레이어에게 플레이어 입장 소식을 알리고, 이 플레이어에게 정보를 전달합니다.
        /// </summary>
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
        /// <param name="Type">ChatType 객체</param>
        public void ToChatMessage(string message, int Type)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["chattype"] = Type;
            json["message"] = message;
            socket.Send(json);
        }
        /// <summary>
        /// 이 유저의 인벤토리에 파일이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="no">MYSQL에 등록된 파일 번호입니다.</param>
        /// <returns></returns>
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
        /// <summary>
        /// 이 유저의 인벤토리에 새로운 아이템(파일)을 추가합니다. 
        /// </summary>
        /// <param name="no">MYSQL에 등록된 파일 번호입니다</param>
        /// <returns></returns>
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
        /// <summary>
        /// 이 유저의 인벤토리에서 아이템을 제거합니다.
        /// </summary>
        /// <param name="no">MYSQL에 등록된 파일 번호입니다</param>
        /// <returns></returns>
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
        /// <summary>
        /// 인벤토리에 있는 아이템을 바탕으로 해당 클라이언트가 강제로 파일을 다운로드 받도록 만듭니다.
        /// </summary>
        /// <param name="no">MYSQL에 등록된 파일 번호입니다</param>
        /// <param name="user_path">해당 클라이언트에 저장될 경로입니다.</param>
        public bool DownloadItem(int no, string user_path)
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
                    return false;
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
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// 해당 유저가 채팅을 시작합니다. 이 함수에서 명령어 구문 분석이 실행됩니다.
        /// </summary>
        /// <param name="message">입력 메세지</param>
        /// <param name="Type">입력 메세지 타입 (ChatType)</param>
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
            if ((message.IndexOf("/ㅈ") == 0) || (message.IndexOf("/w") == 0) || (message.IndexOf("/귓속말") == 0))
            {
                string[] Receiver = message.Split(' ');
                //형식이 맞는지 확인
                if (Receiver.Length > 2)
                {
                    for (int i = 3; i < Receiver.Length; i++)
                    {
                        Receiver[2] += " " + Receiver[i];
                    }

                    //대상이 접속해있는지 아닌지 확인
                    foreach (User user in User.Items.Values.ToList())
                    {
                        if (user.name.Equals(Receiver[1]))
                        {
                            json["chattype"] = ChatType.Whisper;
                            json["message"] = Receiver[2]; //메세지 내용
                            json["no"] = no;
                            json["sender"] = name;
                            user.socket.Send(json);
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
                List<string> idlist = Study.Items[ingroup].Users;
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
