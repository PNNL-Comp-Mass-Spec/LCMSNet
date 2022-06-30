using System.Collections.Generic;
using LcmsNet.Data;

namespace LcmsNet.Method.ViewModels
{
    /// <summary>
    /// Form that displays the throughput for the given samples.
    /// </summary>
    public class ThroughputPreviewViewModel : LCMethodTimelineViewModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ThroughputPreviewViewModel()
        {
        }

        /// <summary>
        /// Displays the alignment for the samples.
        /// </summary>
        /// <param name="samples"></param>
        public void ShowAlignmentForSamples(IReadOnlyList<SampleData> samples)
        {
            // Show the samples
            UpdateSampleMethods(samples);

            // Align the samples
            var optimizer = new LCMethodOptimizer();
            optimizer.UpdateRequired += optimizer_UpdateRequired;
            optimizer.AlignSamples(samples);

            // Display end product
            if (optimizer.Methods != null)
                UpdateSampleMethods(samples);
        }

        /// <summary>
        /// Renders the alignment as it occurs.
        /// </summary>
        /// <param name="sender"></param>
        void optimizer_UpdateRequired(LCMethodOptimizer sender)
        {
            RenderMethods(sender.Methods);
        }

        /// <summary>
        /// Renders the sample methods.
        /// </summary>
        void UpdateSampleMethods(IReadOnlyList<SampleData> samples)
        {
            var methods = new List<LCMethod>();
            foreach (var sample in samples)
            {
                sample.SetActualLcMethod();

                if (sample?.ActualLCMethod == null)
                    continue;

                // Use the "Actual LC Method", so we don't have multiple copies competing for the same times, methods, etc.
                methods.Add(sample.ActualLCMethod);
            }

            if (methods.Count > 0)
                RenderMethods(methods);
        }

        /// <summary>
        /// Renders all of the methods.
        /// </summary>
        /// <param name="methods"></param>
        private void RenderMethods(IReadOnlyList<LCMethod> methods)
        {
            RenderLCMethod(methods);
        }
    }
}
