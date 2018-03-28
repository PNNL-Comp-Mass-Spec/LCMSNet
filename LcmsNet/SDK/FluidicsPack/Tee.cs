using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    [classDeviceControl(null,
        typeof(FluidicsTee),
        "Tee",
        "Fluidics Components")]
    public class Tee : FluidicsComponentBase
    {
        #region Methods
        public Tee()
        {
            Name = "Tee";
        }
        #endregion
    }
}
