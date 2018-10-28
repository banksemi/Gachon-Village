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
        public string Title;
        public string Publisher;
        public string Content = null;
        public DateTime time;
        public string url;
        public PostItem(BoardType.PostType posttype, Site source,string url, int no, string title, string publisher, DateTime time)
        {
            this.url = url;
            this.posttype = posttype;
            this.source = source;
            this.no = no;
            this.Title = title;
            this.Publisher = publisher;
            this.time = time;

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
