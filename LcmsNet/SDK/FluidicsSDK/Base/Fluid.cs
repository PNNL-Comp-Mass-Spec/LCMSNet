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

        /// <summary>
        /// Move from current device to next device.
        /// </summary>
        /// <param name="device"></param>
        public void Move(FluidicsDevice device)
        {
            Location = device;
        }
    }
}
