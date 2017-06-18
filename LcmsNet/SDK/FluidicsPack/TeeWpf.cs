using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    [classDeviceControlAttribute(null,
        typeof(FluidicsTeeWpf),
        "Tee",
        "Fluidics Components")]
    public class TeeWpf : FluidicsComponentBase
    {
        #region Methods
        public TeeWpf()
        {
            Name = "Tee";
        }
        #endregion
    }
}
