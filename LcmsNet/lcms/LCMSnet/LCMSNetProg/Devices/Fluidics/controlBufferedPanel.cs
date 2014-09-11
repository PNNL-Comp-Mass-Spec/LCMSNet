using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet.Devices.Fluidics
{
    /// <summary>
    /// Custom panel class that includes double buffering, used for rendering fluidics objects
    /// </summary>
    public class controlBufferedPanel:System.Windows.Forms.Panel
    {
        public controlBufferedPanel()
            : base()
        {            
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint| ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }
}
