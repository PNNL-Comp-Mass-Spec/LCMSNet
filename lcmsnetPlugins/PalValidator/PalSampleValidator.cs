using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetData.Data;
using LcmsNetData.Logging;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Experiment;

namespace LcmsNetPlugins.PalValidator
{
    /// <summary>
    /// Sample Validator for PAL AutoSampler
    /// </summary>
    [Export(typeof(ISampleValidator))]
    [ExportMetadata("Name", "PalSampleValidations")]
    [ExportMetadata("Version", "1.0")]
    public class PalSampleValidator:ISampleValidator
    {
        /// <summary>
        /// Validate the sample blocks
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        public List<SampleData> ValidateBlocks(List<SampleData> samples)
        {
            // Blocks don't matter to the PAL.
            return new List<SampleData>();
        }

        /// <summary>
        /// Validate the sample PAL settings: PAL method, tray, and vial/well
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<SampleValidationError> ValidateSamples(SampleData sample)
        {
            var errors = new List<SampleValidationError>();

            if (AutoSamplers.ConnectedAutoSamplers.Count == 0)
            {
                LogOnce("NoPALNoValidate", "Could not access the PAL for sample validation! Will not validate PAL settings for sample.");
                return errors;
            }

            //
            // We've validated the method, and the devices... Now we need to validate the PAL settings.
            //
            if (string.IsNullOrEmpty(sample.PAL.Method))
                errors.Add(new SampleValidationError("The PAL Method is not set.", SampleValidationErrorType.PalMethodNotSpecified));

            if (string.IsNullOrEmpty(sample.PAL.PALTray))
            {
                errors.Add(new SampleValidationError("The PAL Tray is not set.", SampleValidationErrorType.PalTrayNotSpecified));
            }
            else if (sample.PAL.PALTray == "(select)")
            {
                errors.Add(new SampleValidationError("The PAL Tray is not set.", SampleValidationErrorType.PalTrayNotSpecified));
            }

            if (sample.DmsData.DatasetName.ToLower() == "(unused)")
            {
                errors.Add(new SampleValidationError("The dataset name is not correct.", SampleValidationErrorType.DatasetNameError));
            }

            //
            // Make sure the volume is set.
            //
            if (sample.Volume < CartConfiguration.MinimumVolume)
            {
                errors.Add(new SampleValidationError("The Sample Volume is lower than the instrument can handle.", SampleValidationErrorType.InjectionVolumeOutOfRange));
            }

            if (AutoSamplers.ConnectedAutoSamplers.Count == 0 || !AutoSamplers.ConnectedAutoSamplers[0].TrayNamesAndMaxVials.TryGetValue(sample.PAL.PALTray, out var maxVial))
            {
                LogOnce("NoPAL", "Could not access the PAL for sample validation!");
                if (sample.PAL.Well <= PalData.CONST_DEFAULT_VIAL_NUMBER || sample.PAL.Well > 1536)
                    errors.Add(new SampleValidationError("The PAL Well/Vial is not set.", SampleValidationErrorType.PalVialNotSpecified));
            }
            else
            {
                LogOnce("UsingPAL", $"Using PAL data for sample validation, tray {sample.PAL.PALTray}, vial/well {sample.PAL.Well}");
                if (sample.PAL.Well <= PalData.CONST_DEFAULT_VIAL_NUMBER)
                {
                    errors.Add(new SampleValidationError("The PAL Well/Vial is not set.", SampleValidationErrorType.PalVialNotSpecified));
                }
                else if (sample.PAL.Well > maxVial)
                {
                    errors.Add(new SampleValidationError($"The PAL Well/Vial is out of range for tray {sample.PAL.PALTray}; maximum is {maxVial}, value is {sample.PAL.Well}.", SampleValidationErrorType.PalVialNotSpecified));
                }
            }

            return errors;
        }

        private void LogOnce(string key, string message)
        {
            if (LoggedKeys.Contains(key))
            {
                return;
            }

            ApplicationLogger.LogMessage(0, message);
            LoggedKeys.Add(key);
        }

        private static readonly List<string> LoggedKeys = new List<string>(2);
    }
}
