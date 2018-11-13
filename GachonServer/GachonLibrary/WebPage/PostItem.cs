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
        public DateTime s_time;
        public DateTime e_time;
        public int num_submitted;
        public PostItem(BoardType.PostType postType)
        {
            this.posttype = postType;
        }
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

        public PostItem(BoardType.PostType posttype, Site source, string url, int no, string title, DateTime s_time, DateTime e_time, int num_submitted)
        {
            this.url = url;
            this.posttype = posttype;
            this.source = source;
            this.no = no;
            this.Title = title;
            this.s_time = s_time;
            this.e_time = e_time;
            this.num_submitted = num_submitted;
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
