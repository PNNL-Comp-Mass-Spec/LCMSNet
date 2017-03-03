using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
      [classDeviceControlAttribute(null,
                                   typeof(FluidicsTee),
                                   "Tee",
                                   "Fluidics Components")]
    public class Tee:FluidicsComponentBase
    {
        #region Methods
        public Tee()
        {
            Name = "Tee";
        }
        #endregion
    }
}
