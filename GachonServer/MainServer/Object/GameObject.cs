﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using GachonLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    public class Vector4
    {
        public float x;
        public float y;
        public float z;
        public float q;
        public Vector4(float x, float y, float z, float q)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.q = q;
        }
    }
    public class GameObject
    {
        public static Dictionary<int, GameObject> Items = new Dictionary<int, GameObject>();
        public bool isStart = false;
        public Vector4 position = new Vector4(0, 0, 0, 0);
        public int no = 0;
        private static int no_count = 0;
        public string name;
        public string group;
        private Object No_Lock = new object();
        private List<Vector4> movelist = new List<Vector4>();
        private const int MaxmoveSize = 10;
        public string skin = "PostBox";
        public GameObject()
        {
            lock(No_Lock)
            {
                no = no_count++;
            }
        }
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
                    json["q"] = position.q;
                    if (this is User)
                        NetworkSend.SendAllUser(json, (User)this);
                    else
                        NetworkSend.SendAllUser(json);
                }
            }
        }
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
        /// 이 객체가 대화를 시작합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Type"></param>
        public void ChatMessage(string message, int Type)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.Chat;
            json["chattype"] = Type;
            json["no"] = no; // 보낸사람의 고유번호
            json["sender"] = name; // 보낸사람 이름
            json["message"] = message;
            NetworkSend.SendAllUser(json);
        }
    }
}
