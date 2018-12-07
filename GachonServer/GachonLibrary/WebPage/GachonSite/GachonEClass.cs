using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace GachonLibrary
{/// <summary>
/// 가천대학교 Eclass (GachontSite 상속)
/// </summary>
    class GachonEClass : GachonSite
    {
        public GachonEClass(string ID) : base(ID, "EClass", "http://eclass.gachon.ac.kr/")
        {
        }
    }
}
