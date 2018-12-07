using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{/// <summary>
/// 메뉴의 종류를 나타냅니다.
/// 각 메뉴마다 마지막으로 읽은 번호를 저장합니다.
/// </summary>
    public class BoardType
    {
        /// <summary>
        /// 공지사항(1), 강의자료실(2), 레포트제출실(3), 그 외(4)
        /// </summary>
        public enum PostType
        {
            Notice =1, Lecture=2, Homework=3, ETC=4
        }
        public PostType type;
        public string url;
        public string name;
        /// <summary>
        /// 마지막으로 읽은 게시글의 번호
        /// </summary>
        public int LastNo = 0;       
        public BoardType(PostType type, string name, string url)
        {
            this.type = type;
            this.name = name;
            this.url = url;
        }
        /// <summary>
        /// uri이 절대경로가 아닐 경우 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="uri"></param>
        public BoardType(PostType type, string name, Uri uri)
        {
            this.type = type;
            this.name = name;
            this.url = uri.AbsoluteUri;
        }
    }
}
