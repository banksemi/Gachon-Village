using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    class ChatType
    {
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 사용자는 주황색 메세지를 수신합니다.
        /// </summary>
        public const int Notice = 0;
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 사용자는 하얀색 메세지를 수신합니다.
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 사용자는 초록색 메세지를 수신합니다.
        /// </summary>
        public const int System = 2;
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 사용자는 보라색 메세지를 수신합니다.
        /// </summary>
        public const int Whisper = 3;
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 채팅창에는 메세지를 표시하진 않지만, 오브젝트의 말풍선으로 메세지를 표시합니다.
        /// </summary>
        public const int NPC = 4;
        /// <summary>
        /// 이 타입으로 메세지를 보낼경우 사용자는 파란색 메세지를 수신합니다.
        /// </summary>
        public const int Group = 5;
    }
}
