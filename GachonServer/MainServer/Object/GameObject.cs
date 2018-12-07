using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using GachonLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    /// <summary>
    /// 이 객체는 서버에서 위치 정보를 가지고 움직일 수 있는 객체를 나타내는 클래스입니다.
    /// 실시간 동기화를 보장합니다.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// 서버에서 관리되고 있는 객체 리스트입니다.
        /// </summary>
        public static Dictionary<int, GameObject> Items = new Dictionary<int, GameObject>();
        /// <summary>
        /// 이 객체가 정상적으로 모든 로딩이 완료됬는지를 나타내는 변수입니다.
        /// </summary>
        public bool isStart { get; private set; }
        /// <summary>
        /// 이 객체가 가지고 있는 위치입니다.
        /// </summary>
        public Vector4 position = new Vector4(0, 0, 0, 0);
        /// <summary>
        /// 이 객체에 부여된 고유 번호입니다. (중복이 없음)
        /// </summary>
        public int no { get; private set; }
        private static int no_count = 0; // 고유번호를 부여하기 위한 Number 카운트
        private Object No_Lock = new object(); // 고유번호를 부여하기 위한 Lock 전용 오브젝트
        /// <summary>
        /// 이 객체의 이름입니다. 게임 내에서 오브젝트 위에 표시됩니다.
        /// </summary>
        public string name;
        /// <summary>
        /// 이 객체의 소속을 나타냅니다. 여기에 입력된 내용이 오브젝트 이름 옆에 표시됩니다.
        /// </summary>
        public string group;
        private List<Vector4> movelist = new List<Vector4>();
        private const int MaxmoveSize = 10; // 움직임 동기화루틴에서 최대 움직임 저장 개수 (넘어갈경우 자동으로 경로 압축)
        /// <summary>
        /// 이 객체가 게임 내에서 보여질 그래픽입니다.
        /// </summary>
        public string skin = "PostBox";
        public GameObject()
        {
            lock(No_Lock)
            {
                no = no_count++;
                isStart = false;
            }
        }
        /// <summary>
        /// 특정 위치로 움직인다. 상대방 클라이언트에서는 0.1초동안 연속적으로 움직이는것처럼 보인다.
        /// </summary>
        /// <param name="vector"></param>
        public void Move(Vector4 vector)
        {
            lock (movelist)
            {
                // 이동 Queue에 움직일 데이터가 최대로 쌓여있다면 - 즉 클라이언트에서 한번에 많은 이동정보를 준경우
                if (movelist.Count == MaxmoveSize)
                {
                    movelist.RemoveAt(0); // 앞부분을 이동 정보를 합친다.
                }
                movelist.Add(vector);
            }
        }
        private void MoveUnit()
        {
            // 이동데이터 한번 앞으로, 모든 유저에게 움직인 정보를 전송
            lock (movelist)
            {
                if (movelist.Count > 0)
                {
                    // 다음 위치 꺼내오기
                    position = movelist[0];
                    movelist.RemoveAt(0);
                    // 위치정보를 담은 JObject 만들기
                    JObject json = new JObject();
                    json["type"] = NetworkProtocol.Move;
                    json["no"] = no;
                    json["x"] = position.x;
                    json["y"] = position.y;
                    json["z"] = position.z;
                    json["q"] = position.q;
                    // 이 클래스가 User 클래스의 부모 클래스일경우
                    if (this is User)
                        NetworkSend.SendAllUser(json, (User)this); // 메세지를 전송하는데, 자기 자신(유저)에게는 전송하지 않는다.
                    else
                        NetworkSend.SendAllUser(json); // 모두에게 메세지를 전송한다.
                }
            }
        }
        /// <summary>
        /// 이 객체의 정보를 모두 담아서 JObject 형태로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public virtual JObject InfoData()
        {
            JObject json = new JObject();
            json["name"] = name;
            json["group"] = group;
            json["no"] = no;
            json["skin"] = skin;
            json["x"] = position.x;
            json["y"] = position.y;
            json["z"] = position.z;
            json["q"] = position.q;
            return json;
        }
        /// <summary>
        /// 이 객체가 로딩이 끝났음을 확인하고 모두와 통신을 시작합니다.
        /// </summary>
        public virtual void Start()
        {
            isStart = true;
            JObject json = InfoData();
            json["type"] = NetworkProtocol.NewObject;
            // 다른 사람들에게 시작했다는것을 알려줌.
            //json["type"]
            foreach (User user in User.Items.Values)
            {
                if (user.isStart) // 월드에 들어와있는 유저들에게만
                {
                    user.socket.Send(json);
                }
            }
            Items.Add(no, this);
        }
        public virtual void Update()
        {
            MoveUnit();
        }
        /// <summary>
        /// 이 객체를 서버에서 관리되는 오브젝트 목록에서 제거합니다. (Dispose)
        /// </summary>
        public void Remove()
        {
            if (isStart == true)
            {
                JObject json = InfoData();
                json["type"] = NetworkProtocol.RemoveObject;
                foreach (User user in User.Items.Values)
                {
                    if (user.isStart) // 월드에 들어와있는 유저들에게만
                    {
                        user.socket.Send(json);
                    }
                }
                Items.Remove(no);
            }
        }
        /// <summary>
        /// 이 객체가 모든 유저에게 채팅 메세지를 보냅니다.
        /// </summary>
        /// <param name="message">전송할 메세지</param>
        /// <param name="Type">ChatType 클래스에 정의된 타입 입력</param>
        public virtual void ChatMessage(string message, int Type)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["chattype"] = Type;
            json["message"] = message;
            json["no"] = no; // 보낸사람의 고유번호
            json["sender"] = name; // 보낸사람 이름
            NetworkSend.SendAllUser(json);
        }
    }
}
