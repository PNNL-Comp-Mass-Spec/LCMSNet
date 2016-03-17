using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet
{

    public class classStatusEventArgs : EventArgs
    {
        public classStatusEventArgs(string message)
        {
            Message = message;
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}
