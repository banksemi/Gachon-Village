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
            List<PostItem> result = new List<PostItem>();
            HtmlDocument dom = guser.VisitPage(board.url);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[@class='list']//table//tr");
            //timer.Print();
            if (sets != null)
            {
                foreach (HtmlNode node in sets)
                {
                    if (node.InnerText.IndexOf("No registered post") < 0 && node.InnerText.IndexOf("등록된 게시글이 없습니다") < 0 && node.ChildNodes["th"] == null)
                    {
                        HtmlNodeCollection datas = node.SelectNodes(".//td");
                        string url = ParseSupport.StringFromHtml(datas[1].ChildNodes["a"].Attributes["href"].Value);

                        result.Insert(0, new PostItem(board.type, this, url, Int32.Parse(datas[0].InnerText),
                            ParseSupport.StringFromHtml(datas[1].InnerText),
                            datas[2].InnerText,
                             DateTime.Parse(datas[3].InnerText)));
                    }
                }
            }
            return result;
        }

        public override void GetPage(GachonUser guser, PostItem post)
        {
            HtmlDocument dom = guser.VisitPage(post.url);
            post.Title = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//div[@class='subject']").InnerText);
            post.Content = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//div[@class='ubboard']//div[contains(@class,'text_to_html')]").InnerText);
        }

        public override void SearchMenu(GachonUser guser)
        {
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
