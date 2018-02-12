using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    class ParseXmlException : Exception
    {
       public ParseXmlException() 
            : base()
        {
            
        }
        public ParseXmlException(String message)
            : base(message)
        {

        }
    }
}
