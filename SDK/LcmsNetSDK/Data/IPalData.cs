namespace LcmsNetSDK.Data
{
    public interface IPalData
    {
        /// <summary>
        /// Vial number to pull sample from.
        /// </summary>
        int Well { get; }

        /// <summary>
        /// Name of the PAL tray to use.
        /// </summary>
        string PALTray { get; }

        /// <summary>
        /// Name of the PAL method to run.
        /// </summary>
        string Method { get; set; }
    }
}
