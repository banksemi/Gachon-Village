using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class PostItem
    {
        public BoardType.PostType posttype;
        public Site source;
        public int no;
        public string board_name;
        public string Title;
        public string Publisher;
        public string Content = null;
        public DateTime time;
        public string url;
        public DateTime s_time;
        public DateTime e_time;
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
