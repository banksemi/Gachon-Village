using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    public class GachonUser : GachonLibrary.GachonUser
    {
        public List<string> keyword = new List<string>();
        public GachonUser(string ID, string password) : base(ID, password)
        {
            keyword.Add("HomeWork");
            keyword.Add("중간고사");
        }
    }
}
