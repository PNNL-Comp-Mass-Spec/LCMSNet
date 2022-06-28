namespace LcmsNet.Notification
{
    /// <summary>
    /// Notifies listeners
    /// </summary>
    public class Notifier
    {
        /*/// <summary>
        /// Writes the system health.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="setting"></param>
        public void Notify(string message, NotificationSetting setting)
        {
            WriteSystemHealth();
        }

        /// <summary>
        /// Writes the system health to
        /// </summary>
        public void WriteSystemHealth()
        {
            bool exists = Directory.Exists(Path);
            if (!exists)
            {
                try
                {
                    Directory.CreateDirectory(Path);
                    exists = true;
                }
                catch
                {

                }
            }

            if (exists)
            {
                string name     = LcmsNetDataClasses.LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME);
                string newPath = System.IO.Path.Combine(Path, name + "-systemHealth.xml");
                LcmsNet.Devices.classDeviceManager.Manager.WriteSystemHealth(newPath);
            }
        }

        /// <summary>
        /// Writes the system health to
        /// </summary>
        public void WriteSystemHealth(string note)
        {

        }*/

        /// <summary>
        /// Gets or sets the path to write system health to.
        /// </summary>
        public string Path { get; set; }
    }
}
