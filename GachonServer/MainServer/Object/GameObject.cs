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
    public class GameObject
    {
        public static Dictionary<int, GameObject> Items = new Dictionary<int, GameObject>();
        public bool isStart = false;
        public float x;
        public float y;
        public float z;
        public int no = 0;
        private static int no_count = 0;
        public string name;
        private Object No_Lock = new object();
        public GameObject()
        {
            lock(No_Lock)
            {
                no = no_count++;
            }
        }
        public JObject InfoData()
        {
            JObject json = new JObject();
            json["name"] = name;
            json["no"] = no;
            json["x"] = x;
            json["y"] = y;
            json["z"] = z;
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
