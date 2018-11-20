using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    class Study : GameObject
    {
        public string key;
        public Study(string name, Vector4 position)
        {
            skin = "Group Sign";
            key = name;
            this.name = "[그룹] " + name;
            this.position = position;
        }
    }
}
