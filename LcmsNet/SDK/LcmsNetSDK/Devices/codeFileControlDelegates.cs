using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Devices
{
    public delegate void DelegateNameChanged(object sender, string newname);

    public delegate void DelegateSaveRequired(object sender);

    //Valves
    /// <summary>
    /// Defines calling structure for when a valves position has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="newPosition"></param>
    public delegate void DelegatePositionChanged(object sender, string newPosition);

    //PAL
    public delegate void DelegateWait(object sender);

    public delegate void DelegateFree(object sender);
}