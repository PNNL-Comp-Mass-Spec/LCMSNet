using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newport.ESP300
{
    class ESP300Exception:Exception
    {

        public ESP300Exception()
            :base()
        {
        }

        public ESP300Exception(string message)
            : base(message)
        {
        }

        public ESP300Exception(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ESP300Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
       
    }

}
