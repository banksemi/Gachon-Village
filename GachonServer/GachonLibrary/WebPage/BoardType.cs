using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class BoardType
    {
        public enum PostType
        {
            Notice, Lecture, Homework, ETC
        }
        public PostType type;
        public string url;
        public int LastNo = 0;
        public BoardType(PostType type, string url)
        {
            this.type = type;
            this.url = url;
        }
        public BoardType(PostType type, Uri uri)
        {
            this.type = type;
            this.url = uri.AbsoluteUri;
        }
    }
}
