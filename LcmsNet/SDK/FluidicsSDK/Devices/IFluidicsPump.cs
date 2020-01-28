namespace FluidicsSDK.Devices
{
    /// <summary>
    /// Interface to control a pump
    /// </summary>
    public interface IFluidicsPump : IFluidicsDevice
    {
        // event used to notify of a change in flowrate
        //event EventHandler<PumpEventArgs<double>> FlowChanged;
        // event used to notify of a change in pressure
        //event EventHandler<PumpEventArgs<double>> PressureChanged;
        // event used to notify of a change in %B
        //event EventHandler<PumpEventArgs<double>> PercentBChanged;
        // Pressure exerted on/by pump
        double GetPressure();
        // Flowrate of liquid through pump
        double GetFlowRate();
        // %B in pump.
        double GetPercentB();
        //actual measured flow rate
        double GetActualFlow();
        //double GetMixerVolume(); // Generally unnecessary, and is only even accessible on certain pumps
    }
}
