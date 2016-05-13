using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Base;
using FluidicsSDK.Graphic;
using System.Drawing;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public class FluidicsWasteComponent:FluidicsDevice
    {
        public FluidicsWasteComponent()
        {
            const int size = 50;
            //don't really need an "object" so we'll just create a 10x10 pixel rectangle and overlay a port
            base.AddPrimitive(new FluidicsRectangle(new Point(0, 0), new Size(size, size), Color.Black, Brushes.White, true, 1));
            base.AddPort(new Point(size /2, size /2));
            m_info_controls_box.Width = 50;
            Ports[0].Sink = true;
        }

        public override void ActivateState(int state)
        {
        }

         protected override void SetDevice(IDevice device)
        {        
        }

         protected override void ClearDevice(IDevice device)
         {
         }

        public override string StateString()
        {
            return string.Empty;
        }           

        public override int CurrentState
        {
            get
            {
                //pumps don't have a state of this kind.
                return -1;
            }
            set
            {
                //do nothing
            }
        }   

    }
}
