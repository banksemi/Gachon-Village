using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
namespace GachonLibrary
{
    public static class GachonObjects
    {
        public static Dictionary<string, GachonClass> AllClass = new Dictionary<string, GachonClass>();
        public static Dictionary<string, Site> AllSite = new Dictionary<string, Site>();
        public static Dictionary<string, GachonUser> AllUser = new Dictionary<string, GachonUser>();


        //맨 처음 서버를 셋팅을 함. 표에 있는 모든 강의를 읽어온다. 이때 읽어올 때는 SQL 테이블에 추가되지 않도록 false
        public static void Initialize()
        {
            //모든 강의 정보를 데이터베이스 Course 테이블에서 읽어옴
            MysqlNode node = new MysqlNode(GachonOption.MysqlOption, "SELECT * FROM course");
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    // Format이 맞는지 확인하기 위해 Class type인지 확인
                    bool isClassType = node.GetString("type") == "Class"; 
                    GachonClass newclass = GachonClass.GetObject(node.GetString("name"), node.GetString("no"), isClassType, false);  //title , key
                    
                    //해당 강의에 대한 site정보가 있으면 해당 강의에 연결
                    if (!string.IsNullOrEmpty(node.GetString("eclass")))
                    {
                        newclass.CombineSite(new GachonEClass(node.GetString("eclass")), false);
                    }
                    if (!string.IsNullOrEmpty(node.GetString("gcafe")))
                    {
                        newclass.CombineSite(new GachonCafe(node.GetString("gcafe")), false);
                    }
                    if (!string.IsNullOrEmpty(node.GetString("cyber")))
                    {
                        newclass.CombineSite(new GachonCyberCampus(node.GetString("cyber")), false);
                    }
                    if (!string.IsNullOrEmpty(node.GetString("navercafe")))
                    {
                        newclass.CombineSite(new NaverCafe(node.GetString("navercafe")), false);
                    }
                }
            }
        }
    }
}
