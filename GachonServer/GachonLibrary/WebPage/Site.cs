﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
namespace GachonLibrary
{
    public abstract class Site
    {
        /* 이 객체는 ID Type을 제외한 다른 정보를 가지지 않도록 설계한다.
            중복된 사이트가 생성될 수 있으며, GachonClass 단에서 자동으로 걸러진다.
             */
        public delegate void PostData(PostItem postItem);
        public event PostData NewPost;
        public List<BoardType> boards = new List<BoardType>();
        public string Type { get; private set; }
        public string ID { get; private set; }
        private bool ReadBoardList = false;
        public Site(string ID, string Type)
        {
            this.ID = ID;
            this.Type = Type;
        }

        /// <summary>
        /// 사이트가 결합(생성)될때 이 함수는 GachonClass에 의해 자동으로 실행됩니다.
        /// </summary>
        public void Start()
        {
        }
        protected void NewPostEvent(PostItem item)
        {
            NewPost.Invoke(item);
        }
        public abstract void SearchMenu(GachonUser guser);
        public abstract List<PostItem> GetList(GachonUser guser, BoardType board);
        public abstract void GetPage(GachonUser guserm, PostItem item);

        public void PageSearch(GachonUser guser)
        {
            if (ReadBoardList == false)
            {
                ReadBoardList = true;
                try
                {
                    SearchMenu(guser);
                }
                catch (NoConnectPageError E)
                {
                    Console.WriteLine("[PageSearch Error] " + E.Message);
                    ReadBoardList = false;
                    return;
                }

                #region 메뉴별 가장 최근의 게시글 번호 갱신 (article 테이블에서 sitetype, siteid, board_name 으로 그룹화)
                MysqlNode node = new MysqlNode(GachonOption.MysqlOption,
                    "select sitetype, siteid, board_name, max(no) as NO from article where siteType = ?sitetype and siteid = ?siteid group by sitetype, siteid, board_name;");
                node["sitetype"] = Type;
                node["siteid"] = ID;
                using (node.ExecuteReader())
                {
                    while (node.Read())
                    {
                        string find_menuName = node.GetString("board_name"); //각 개시판에
                        int find_count = node.GetInt("no"); // max(no)인 가장 최근 게시글 번호
                        foreach (BoardType board in boards)
                        {
                            if(board.name == find_menuName) // 각 게시판마다 최근에 읽은 게시글 번호를 갱신해준다
                            {
                                board.LastNo = find_count;
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            foreach (BoardType board in boards)
            {
                List<PostItem> items = null;
                try
                {
                    items = GetList(guser, board);
                }
                catch (NoConnectPageError e) { }
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        //게시판이 마지막에 읽은 번호보다 최신의 게시글이 등록되면
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
        public void Update(GachonUser guser)
        {
            // 자기 자신에게 락을 걸고 수행. 
            // 락을 걸지 않으면 동시에 여러 쓰레드가 업데이트 요청시 중복된 결과 반환 가능성 존재.
            lock(this)
            {
                PageSearch(guser);
            }
        }
        /// <summary>
        /// 새로운 게시판을 추가해줍니다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="baseUri"></param>
        /// <param name="href"></param>
        protected void NewBoard(BoardType.PostType type, string name, Uri baseUri, string href)
        {
            boards.Add(new BoardType(type, name, new Uri(baseUri, href).AbsoluteUri));
        }
    }
}
