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

        public override void PageSearch(GachonUser guser)
        {
            throw new NotImplementedException();
        }
    }
}
