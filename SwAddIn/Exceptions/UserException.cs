using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Exceptions
{
    public class UserException : Exception
    {
        public UserException(string msg) : base(msg)
        {
        }

        public UserException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }
}
