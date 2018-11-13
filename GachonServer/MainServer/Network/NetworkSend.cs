using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    public static class NetworkSend
    {
        public static void SendAllUser(JObject json, User ignoreuser = null)
        {
            foreach(User user in User.Items.Values.ToList())
            {
                if (ignoreuser != user)
                {
                    user.socket.Send(json);
                }
            }
        }
    }
}
