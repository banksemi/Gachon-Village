using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    class NaverCafe : Site
    {
        public NaverCafe(string ID, string Type) : base(ID, Type)
        {
        }

        public override List<PostItem> GetList(GachonUser guser, BoardType board)
        {
            return null;
        }

        public override void GetPage(GachonUser guserm, PostItem item)
        {
        }

        public override void SearchMenu(GachonUser guser)
        {
        }
    }
}
