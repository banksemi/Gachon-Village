using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    public static class NetworkProtocol
    {
        public const int TipMessage = 1;
        public const int Login = 2;
        public const int SetInfo = 3;
        public const int EnterWorld = 4;
        public const int NewObject = 5;
        public const int RemoveObject = 6;
        public const int Move = 7;
        public const int Chat = 8;
        public const int PostAlarm = 9;
        public const int Action = 10;
        public const int Post_Open = 11;
        public const int Post_Detail = 12;
        public const int NewStudy = 13;
        public const int CloseNewStudy = 14;
        public const int Keyword_Open = 15;
        public const int Keyword_Add = 16;
        public const int Keyword_Remove = 17;
        public const int Inventory_Add = 18;
        public const int Inventory_Remove = 19;
        public const int File_Download = 20;
        public const int Study_SignUp = 21;
        public const int Study_UI = 22;
        public const int Study_Member_Request = 23;
        public const int Study_SaveChatting = 24;
        public const int Study_FileUpload = 25;
        public const int Study_FileDownload = 26;
    }
}
