using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
namespace SQL_Library
{
    public class MysqlNode : IDisposable
    {
        private string sql;
        private MysqlOption option;
        private MySqlConnection conn = null;
        private Dictionary<String, object> Parameters = new Dictionary<string, object>();
        public MySqlDataReader Reader { get; private set; }
        #region
        public string GetString(string name)
        {
            return Reader.GetString(name);
        }
        #endregion
        public MysqlNode(MysqlOption option ,string sql)
        {
            this.option = option;
            ChangeSql(sql);
        }
        public void ChangeSql(string sql)
        {
            this.sql = sql;
        }

        public bool Read()
        {
            return Reader.Read();
        }
        public void AddParameter(string index, object data)
        {
            if (ContainParameter(index)) Parameters[index] = data;
            else
            {
                Parameters.Add(index, data);
            }
        }
        public bool ContainParameter(string index)
        {
            if (Parameters.ContainsKey(index)) return true;
            return false;
        }
        public object this[string index]    // Indexer declaration  
        {
            get
            {
                if (ContainParameter(index)) return Parameters[index];
                else return null;
            }
            set
            {
                AddParameter(index, value);
            }
        }
        public void AllClose()
        {
            if (Reader != null) Reader.Close();
            if (conn != null) conn.Close();
            conn = null;
            Reader = null;
        }
        private MySqlCommand Open()
        {
            AllClose(); // 모든 객체 닫기
            conn = new MySqlConnection(option.ToString());
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            foreach (string key in Parameters.Keys)
            {
                cmd.Parameters.Add(new MySqlParameter(key, Parameters[key].ToString()));
            }
            return cmd;
        }
        public MysqlNode ExecuteReader()
        {
            MySqlCommand cmd = Open();
            Reader = cmd.ExecuteReader();
            return this;
        }
        public int ExecuteNonQuery()
        {
            MySqlCommand cmd = Open();
            int result = cmd.ExecuteNonQuery();
            AllClose();
            return result;
        }
        public void Dispose()
        {
            AllClose();
        }
    }
}
