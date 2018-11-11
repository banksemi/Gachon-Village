using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class DuplicationError : System.Exception
    {
        public DuplicationError()
        {

        }
        public DuplicationError(string message) : base(message)
        {

        }
    }
}
