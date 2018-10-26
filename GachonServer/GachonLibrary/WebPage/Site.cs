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
        public abstract void PageSearch(GachonUser guser);
        public void Update(GachonUser guser)
        {
            // 자기 자신에게 락을 걸고 수행. 
            // 락을 걸지 않으면 동시에 여러 쓰레드가 업데이트 요청시 중복된 결과 반환 가능성 존재.
            lock(this)
            {
                PageSearch(guser);
            }
        }
    }
}
