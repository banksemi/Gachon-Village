using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using WebSupport;
using System.Text.RegularExpressions;
namespace GachonLibrary
{
    public class GachonSite : Site
    {
        string SiteUrl = null;
        public GachonSite(string ID, string Type, string SiteUrl) : base(ID, Type)
        {
            this.SiteUrl = SiteUrl;
        }
        /// <summary>
        /// 공지사항, 자유게시판 등 메뉴를 읽어오는 함수입니다.
        /// </summary>
        /// <param name="guser"></param>
        public override void SearchMenu(GachonUser guser)
        {
            if (boards.Count == 0)// 값이 없을때만 실행
            {
                // 웹사이트의 메뉴부분의 주소를 읽어옵니다.
                Uri baseuri = new Uri(SiteUrl + "board/list.jsp?Menu_seq=0&Forum_seq=" + ID);
                HtmlDocument dom = guser.VisitPage(baseuri);
                HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//ul[@class='dep']/li/a");
                if (sets == null) throw new NoConnectPageError("연결이 정상적으로 이뤄지지 않음");

                #region 각 메뉴의 종류를 분류하여 게시판 타입을 지정해줍니다.
                foreach (HtmlNode node in sets)
                {
                    string menu_title = node.InnerText;
                    string href = ParseSupport.StringFromHtml(node.Attributes["href"].Value);

                    switch (menu_title)
                    {
                        case "공지사항":
                            boards.Add(new BoardType(BoardType.PostType.Notice, menu_title, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "강의자료실":
                        case "토론/시험자료실":
                            boards.Add(new BoardType(BoardType.PostType.Lecture, menu_title, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "레포트제출실":
                            boards.Add(new BoardType(BoardType.PostType.Homework, menu_title, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "HOME":
                            break;

                        default:
                            boards.Add(new BoardType(BoardType.PostType.ETC, menu_title, new Uri(baseuri, href)));
                            break;                         
                    }              
                }
                #endregion
            }
        }
        /// <summary>
        /// 메뉴 안 게시글 목록을 읽어오는 함수입니다.
        /// </summary>
        /// <param name="guser"></param>
        /// <param name="board">메뉴</param>
        /// <returns></returns>
        public override List<PostItem> GetList(GachonUser guser, BoardType board)
        {
            //Timer timer = new Timer(Type+ID + "["+ board.url+"]", 5);
            List<PostItem> result = new List<PostItem>();
            HtmlDocument dom = guser.VisitPage(board.url);
            HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//div[@class='boardlist']/table/tbody/tr");
            //timer.Print();


            if (sets != null)
            {

                foreach (HtmlNode node in sets)
                {
                    //등록된 게시글이 존재하면
                    if (node.InnerText.IndexOf("등록된 글이") < 0)
                    {
                        //게시글의 주소를 얻음
                        HtmlNodeCollection datas = node.SelectNodes(".//td");
                        string url = datas[1].SelectSingleNode(".//a").Attributes["href"].Value;
                        url = ParseSupport.StringFromHtml(url);

                        string no = null;
                        //javascript:go page()의 파라미터를 통해 주소를 얻음
                        if (url.IndexOf("java") == 0)
                        {
                            Regex reg = new Regex(@"javascript:goPage\([0-9]+,([0-9]+)\)");
                            no = reg.Match(url).Groups[1].Value;
                            url = board.url.Replace("list.jsp", "view.jsp") + "&article_no=" + no;                            
                        }
                        //그 외에는 절대경로를 얻어냄
                        else
                        {
                            Uri uri = new Uri(new Uri(board.url), url);
                            url = uri.AbsoluteUri;
                            no = ParseSupport.UrlQueryParser(url)["article_no"].ToString();
                        }
                        PostItem item;

                        #region 레포트 제출실 메뉴일 경우
                        if (board.type == BoardType.PostType.Homework)
                        {
                            //각 list의 정보를 포함한 PostItem 객체를 생성한다
                            //레포트 제출실의 경우 제출 인원, 마감날짜의 정보가 일반 게시글과 다르기 때문에 분류하였음
                            item = new PostItem(BoardType.PostType.Homework, board.name)
                            {
                                source = this,
                                url = url,
                                no = Int32.Parse(no),
                                Title = ParseSupport.StringFromHtml(datas[1].InnerText),
                                time = DateTime.Now, 
                                s_time = DateTime.Parse(datas[2].InnerText.Substring(0, 10)),
                                e_time = DateTime.Parse(datas[2].InnerText.Substring(13, 10)),
                                num_submitted = Int32.Parse(datas[5].InnerText.Split('/')[0]),
                                Publisher = "관리자"
                            };                                  
                        }
                        #endregion
                        #region 그 외의 메뉴 (공지사항, 강의자료실 등)
                        else
                        {
                            item = new PostItem(board.type, board.name)
                            {
                                source = this,
                                url = url,
                                no = Int32.Parse(no),
                                Title = ParseSupport.StringFromHtml(datas[1].InnerText),
                                Publisher = datas[2].InnerText,
                                time = DateTime.Parse(datas[3].InnerText)
                            };
                        }
                        result.Insert(0, item);
                        #endregion
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 게시글 안의 내용을 읽어오는 함수입니다.
        /// </summary>
        /// <param name="guser"></param>
        /// <param name="post">게시글</param>
        public override void GetPage(GachonUser guser, PostItem post)
        {
            //게시글의 제목과 내용을 읽어옵니다.
            HtmlDocument dom = guser.VisitPage(post.url);
            post.Title = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//td[@class='tit']").InnerText);
            post.Content = ParseSupport.StringFromHtmlNode(dom.DocumentNode.SelectSingleNode("//td[contains(@class,'text')]"));
        }
    }
}
