using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQL_Library;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    class AndroidFunction
    {
        public static void SendTakesList(ESocket socket)
        {
            string id = GachonSocket.GetId(socket);
            if (id != null)
            {
                MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM takes_course JOIN course ON takes_course.course_no=course.no WHERE student_id=?id");
                node["id"] = id;
                JArray group = new JArray();
                JArray course = new JArray();
                using (node.ExecuteReader())
                {
                    while(node.Read())
                    {
                        if (node.GetString("type") == "Class")
                        {
                            course.Add(node.GetString("name"));
                        }
                        else if (node.GetString("type") == "Group")
                        {

                            group.Add(node.GetString("name"));
                        }
                    }
                }

                JObject json = new JObject();
                json["type"] = AndroidProtocol.GroupList;
                json["group"] = group;
                json["class"] = course;
                socket.Send(json);
            }
        }
    }
}
