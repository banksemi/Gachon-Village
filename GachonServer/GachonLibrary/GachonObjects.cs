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


        public static bool Did_init = false;
      
        //맨 처음 서버를 셋팅을 함. 표에 있는 모든 강의를 읽어온다. 읽어올 때는 SQL 테이블에 추가되지 않도록 false
        public static void Get_SQL_value()
        {
            if (!Did_init)
            {
                Did_init = true;

                MysqlNode node = new MysqlNode(GachonOption.MysqlOption, "SELECT * FROM course");
                using (node.ExecuteReader())
                {
                    while (node.Read())
                    {
                        bool isClassType = node.GetString("type") == "Class";
                        GachonClass newclass = GachonClass.GetObject(node.GetString("name"), node.GetString("no"), isClassType, false);  //title , key
                        if (node.GetString("eclass").Length !=0)
                        {
                            newclass.CombineSite(new GachonEClass(node.GetString("eclass")),false);
                        }
                        if (node.GetString("gcafe").Length != 0)
                        {
                            newclass.CombineSite(new GachonCafe(node.GetString("gcafe")), false);
                        }
                        if (node.GetString("cyber").Length != 0)
                        {
                            newclass.CombineSite(new GachonCyberCampus(node.GetString("cyber")), false);
                        }
                        if (node.GetString("navercafe").Length != 0)
                        {
                            newclass.CombineSite(new NaverCafe(node.GetString("navercafe")), false);
                        }
                    }
                }
            } 
        }
    }
}
