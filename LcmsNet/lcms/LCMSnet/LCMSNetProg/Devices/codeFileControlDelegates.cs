using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices
{
    public delegate void DelegateNameChanged(object sender, string newname);
    public delegate void DelegateSaveRequired(object sender);
    public delegate void DelegatePositionChanged(object sender, LcmsNet.Devices.Valves.enumValvePosition2Pos newPosition);
}
