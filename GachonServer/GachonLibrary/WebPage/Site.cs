using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SearchMenu(guser);
            foreach (BoardType board in boards)
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
        public void Update(GachonUser guser)
        {
            // 자기 자신에게 락을 걸고 수행. 
            // 락을 걸지 않으면 동시에 여러 쓰레드가 업데이트 요청시 중복된 결과 반환 가능성 존재.
            lock(this)
            {
                PageSearch(guser);
            }
        }
        protected void NewBoard(BoardType.PostType type, string name, Uri baseUri, string href)
        {
            boards.Add(new BoardType(type, new Uri(baseUri,href).AbsoluteUri));
        }
    }
}
