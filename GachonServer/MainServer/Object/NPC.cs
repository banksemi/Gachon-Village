using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    class NPC : GameObject
    {
        /// <summary>
        /// 이 객체가 가진 고유 기능입니다. 유저는 상호작용키를 통해 이 기능에 접근할 수 있습니다(Function.NPC_Action 함수 참조)
        /// </summary>
        public string function;
        private int message_delay = 0;
        private List<string> message_list = new List<string>();
        private static Random rd = new Random();
        public override void Update()
        {
            message_delay--;
            if (message_delay < 0)
            {
                message_delay = rd.Next(100, 300);
                if (message_list.Count > 0)
                {
                    ChatMessage(message_list[rd.Next(0, message_list.Count)], ChatType.NPC);
                }
            }
        }
        /// <summary>
        /// 이 객체의 정보를 JObject 형식으로 반환합니다. 이 클래스는 function 정보도 포함되어있습니다.
        /// </summary>
        /// <returns></returns>
        public override JObject InfoData()
        {
            JObject json = base.InfoData();
            json["function"] = function;
            return json;
        }
        /// <summary>
        /// 이 NPC의 대사를 추가합니다. 대사는 일정 시간 간격으로 모두에게 표시됩니다.
        /// </summary>
        /// <param name="message">대사</param>
        public void AddMessage(string message)
        {
            message_list.Add(message);
        }
    }
}
