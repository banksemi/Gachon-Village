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
        /// <summary>
        /// 유저에게 새로운 홈워크 정보를 알립니다.
        /// </summary>
        public static void AddHomework(ESocket socket, string course_name, string title, DateTime end)
        {
            JObject item = new JObject();
            item["type"] = NetworkProtocol.Homework_Add;
            item["title"] = "[" + course_name + "] " + title;
            item["date"] = end;
            socket.Send(item);
        }
    }
}
