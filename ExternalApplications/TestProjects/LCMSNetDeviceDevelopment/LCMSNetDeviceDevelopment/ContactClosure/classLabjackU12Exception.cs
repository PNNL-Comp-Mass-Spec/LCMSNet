using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.ContactClosure
{
    public class classLabjackU12Exception : Exception
    {
        public classLabjackU12Exception()
            : base()
        {
        }
        public classLabjackU12Exception(string message)
            : base(message)
        {
        }
        public classLabjackU12Exception(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
