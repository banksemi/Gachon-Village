using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NetworkLibrary;
using NetworkLibrary.File;
using SQL_Library;
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
    }
}
