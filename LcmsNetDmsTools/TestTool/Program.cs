using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDmsTools;

namespace TestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            classDBTools dbt = new classDBTools();
            dbt.LoadCacheFromDMS();
        }
    }
}
