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
        public static string GetRandomName(string inputname)
        {
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }
            string name;
            do
            {
                name = "files/" + DateTime.Now.ToString("yyyy-MM-dd") + inputname.GetHashCode() + new Random().Next(0, 400000000);
            }
            while (File.Exists(name));
            return name;
        }
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
