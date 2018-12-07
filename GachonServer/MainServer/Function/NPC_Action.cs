using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQL_Library;
namespace MainServer
{
    static partial class Function
    {
        /// <summary>
        /// 해당 NPC가 유저에게 상호작용의 결과를 반환합니다.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="user"></param>
        public static void NPC_Action(NPC npc, User user)
        {
            //user.ToChatMessage(npc.function + "를 선택", ChatType.System);
            if (npc.function == "우편함 열기")
            {
                // 해당 유저에게 우편함 첫번째 페이지 전송
                PostSystem.GetPage(user, 1);
            }
            if (npc.function == "키워드 알림 설정")
            {
                // 해당 유저에게 모든 키워드 목록 전송
                KeywordSystem.GetList(user);
            }
            if (npc.function == "그룹 상호작용")
            {
                // 해당 유저에게 OpenMenu 실행
                Study StudyGroup = (Study)npc;
                StudyGroup.OpenMenu(user);
            }
        }
    }
}
