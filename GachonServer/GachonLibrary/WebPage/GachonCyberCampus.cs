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
        /// <summary>
        /// 가천대학교 사이버캠퍼스의 게시글 목록을 읽어옵니다.
        /// </summary>
        /// <param name="guser"></param>
        /// <param name="board"></param>
        /// <returns></returns>
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
                    //등록된 게시글이 있다면
                    #region 각 게시글의 번호, 제목, 주소 등을 읽어옵니다.
                    if (node.InnerText.IndexOf("No registered post") < 0 && node.InnerText.IndexOf("등록된 게시글이 없습니다") < 0 && node.ChildNodes["th"] == null)
                    {
                        HtmlNodeCollection datas = node.SelectNodes(".//td");
                        string url = ParseSupport.StringFromHtml(datas[1].ChildNodes["a"].Attributes["href"].Value);
                        //읽은 게시글의 정보를 postItem에 추가
                        result.Insert(0, new PostItem(board)
                        {
                            url = url,
                            source = this,
                            no = Int32.Parse(datas[0].InnerText),
                            Title = ParseSupport.StringFromHtml(datas[1].InnerText),
                            Publisher = datas[2].InnerText,
                            time = DateTime.Parse(datas[3].InnerText)
                        });
                    }
                    #endregion
                }
            }
            return result;
        }
        /// <summary>
        /// (사이버 캠퍼스) 게시글의 내용이 담긴 페이지를 읽어옵니다.
        /// </summary>
        /// <param name="guser"></param>
        /// <param name="post"></param>
        public override void GetPage(GachonUser guser, PostItem post)
        {
            // 게시글의 제목과 내용을 읽어옵니다
            HtmlDocument dom = guser.VisitPage(post.url);
            post.Title = ParseSupport.StringFromHtmlNode(dom.DocumentNode.SelectSingleNode("//div[@class='subject']"));
            post.Content = ParseSupport.StringFromHtmlNode(dom.DocumentNode.SelectSingleNode("//div[@class='ubboard']//div[contains(@class,'text_to_html')]"));
        }

        /// <summary>
        /// (사이버캠퍼스) 메뉴 목록을 읽어옵니다.
        /// </summary>
        /// <param name="guser"></param>
        public override void SearchMenu(GachonUser guser)
        {
            Uri baseuri = new Uri("http://cyber.gachon.ac.kr/mod/ubboard/index.php?id=" + ID);
            HtmlDocument dom = guser.VisitPage(baseuri);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[contains(@class,'boards')]//tr/td[1]"); //메뉴
            #region 각 과목의 공지사항 메뉴를 찾아 저장합니다.
            foreach (HtmlNode node in sets)
            {
                string menu_title = ParseSupport.StringFromHtml(node.InnerText);//메뉴 이름
                if (node.ChildNodes["a"] == null) continue;
                string href = node.ChildNodes["a"].Attributes["href"].Value; //주소
                //공지사항 메뉴
                if (menu_title.IndexOf("Announcements") >= 0 || menu_title.IndexOf("공지") >= 0) 
                {
                    NewBoard(BoardType.PostType.Notice, menu_title, baseuri, href);
                }
            }
            #endregion
        }
    }
}
