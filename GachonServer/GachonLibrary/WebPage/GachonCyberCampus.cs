using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSupport;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
namespace GachonLibrary
{
    class GachonCyberCampus : Site
    {
        public GachonCyberCampus(string ID) : base(ID, "Cyber")
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
            if (ID != "37984") return;
            Uri baseuri = new Uri("http://cyber.gachon.ac.kr/mod/ubboard/index.php?id=" + ID);
            HtmlDocument dom = guser.VisitPage(baseuri);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[contains(@class,'boards')]//tr/td[1]");
            foreach (HtmlNode node in sets)
            {
                string menu_title = ParseSupport.StringFromHtml(node.InnerText);
                if (node.ChildNodes["a"] == null) continue;
                string href = node.ChildNodes["a"].Attributes["href"].Value;
                if (menu_title.IndexOf("Announcements") >= 0 || menu_title.IndexOf("공지") >= 0)
                {
                    NewBoard(BoardType.PostType.Notice, menu_title, baseuri, href);
                }
            }
        }
    }
}
