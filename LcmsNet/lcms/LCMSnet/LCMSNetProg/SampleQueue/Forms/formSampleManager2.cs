
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
//using LcmsNet.Devices.NetworkStart;


namespace LcmsNet.SampleQueue.Forms
{
    public partial class formSampleManager2 : Form
    {
        #region "Events"

        /// <summary>
        /// Fired when a sample run should be stopped.
        /// </summary>
        public event EventHandler Stop;

        #endregion

        /// <summary>
        /// Default constructor that takes cart configuration data.
        /// </summary>
        /// <param name="queue">Sample queue to provide interface to.</param>
        public formSampleManager2(classSampleQueue queue)
        {
            //
            // Initializes the windows form objects
            //
            InitializeComponent();
            // ToDo: Initialize(queue);
        }

        /// <summary>
        /// Default constructor for constructor.
        /// </summary>
        public formSampleManager2()
        {
            InitializeComponent();
        }

        public void PreviewAvailable(object sender, SampleProgressPreviewArgs e)
        {
            if (e?.PreviewImage == null)
                return;
            /*
            var width = mpicture_preview.Width;
            var height = mpicture_preview.Height;

            if (width <= 0)
                return;

            if (height <= 0)
                return;
            mpicture_preview.Image?.Dispose();

            try
            {
                Image x = new Bitmap(width, height);
                using (var g = Graphics.FromImage(x))
                {
                    g.DrawImage(e.PreviewImage, 0, 0, width, height);
                    mpicture_preview.Image = x;
                }
            }
            catch (Exception)
            {
                // Ignore exceptions here
            }
            e.Dispose();

            */
        }

        /// <summary>
        /// Supplies the list of PAL trays to the sample queue and associated objects.
        /// </summary>
        public void AutoSamplerTrayList(object sender, classAutoSampleEventArgs args)
        {
            var trays = args.TrayList;

            //ToDo:
            /*mcontrol_sequenceView.AutoSamplerTrays = trays;
            mcontrol_column1.AutoSamplerTrays = trays;
            mcontrol_column2.AutoSamplerTrays = trays;
            mcontrol_column3.AutoSamplerTrays = trays;
            mcontrol_column4.AutoSamplerTrays = trays;
            */
        }

        /// <summary>
        /// Supplies a list of instrument methods to the sample queue and associated objects.
        /// </summary>
        public void InstrumentMethodList(object sender, classNetworkStartEventArgs args)
        {
            var methods = args.MethodList;

            //ToDo:
            /*mcontrol_sequenceView.InstrumentMethods = methods;
            mcontrol_column1.InstrumentMethods = methods;
            mcontrol_column2.InstrumentMethods = methods;
            mcontrol_column3.InstrumentMethods = methods;
            mcontrol_column4.InstrumentMethods = methods;
            */
        }

    }
}