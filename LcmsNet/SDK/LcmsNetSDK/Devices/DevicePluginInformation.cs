using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Encapsulates a device type and other extracted properties.
    /// </summary>
    public class DevicePluginInformation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="attribute"></param>
        public DevicePluginInformation(Type deviceType, DeviceControlAttribute attribute)
        {
            DeviceType = deviceType;
            DeviceAttribute = attribute;
        }

        public string DisplayName
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var name = base.ToString();
            if (!string.IsNullOrWhiteSpace(DeviceAttribute?.Name))
            {
                name = DeviceAttribute.Name;
            }
            return name;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public Type DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the attribute of the device.
        /// </summary>
        public DeviceControlAttribute DeviceAttribute { get; set; }

        #endregion
    }
}