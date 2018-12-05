using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSupport;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
namespace GachonLibrary
{/// <summary>
/// 가천대학교 소프트웨어학과 네이버 카페
/// </summary>
    public class NaverCafe : Site
    {
        public NaverCafe(string ID) : base(ID, "Navercafe")
        {
        }

        /// <summary>
        /// (네이버 카페) 게시글의 목록을 가져옵니다.
        /// </summary>
        /// <param name="guser"></param>
        /// <param name="board"></param>
        /// <returns></returns>
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
                    //등록된 게시글이 존재하면
                    if (node.InnerText.IndexOf("No registered post") < 0 && node.InnerText.IndexOf("등록된 게시글이 없습니다") < 0 && node.ChildNodes["th"] == null)
                    {
                        HtmlNodeCollection datas = node.SelectNodes("./td");
                        HtmlNodeCollection TitleDivs = datas[0].SelectNodes("./div");
                        
                        //data Count가 5일 때 원하는 배열의 값이 순서대로 들어있음
                        if ((datas.Count ==5) &&(TitleDivs[0].InnerText.IndexOf("공지") == -1))
                        {
                            TitleDivs[1] = TitleDivs[1].SelectSingleNode(".//a");
                            datas[1] = datas[1].SelectSingleNode(".//a");
                            string url = ParseSupport.StringFromHtml(TitleDivs[1].Attributes["href"].Value);
                            //새로운 PostItem 생성
                            result.Insert(0, new PostItem(board)
                            {
                                url = url,
                                source = this,
                                no = Int32.Parse(TitleDivs[0].InnerText),
                                Title = ParseSupport.StringFromHtml(TitleDivs[1].InnerText).Trim(),
                                Publisher = datas[1].InnerText.Trim(),
                                time = DateTime.Parse(datas[2].InnerText),
                            });
                        }
                    }
                }
            }
            return result;
        }

        public override void GetPage(GachonUser guserm, PostItem item)
        {
        }

        /// <summary>
        /// (네이버 카페) 메뉴 목록을 읽어옵니다.
        /// </summary>
        /// <param name="guser"></param>
        public override void SearchMenu(GachonUser guser)
        {
            Uri baseuri = new Uri("https://cafe.naver.com/" + ID);
            HtmlDocument dom = guser.VisitPage(baseuri, Encoding.Default);         
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//ul[@class='cafe-menu-list']//li");
            #region 메뉴 목록중 공지사항 관련 메뉴를 얻어옵니다.
            foreach (HtmlNode node in sets)
            {
                string menu_title = ParseSupport.StringFromHtml(node.InnerText); //메뉴 이름
                if (node.ChildNodes["a"] == null) continue;
                string href = node.ChildNodes["a"].Attributes["href"].Value; //주소
                if (menu_title.IndexOf("공모전") >= 0 || menu_title.IndexOf("공지") >= 0)
                {
                    NewBoard(BoardType.PostType.Notice, menu_title, baseuri, href);
                }
            }
            #endregion
        }
    }
}
