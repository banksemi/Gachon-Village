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
        public string name;
        public int LastNo = 0;
        public BoardType(PostType type, string name, string url)
        {
            this.type = type;
            this.name = name;
            this.url = url;
        }
        public BoardType(PostType type, string name, Uri uri)
        {
            this.type = type;
            this.name = name;
            this.url = uri.AbsoluteUri;
        }
    }
}
