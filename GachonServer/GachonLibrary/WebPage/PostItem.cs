using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class PostItem
    {
        /// <summary>
        /// 공지사항, 강의자료실, 레포트 제출실 등 게시글의 유형을 의미
        /// </summary>
        public BoardType.PostType posttype;
        /// <summary>
        /// Eclass, Cyber Campus, Naver cafe등 사이트의 유형
        /// </summary>
        public Site source;
        /// <summary>
        /// 게시글의 번호
        /// </summary>
        public int no;
        /// <summary>
        /// 메뉴의 이름
        /// </summary>
        public string board_name;
        /// <summary>
        /// 게시글 제목
        /// </summary>
        public string Title;
        /// <summary>
        /// 게시글을 올린 사람
        /// </summary>
        public string Publisher;
        /// <summary>
        /// 게시글 내용
        /// </summary>
        public string Content = null;
        /// <summary>
        /// 게시글 올린 시각
        /// </summary>
        public DateTime time;
        /// <summary>
        /// 게시글의 주소
        /// </summary>
        public string url;
        /// <summary>
        /// 레포트 게시글 올린 날짜
        /// </summary>
        public DateTime s_time;
        /// <summary>
        /// 레포트 게시글 마감 날짜
        /// </summary>
        public DateTime e_time;
        /// <summary>
        /// 레포트 게시글 제출 인원
        /// </summary>
        public int num_submitted;
        public PostItem(BoardType.PostType postType, string board_name)
        {
            this.posttype = postType;
            this.board_name = board_name;
        }
        public PostItem(BoardType board)
        {
            this.posttype = board.type;
            this.board_name = board.name;
        }
        /// <summary>
        /// 게시글 내용을 30자 이내로 줄임 (미리보기)
        /// </summary>
        public string Content_Briefly
        {
            get
            {
                if (Content == null) return "Not Content";
                if (Content.Length < 30) return Content;
                else return Content.Substring(0, 30) + " . . .";
            }
        }
        public override string ToString()
        {
            return  Title + "(" + Publisher  +", " + time.ToShortDateString() + ")";
        }
    }
}
