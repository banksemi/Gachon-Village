using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    /// <summary>
    /// 이 클래스는 소켓에 메세지를 캡슐화를 한것처럼 쉽게 전송할 수 있도록 지원합니다.
    /// </summary>
    static class NetworkMessageList
    {
        /// <summary>
        /// 소켓에 Tip 메세지 (안드로이드일 경우 토스트 메세지)를 전송합니다.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        public static void TipMessage(ESocket socket, string message)
        {
            JObject json = new JObject();
            json["type"] = NetworkProtocol.TipMessage;
            json["message"] = message;
            socket.Send(json);
        }
    }
}
