using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P;

namespace pal
{
    class classPal
    {
        PalClass mobj_testPal = new PalClass();

        public classPal()
        {
            if (System.IO.File.Exists(@"c:\data\blah.pal"))
            {
                System.IO.File.Delete(@"c:\data\blah.pal");
            }
            
        }

        public void Initialize()
        {
            System.Threading.Thread.Sleep(100);
            //int e = mobj_testPal.StartDriver("0", "COM1");
            //play tiny dancer
            

            //mobj_testPal.ResetPAL();
            System.IO.TextWriter writer = System.IO.File.CreateText(@"c:\data\blah.pal");
            writer.Close();

            /// 

            //mobj_testPal.ReadBarcode();
           // mobj_testPal.GetTrayNames();
            
        }
    }
}
