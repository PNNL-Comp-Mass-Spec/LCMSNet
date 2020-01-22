﻿/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 8/16/2013
 *
 ********************************************************************************************************/

using System.Windows.Media;

namespace FluidicsSDK.Base
{
    public abstract class Fluid
    {
        protected Fluid()
        {
            FluidColor = Colors.Black;
            Name = "someFluid";
            Location = null;
        }

        public Color FluidColor { get; }

        public string Name { get; }

        public FluidicsDevice Location { get; set; }

        // Move from current device to next device.
        public void Move(FluidicsDevice device)
        {
            Location = device;
        }
    }
}
