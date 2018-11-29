using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
namespace MainServer
{
    class Study : GameObject
    {
        public string key;
        public static new Dictionary<string, Study> Items = new Dictionary<string, Study>();
        public Study(string name, Vector4 position)
        {
            skin = "Group Sign";
            key = name;
            this.name = name;
            this.group = "그룹";
            this.position = position;
            Items.Add(key, this);
        }
        /// <summary>
        /// 해당 그룹에 속한 모든 유저의 ID를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public List<string> GroupUsers()
        {
            List<string> idlist = new List<string>();
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT student_id FROM takes_course WHERE course_no=?groupid");
            node["groupid"] = name;
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    idlist.Add(node.GetString("student_id"));
                }
            }
            return idlist;
        }
    }
}
