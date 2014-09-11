using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace serv
{
    public enum enumNetStartMessageTypes
    {
        Unknown = 0,
        Query,
        Post,
        Execute,
        Acknowledge,
        Response,
        Error,
        System,
        SystemError
    }
}
