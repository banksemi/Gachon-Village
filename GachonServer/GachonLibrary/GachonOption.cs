using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
namespace GachonLibrary
{
    public static class GachonOption
    {
        /// <summary>
        /// 게시글을 파싱할때 해당 페이지를 읽습니다. 
        /// false로 설정할경우 타이틀이 짤리거나, 게시글 내용을 확인할수 없지만, 게시글의 조회수가 늘어나지는 않습니다.
        /// </summary>
        public static bool VisitPage = true;
        private static MysqlOption _MysqlOption = null;
        public static MysqlOption MysqlOption
        {
            get { return _MysqlOption; }
            set { _MysqlOption = value;
                    GachonObjects.Initialize();
            }
        }
    }
}
