using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SQL_Library;
namespace MainServer
{
    static partial class Function
    {
        /// <summary>
        /// 프로그램 시작시 처음 실행되는 함수입니다. 이 함수에서 NPC를 추가합니다.
        /// </summary>
        public static void Init_Load()
        {
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM npc");
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    // MYSQL로부터 데이터를 읽어 새로운 NPC를 생성합니다.
                    NPC npc = new NPC();
                    npc.group = "NPC";
                    npc.skin = node.GetString("skin");
                    npc.name = node.GetString("name");
                    npc.function = node.GetString("function");
                    npc.position = new Vector4(node.GetFloat("x"), node.GetFloat("y"), node.GetFloat("z"), node.GetFloat("q"));
                    // 혹시 해당 NPC에 대사가 있을경우 그 대사 정보를입력합니다.
                    MysqlNode messagenode = new MysqlNode(private_data.mysqlOption, "SELECT * FROM npc_message WHERE npc_no=?no");
                    messagenode["no"] = node.GetInt("npc_no");
                    using (messagenode.ExecuteReader())
                    {
                        while (messagenode.Read())
                        {
                            // 해당 NPC에게 대사 추가
                            npc.AddMessage(messagenode.GetString("message"));
                            Console.WriteLine("NPC " + npc.name + " 메세지 입력 -> " + messagenode.GetString("message"));
                        }
                    }
                    // NPC의 로딩이 완료되었고, 다른 유저들이 이 NPC를 볼 수 있도록 Start함수를 실행시킵니다.
                    npc.Start();
                    Console.WriteLine("NPC " + npc.name + " 추가");
                }
            }

            // 이어서 Group_Load 함수를 실행시킵니다.
            Group_Load();
        }
        private static void Group_Load()
        {
            // 모든 그룹에 대해 정보를 읽어옵니다.
            // Group은 Course와 같은 방식으로 접근이 가능하며,
            // 세부 정보는 GROUP 테이블에 담겨있기때문에 
            // Course, Group 테이블을 조인합니다.
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT * FROM course join `group` on group.group_name=course.no");
            using (node.ExecuteReader())
            {
                while (node.Read())
                {
                    // 그룹 이름, 그룹장, 위치 정보를 이용하여 새로운 그룹을 서버에 추가시킵니다.
                    // Study 클래스같은 경우는 NPC를 상속하는 클래스입니다.
                    Study group = new Study(node.GetString("group_name"), node.GetString("master"), new Vector4(node.GetFloat("x"), node.GetFloat("y"), node.GetFloat("z"), node.GetFloat("q")));
                    group.Start();
                    Console.WriteLine("Group " + group.name + " 추가");
                }
            }
        }
    }
}
