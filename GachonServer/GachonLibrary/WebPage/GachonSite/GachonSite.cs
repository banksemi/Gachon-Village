﻿using System;
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
        public List<BoardType> boards = new List<BoardType>();
        int notice_no = 0;
        string SiteUrl = null;
        public GachonSite(string ID, string Type, string SiteUrl) : base(ID, Type)
        {
            this.SiteUrl = SiteUrl;
        }
        private void SearchMenu(GachonUser guser)
        {
            if (boards.Count == 0)// 값이 없을때만 실행
            {
                Uri baseuri = new Uri(SiteUrl + "board/list.jsp?Menu_seq=0&Forum_seq=" + ID);
                HtmlDocument dom = guser.VisitPage(baseuri);
                HtmlNodeCollection sets = dom.DocumentNode.SelectNodes("//ul[@class='dep']/li/a");
                foreach(HtmlNode node in sets)
                {
                    string menu_title = node.InnerText;
                    if (menu_title == "HOME")
                        continue;
                    if (menu_title == "공지사항")
                    {
                        string href = ParseSupport.StringFromHtml(node.Attributes["href"].Value);
                        boards.Add(new BoardType(BoardType.PostType.Notice, new Uri(baseuri, href).AbsoluteUri));
                    }
                    else if (menu_title == "강의자료실" || menu_title == "토론/시험자료실")
                    {
                        string href = ParseSupport.StringFromHtml(node.Attributes["href"].Value);
                        boards.Add(new BoardType(BoardType.PostType.Lecture, new Uri(baseuri, href).AbsoluteUri));
                    }
                    else if (menu_title == "레포트제출실")
                    {
                    }
                    else
                    {
                        string href = ParseSupport.StringFromHtml(node.Attributes["href"].Value);
                        boards.Add(new BoardType(BoardType.PostType.ETC, new Uri(baseuri, href)));
                    }
                }
            }
        }
        private List<PostItem> GetList(GachonUser guser, BoardType board)
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
                        string url = datas[1].InnerHtml;
                        Regex reg = new Regex(@"javascript:goPage\([0-9]+,([0-9]+)\)");
                        url = board.url.Replace("list.jsp", "view.jsp") + "&article_no=" + reg.Match(url).Groups[1].Value;
                        result.Insert(0,new PostItem(board.type, this ,url,Int32.Parse(datas[0].InnerText),
                            ParseSupport.StringFromHtml(datas[1].InnerText),
                            datas[2].InnerText,
                             DateTime.Parse(datas[3].InnerText)));
                    }
                }
            }
            return result;
        }
        public void GetPage(GachonUser guser, PostItem post)
        {
            HtmlDocument dom = guser.VisitPage(post.url);
            post.Title = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//td[@class='tit']").InnerText);
            post.Content = ParseSupport.StringFromHtml(dom.DocumentNode.SelectSingleNode("//td[contains(@class,'text')]").InnerText);
        }
        //나중에 뒤로 넘기기
        public override void PageSearch(GachonUser guser)
        {
            SearchMenu(guser);
            foreach(BoardType board in boards)
            {
                List<PostItem> items = GetList(guser, board);
                for (int i = 0; i < items.Count; i++)
                {
                    if (board.LastNo < items[i].no)
                    {
                        if (GachonOption.VisitPage) GetPage(guser, items[i]); //아이템 정보를 읽어서 갱신
                        NewPostEvent(items[i]);
                        board.LastNo = items[i].no;
                    }
                }
                // 혹시 게시글이 삭제됬을경우 마지막 방문 게시글을 되돌린다.
                int this_last_no = 0;
                if (items.Count > 0) this_last_no = items[items.Count - 1].no;
                if (board.LastNo > this_last_no)
                {
                    board.LastNo = this_last_no;
                }
            }
        }
    }
}
