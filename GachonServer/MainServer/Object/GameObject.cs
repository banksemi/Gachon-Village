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
    public class Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public class GameObject
    {
        public static Dictionary<int, GameObject> Items = new Dictionary<int, GameObject>();
        public bool isStart = false;
        public Vector3 position = new Vector3(0, 0, 0);
        public int no = 0;
        private static int no_count = 0;
        public string name;
        private Object No_Lock = new object();
        private List<Vector3> movelist = new List<Vector3>();
        private const int MaxmoveSize = 10;
        public GameObject()
        {
            lock(No_Lock)
            {
                no = no_count++;
            }
        }
        public void Move(Vector3 vector)
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
            // 이동데이터 한번 앞으로
            lock (movelist)
            {
                if (movelist.Count > 0)
                {
                    position = movelist[0];
                    movelist.RemoveAt(0);
                    JObject json = new JObject();
                    json["type"] = NetworkProtocol.Move;
                    json["no"] = no;
                    json["x"] = position.x;
                    json["y"] = position.y;
                    json["z"] = position.z;
                    if (this is User)
                        NetworkSend.SendAllUser(json, (User)this);
                    else
                        NetworkSend.SendAllUser(json);
                }
            }
        }
        public JObject InfoData()
        {
            JObject json = new JObject();
            json["name"] = name;
            json["no"] = no;
            json["x"] = position.x;
            json["y"] = position.y;
            json["z"] = position.z;
            return json;
        }
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
        public void Update()
        {
            MoveUnit();
        }
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
    }
}
