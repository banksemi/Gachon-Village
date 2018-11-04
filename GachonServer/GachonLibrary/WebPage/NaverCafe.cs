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
    public class NaverCafe : Site
    {
        public NaverCafe(string ID) : base(ID, "Naver")
        {
        }

        public override List<PostItem> GetList(GachonUser guser, BoardType board)
        {
            List<PostItem> result = new List<PostItem>();
            HtmlDocument dom = guser.VisitPage(board.url, Encoding.Default);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[contains(@class,'article-board')]//div[contains(@class,'inner_list')]");
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

        public override void GetPage(GachonUser guserm, PostItem item)
        {
        }

        public override void SearchMenu(GachonUser guser)
        {
            Uri baseuri = new Uri("https://cafe.naver.com/" + ID);
            HtmlDocument dom = guser.VisitPage(baseuri, Encoding.Default);         
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//ul[@class='cafe-menu-list']//li");
            foreach (HtmlNode node in sets)
            {
                string menu_title = ParseSupport.StringFromHtml(node.InnerText);
                if (node.ChildNodes["a"] == null) continue;
                string href = node.ChildNodes["a"].Attributes["href"].Value;
                if (menu_title.IndexOf("공모전") >= 0 || menu_title.IndexOf("공지") >= 0)
                {
                    NewBoard(BoardType.PostType.Notice, menu_title, baseuri, href);
                }
            }
        }
    }
}
