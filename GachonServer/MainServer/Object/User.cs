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

        public bool AddFileItem(int no)
        {
            // 해당 번호의 파일이 실제로 있는지 확인 + 파일 정보 불러오기
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT file.name, file.size, account.name as owner, date FROM file join account on file.owner=account.id where file_no=?no");
            node["no"] = no;
            JObject item = null;
            using (node.ExecuteReader())
            {
                if (node.Read())
                {
                    item = new JObject();
                    item["type"] = NetworkProtocol.Inventory_Add;
                    item["no"] = no;
                    item["size"] = node.GetInt("size");
                    item["name"] = node.GetString("name");
                    item["date"] = node.GetDateTime("date");
                    item["owner"] = node.GetString("owner");
                }
                else
                {
                    return false;
                }
            }
            node = new MysqlNode(private_data.mysqlOption, "INSERT INTO inventory(student_id, file_no) VALUES (?id, ?no)");
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
    }
}
