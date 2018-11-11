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
        public override void SearchMenu(GachonUser guser)
        {
            if (boards.Count == 0)// 값이 없을때만 실행
            {
                Uri baseuri = new Uri(SiteUrl + "board/list.jsp?Menu_seq=0&Forum_seq=" + ID);
                HtmlDocument dom = guser.VisitPage(baseuri);
                HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//ul[@class='dep']/li/a");
                foreach(HtmlNode node in sets)
                {
                    string menu_title = node.InnerText;
                    string href = ParseSupport.StringFromHtml(node.Attributes["href"].Value);

                    switch (menu_title)
                    {
                        case "공지사항":
                            boards.Add(new BoardType(BoardType.PostType.Notice, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "강의자료실":
                        case "토론/시험자료실":
                            boards.Add(new BoardType(BoardType.PostType.Lecture, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "레포트제출실":
                            boards.Add(new BoardType(BoardType.PostType.Homework, new Uri(baseuri, href).AbsoluteUri));
                            break;

                        case "HOME":
                            break;

                        default:
                            boards.Add(new BoardType(BoardType.PostType.ETC, new Uri(baseuri, href)));
                            break;                         
                    }              
                }
            }
        }
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
                    if (node.InnerText.IndexOf("등록된 글이") < 0)
                    {
                        HtmlNodeCollection datas = node.SelectNodes(".//td");
                        string url = datas[1].SelectSingleNode(".//a").Attributes["href"].Value;
                        url = ParseSupport.StringFromHtml(url);

                        string no = null;
                        if (url.IndexOf("java") == 0)
                        {
                            Regex reg = new Regex(@"javascript:goPage\([0-9]+,([0-9]+)\)");
                            no = reg.Match(url).Groups[1].Value;
                            url = board.url.Replace("list.jsp", "view.jsp") + "&article_no=" + no;                            
                        }
                        else
                        {
                            Uri uri = new Uri(new Uri(board.url), url);
                            url = uri.AbsoluteUri;
                            no = ParseSupport.UrlQueryParser(url)["article_no"].ToString();
                        }
                        PostItem item;
                        if (board.type == BoardType.PostType.Homework)
                        {
                            item = new PostItem(BoardType.PostType.Homework)
                            {
                                source = this,
                                url = url,
                                no = Int32.Parse(no),
                                Title = ParseSupport.StringFromHtml(datas[1].InnerText),
                                s_time = DateTime.Parse(datas[2].InnerText.Substring(0, 10)),
                                e_time = DateTime.Parse(datas[2].InnerText.Substring(13, 10)),
                                num_submitted = Int32.Parse(datas[5].InnerText.Split('/')[0])
                            };                                  
                        }
                        else
                        {
                            item = new PostItem(BoardType.PostType.Homework)
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
                    }
                }
            }
            return result;
        }
        public override void GetPage(GachonUser guser, PostItem post)
        {
            HtmlDocument dom = guser.VisitPage(post.url);
            post.Title = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//td[@class='tit']").InnerText);
            post.Content = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//td[contains(@class,'text')]").InnerText);
        }
    }
}
