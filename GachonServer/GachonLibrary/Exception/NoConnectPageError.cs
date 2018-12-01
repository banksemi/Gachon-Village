using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GachonLibrary
{
    public class NoConnectPageError : System.Exception
    {
        public NoConnectPageError()
        {

        }
        public NoConnectPageError(string message) : base(message)
        {

        }
    }
}
