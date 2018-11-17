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
        public NaverCafe(string ID) : base(ID, "Navercafe")
        {
        }

        public override List<PostItem> GetList(GachonUser guser, BoardType board)
        {
            List<PostItem> result = new List<PostItem>();
            HtmlDocument dom = guser.VisitPage(board.url, Encoding.Default);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[@id='main-area']//tr");
            //timer.Print();
            if (sets != null)
            {
                foreach (HtmlNode node in sets)
                {
                    if (node.InnerText.IndexOf("No registered post") < 0 && node.InnerText.IndexOf("등록된 게시글이 없습니다") < 0 && node.ChildNodes["th"] == null)
                    {
                        HtmlNodeCollection datas = node.SelectNodes("./td");
                        HtmlNodeCollection TitleDivs = datas[0].SelectNodes("./div");
                        
                        if ((datas.Count ==5) &&(!TitleDivs[0].InnerText.Equals("공지")))
                        {
                            TitleDivs[1] = TitleDivs[1].SelectSingleNode(".//a");
                            datas[1] = datas[1].SelectSingleNode(".//a");
                            string url = ParseSupport.StringFromHtml(TitleDivs[1].Attributes["href"].Value);
                      
                            result.Insert(0, new PostItem(board.type, 
                                this, 
                                url, 
                                Int32.Parse(TitleDivs[0].InnerText),
                                ParseSupport.StringFromHtml(TitleDivs[1].InnerText).Trim(),
                                datas[1].InnerText.Trim(),
                                DateTime.Parse(datas[2].InnerText),
                                board.name));
                        }
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
