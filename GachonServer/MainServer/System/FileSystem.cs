using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NetworkLibrary;
using NetworkLibrary.File;
using SQL_Library;
using Newtonsoft.Json.Linq;
namespace MainServer
{
    public static class FileSystem
    {
        /// <summary>
        /// 실제 존재하지 않는 랜덤한 파일 이름을 만듭니다.
        /// </summary>
        /// <param name="inputname">입력 해쉬값 (큰 의미 없음) </param>
        /// <returns></returns>
        public static string GetRandomName(string inputname)
        {
            // 파일 디렉토리가 존재하지 않은경우 자동 생성
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }
            string name;
            do
            {
                // 파일 이름 만들기
                name = "files/" + DateTime.Now.ToString("yyyy-MM-dd") + inputname.GetHashCode() + new Random().Next(0, 400000000);
            }
            while (File.Exists(name)); // 해당 이름을 가지는 파일이 이미 있을경우 다시 이름 생성
            return name;
        }
        /// <summary>
        /// Mysql에 파일 정보(메타데이터)를 입력합니다.
        /// </summary>
        /// <param name="path">파일 실제 경로</param>
        /// <param name="original_file_name">서버에 입력될 파일 이름</param>
        /// <param name="owner">파일의 주인</param>
        /// <returns>실제 입력된 row의 번호</returns>
        public static long FileQuery(string path, string original_file_name, User owner)
        {
            FileInfo fileInfo = new FileInfo(path);
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "INSERT INTO file(name,size,path,owner,date) VALUES (?name,?size,?path,?owner,?date)");
            node["name"] = original_file_name;
            node["size"] = fileInfo.Length;
            node["path"] = path;
            node["owner"] = owner.ID;
            node["date"] = DateTime.Now;
            long no = node.ExecuteInsertQuery();
            return no;
        }
        /// <summary>
        /// Mysql에서 파일 정보를 불러옵니다. (type, no size, name, date, owner)
        /// </summary>
        /// <param name="no">mysql에 입력된 row 번호</param>
        /// <returns></returns>
        public static JObject GetFileItem(int no)
        {
            // 해당 번호의 파일이 실제로 있는지 확인 + 파일 정보 불러오기
            MysqlNode node = new MysqlNode(private_data.mysqlOption, "SELECT file.name, file.size, account.name as owner, date FROM file join account on file.owner=account.id where file_no=?no");
            node["no"] = no;
            JObject item = null;
            using (node.ExecuteReader())
            {
                if (node.Read())
                {
                    item = new JObject();
                    item["type"] = NetworkProtocol.Inventory_Add;
                    item["no"] = no;
                    item["size"] = node.GetInt("size");
                    item["name"] = node.GetString("name");
                    item["date"] = node.GetDateTime("date");
                    item["owner"] = node.GetString("owner");
                    return item;
                }
            }
            return null;
        }
    }
}
