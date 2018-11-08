using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace GachonLibrary
{
    public class GachonClass
    {
        /// <summary>
        /// 자동 크롤링 사용시 해당 시간(초)의 한번씩 강제로 게시글을 읽고, 모든 정보를 갱신합니다.
        /// </summary>
        public static int AutoCompulsionDelay = 5;
        /// <summary>
        /// 자동 크롤링 사용시 해당 시간에 한번씩 각사이트의 게시판을 읽어옵니다.
        /// </summary>
        public static int AutoCrawlingDelay = 2;

        public delegate void ClassPostData(GachonClass gclass, PostItem postItem);
        public static event ClassPostData NewPost;
        
        /// <summary>
        /// 해당 강의의 고유 번호입니다. 일반적으로 학수번호를 의미할때는 ID+Sec_ID 입니다.
        /// </summary>
        public string ID;
        /// <summary>
        /// 분반 정보입니다.
        /// </summary>
        public string Sec_ID;
        /// <summary>
        /// 해당 강의의 이름입니다.
        /// </summary>
        public string Title;
        /// <summary>
        /// 해당 강의의 개설 연도입니다.
        /// </summary>
        public int year;
        /// <summary>
        /// 개설 연도, ID, 분반 정보를 하나로 합친 고유 키값입니다.
        /// </summary>
        public string Key { get { return year + ID + Sec_ID; } }
        /// <summary>
        /// 해당 강의에 연결되있는 사이트(Eclass,Cafe...) 목록입니다.
        /// </summary>
        public Dictionary<string, Site> Sites = new Dictionary<string, Site>();
        /// <summary>
        ///  해당 강의를 수강중이며, 로그인처리가 되있는 학생 목록입니다.
        /// </summary>
        public List<GachonUser> Users = new List<GachonUser>();
        private Thread AutoCrawlingThread = null;
        private static bool _startautocrawling = false;
        private GachonClass(string Title, string key, bool Formal = false)
        {
            if (GachonObjects.AllClass.ContainsKey(key)) throw new DuplicationError();
            // 규칙을 가진 강의일경우 (Year(4) + ID(5) + Sec_ID(3))
            if (Formal == true)
            {
                Initialization(Title, Int32.Parse(key.Substring(0, 4)), key.Substring(4, 5), key.Substring(9, 3));
            }
            else
            {
                Initialization(Title, DateTime.Now.Year, key, "001");
            }
        }
        public static GachonClass GetObject(string Title, string key, bool Formal = false)
        {
            if (GachonObjects.AllClass.ContainsKey(key)) return GachonObjects.AllClass[key];
            else
            {
                GachonClass gachonClass = new GachonClass(Title, key, Formal);
                GachonObjects.AllClass.Add(key, gachonClass);
                return gachonClass;
            }
        }
        public void CombineTakeUser(GachonUser gachonUser)
        {
            if (!gachonUser.Takes.Contains(this)) gachonUser.Takes.Add(this);
            if (!Users.Contains(gachonUser)) Users.Add(gachonUser);
        }
        private void Initialization(string Title, int year, string ID, string Sec_ID)
        {
            this.Title = Title;
            this.year = year;
            this.ID = ID;
            this.Sec_ID = Sec_ID;
            if (StartAutoCrawling == true)
            {
                AutoCrawlingThread = new Thread(CheckNewPost_Thread);
                AutoCrawlingThread.Start();
            }
        }
        public override string ToString()
        {
            return String.Format("[{0}] {1} (Eclass {2}, 사이버캠퍼스 {3}) - {4} 명 수강중", Key, Title,
                      Sites.ContainsKey("EClass") ? Sites["EClass"].ID : "없음",
                      Sites.ContainsKey("Cyber") ? Sites["Cyber"].ID : "없음",Users.Count);
        }
        /// <summary>
        /// 동일한 사이트 생성일 경우 기존 객체를 연결시킨다.
        /// </summary>
        /// <param name="site"></param>
        public void CombineSite(Site site)
        {
            lock(Sites)
            {
                // 이때 동일한 사이트가 존재할 경우 새로 추가하려는 객체를 추가하지 않고 
                // 기존 객체를 연결한다.
                if (GachonObjects.AllSite.ContainsKey(site.Type + site.ID))
                    site = GachonObjects.AllSite[site.Type + site.ID];
                else
                {
                    GachonObjects.AllSite.Add(site.Type + site.ID, site);
                    site.Start();
                }
                // 이 강의에 사이트 연결
                Sites.Add(site.Type, site);
                // 해당 사이트에 새로운 게시글이 올라올경우 이벤트 실행
                site.NewPost += Site_NewPost;

            }
        }
        /// <summary>
        /// 각 사이트에서 새로운 알림이 올 경우 이 함수가 실행됩니다.
        /// </summary>
        /// <param name="postItem"></param>
        private void Site_NewPost(PostItem postItem)
        {
            // 이 객체의 이벤트를 듣고있는 리스너에게 이벤트 메세지 전달.
            NewPost?.Invoke(this,postItem);
        }
        /// <summary>
        /// 새로운 게시글이 있는지 확인합니다.
        /// 탐색 세션은 기존에 GachonUser로 로그인중인 유저중 한명을 랜덤으로 선택합니다.
        /// </summary>
        public void CheckNewPost()
        {
            if (Users.Count > 0)
            {
                Random rd = new Random();
                int a = rd.Next(Users.Count);
                GachonUser gu = Users[a];
                foreach (Site site in Sites.Values)
                {
                    site.Update(gu);
                }
            }
        }
        private void CheckNewPost_Thread()
        {
            while(true)
            {
                CheckNewPost();
                Thread.Sleep(TimeSpan.FromSeconds(AutoCrawlingDelay));
            }
        }
        /// <summary>
        /// 다음 값을 True로 설정하면 각 클래스마다 쓰레드가 실행되어 새로운 게시글이 발생될때 Invoke 처리를 합니다.
        /// 다음 값을 False로 설정하면 각 클래스에 있는 쓰레드들은 종료됩니다.
        /// </summary>
        public static bool StartAutoCrawling
        {
            get
            {
                return _startautocrawling;
            }
            set
            {
                lock (GachonObjects.AllClass)
                {
                    if (_startautocrawling == false && value == true)
                    {
                        foreach (GachonClass gc in GachonObjects.AllClass.Values)
                        {
                            gc.AutoCrawlingThread = new Thread(gc.CheckNewPost_Thread);
                            gc.AutoCrawlingThread.Start();
                        }
                    }
                    else if (_startautocrawling == true && value == false)
                    {
                        foreach (GachonClass gc in GachonObjects.AllClass.Values)
                        {
                            gc.AutoCrawlingThread.Abort();
                        }
                    }
                    _startautocrawling = value;
                }
            }
        }
    }
}
