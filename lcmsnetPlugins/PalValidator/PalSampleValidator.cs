using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Experiment;
using LcmsNetSDK.Logging;

namespace PALAutoSampler.Validator
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
        public List<classSampleData> ValidateBlocks(List<classSampleData> samples)
        {
            // Blocks don't matter to the PAL.
            return new List<classSampleData>();
        }

        /// <summary>
        /// Validate the sample PAL settings: PAL method, tray, and vial/well
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public List<classSampleValidationError> ValidateSamples(classSampleData sample)
        {
            var errors = new List<classSampleValidationError>();

            //
            // We've validated the method, and the devices... Now we need to validate the PAL settings.
            //
            if (string.IsNullOrEmpty(sample.PAL.Method))
                errors.Add(new classSampleValidationError("The PAL Method is not set.", enumSampleValidationError.PalMethodNotSpecified));

            if (string.IsNullOrEmpty(sample.PAL.PALTray))
            {
                errors.Add(new classSampleValidationError("The PAL Tray is not set.", enumSampleValidationError.PalTrayNotSpecified));
            }
            else if (sample.PAL.PALTray == "(select)")
            {
                errors.Add(new classSampleValidationError("The PAL Tray is not set.", enumSampleValidationError.PalTrayNotSpecified));
            }

            if (sample.DmsData.DatasetName.ToLower() == "(unused)")
            {
                errors.Add(new classSampleValidationError("The dataset name is not correct.", enumSampleValidationError.DatasetNameError));
            }

            //
            // Make sure the volume is set.
            //
            if (sample.Volume < classCartConfiguration.MinimumVolume)
            {
                errors.Add(new classSampleValidationError("The Sample Volume is lower than the instrument can handle.", enumSampleValidationError.InjectionVolumeOutOfRange));
            }

            if (AutoSamplers.ConnectedAutoSamplers.Count == 0 || !AutoSamplers.ConnectedAutoSamplers[0].TrayNamesAndMaxVials.TryGetValue(sample.PAL.PALTray, out var maxVial))
            {
                LogOnce("NoPAL", "Could not access the PAL for sample validation!");
                if (sample.PAL.Well <= classPalData.CONST_DEFAULT_VIAL_NUMBER || sample.PAL.Well > 1536)
                    errors.Add(new classSampleValidationError("The PAL Well/Vial is not set.", enumSampleValidationError.PalVialNotSpecified));
            }
            else
            {
                LogOnce("UsingPAL", $"Using PAL data for sample validation, tray {sample.PAL.PALTray}, vial/well {sample.PAL.Well}");
                if (sample.PAL.Well <= classPalData.CONST_DEFAULT_VIAL_NUMBER)
                {
                    errors.Add(new classSampleValidationError("The PAL Well/Vial is not set.", enumSampleValidationError.PalVialNotSpecified));
                }
                else if (sample.PAL.Well > maxVial)
                {
                    errors.Add(new classSampleValidationError($"The PAL Well/Vial is out of range for tray {sample.PAL.PALTray}; maximum is {maxVial}, value is {sample.PAL.Well}.", enumSampleValidationError.PalVialNotSpecified));
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

            classApplicationLogger.LogMessage(0, message);
            LoggedKeys.Add(key);
        }

        private static readonly List<string> LoggedKeys = new List<string>(2);
    }
}
