using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Experiment;
using LcmsNetSDK.Method;

namespace CoreSampleValidator
{
    /// <summary>
    /// Validates a sample for the basics before it can be run by LCMSNet.
    /// </summary>
    [Export(typeof(ISampleValidator))]
    [ExportMetadata("Name", "CoreSampleValidator")]
    [ExportMetadata("Version", "1.0")]
    public class CoreSampleValidator : ISampleValidator
    {
        /// <summary>
        /// Validates a sample based on the methods being correct and not in error.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<SampleValidationError> ValidateSamples(ISampleInfo sample)
        {
            var errors = new List<SampleValidationError>();

            // Validate the LC-Method
            if (string.IsNullOrWhiteSpace(sample.LCMethodName) || !LCMethodManager.Manager.TryGetLCMethod(sample.LCMethodName, out var method))
            {
                errors.Add(new SampleValidationError("The LC-Method was not selected.", SampleValidationErrorType.LCMethodNotSelected));
            }
            else
            {
                var manager = DeviceManager.Manager;
                if (method.Events.Count < 1)
                {
                    errors.Add(new SampleValidationError("The LC-Method was not selected or does not contain any events.", SampleValidationErrorType.LCMethodHasNoEvents));
                }
                else
                {
                    foreach (var lcEvent in method.Events)
                    {
                        // VALIDATE THE DEVICE!!!
                        var device = lcEvent.Device;

                        // Make sure the device is not null (it shouldn't be, but...)
                        if (device != null)
                        {
                            // Make sure the device exists in the configuration correctly!
                            if (device.GetType() == typeof(TimerDevice))
                            {
                                // Do nothing!
                            }
                            else if (!manager.Devices.Contains(device))
                            {
                                errors.Add(new SampleValidationError(
                                    $"The device {device.Name} in the LC-Method does not exist.  Make sure the method is valid.",
                                    SampleValidationErrorType.DeviceDoesNotExist));
                            }
                        }
                        else
                        {
                            errors.Add(new SampleValidationError(
                                $"The LC-Method {method.Name} is invalid. Make sure the method is built correctly. ",
                                SampleValidationErrorType.LCMethodIncorrect));
                        }
                    }
                }
            }

            return errors;
        }
    }
}
