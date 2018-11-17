using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    class NPC : GameObject
    {
        int message_delay = 0;
        List<string> message_list = new List<string>();
        static Random rd = new Random();
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
        public void AddMessage(string message)
        {
            message_list.Add(message);
        }
    }
}
