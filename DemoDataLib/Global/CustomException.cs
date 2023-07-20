using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Global
{
    public class CustomException : Exception
    {
        public string LangKey { get; private set; }
        public CustomException(string langKey)
        {
            LangKey = langKey;
        }
    }
}
