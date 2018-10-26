using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class GachonCafe : GachonSite
    {
        public GachonCafe(string ID) : base(ID, "GCafe", "http://cafe.gachon.ac.kr/")
        {
        }
    }
}
