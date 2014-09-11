using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMSL.DocumentGenerator.Core.Model
{
    public class InvalidDocumentContent:Exception
    {
        public InvalidDocumentContent()
            : base()
        {

        }

        public InvalidDocumentContent(string message)
            : base(message)
        {

        }

        public InvalidDocumentContent(string message, Exception inner)
            : base(message, inner)
        {

        }

        public InvalidDocumentContent(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }
    }
}
