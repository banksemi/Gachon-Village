using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{/// <summary>
/// 가천대학교 카페 (GachonSite 상속)
/// </summary>
    public class GachonCafe : GachonSite
    {
        public GachonCafe(string ID) : base(ID, "GCafe", "http://cafe.gachon.ac.kr/")
        {
        }
    }
}
