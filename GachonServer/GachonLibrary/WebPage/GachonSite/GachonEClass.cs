using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace GachonLibrary
{
    class GachonEClass : GachonSite
    {
        public GachonEClass(string ID) : base(ID, "EClass", "http://eclass.gachon.ac.kr/")
        {
        }
    }
}
