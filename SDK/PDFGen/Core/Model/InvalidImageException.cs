using System;

namespace PDFGenerator.Core.Model
{
    public class InvalidImageException:Exception
    {
        public InvalidImageException()
            : base()
        {

        }

        public InvalidImageException(string message)
            : base(message)
        {

        }

        public InvalidImageException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public InvalidImageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {

        }
    }
}
