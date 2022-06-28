using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.SampleValidation
{
    public class SampleValidatorManager
    {
        private const string CONST_VALIDATOR_PATH = DeviceManager.CONST_DEVICE_PLUGIN_PATH;
        private static SampleValidatorManager m_instance;

        private SampleValidatorManager()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof (SampleValidatorManager).Assembly));

            var validatorPath = CONST_VALIDATOR_PATH;
            var validatorFolder = new DirectoryInfo(validatorPath);

            if (!validatorFolder.Exists)
            {
                Validators = new List<Lazy<ISampleValidator, ISampleValidatorMetaData>>();
                return;
            }

            var mmefDirectorycatalog = new DirectoryCatalog(validatorPath);
            catalog.Catalogs.Add(mmefDirectorycatalog);
            // MEF automatically pulls in the DLL-internal Exports, it appears.
            //var defaultDirectoryCatalog = new DirectoryCatalog(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            //catalog.Catalogs.Add(defaultDirectoryCatalog);
            var mmefContainer = new CompositionContainer(catalog);
            mmefContainer.ComposeParts(this);
            //Debug.WriteLine($"Loaded : {Validators.Count()} sample validators");
        }

        public static SampleValidatorManager Instance => m_instance ?? (m_instance = new SampleValidatorManager());

        [ImportMany]
        public IEnumerable<Lazy<ISampleValidator, ISampleValidatorMetaData>> Validators { get; set; }
    }
}
