using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LcmsNetSDK.Experiment
{
    public class classSampleValidatorManager
    {
        private const string CONST_VALIDATOR_PATH = @"LCMSNet\SampleValidators";
        private static classSampleValidatorManager m_instance;

        private classSampleValidatorManager()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof (classSampleValidatorManager).Assembly));

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var validatorPath = Path.Combine(appDataPath, CONST_VALIDATOR_PATH);
            var validatorFolder = new DirectoryInfo(validatorPath);

            if (!validatorFolder.Exists)
                validatorFolder.Create();

            var mmefDirectorycatalog = new DirectoryCatalog(validatorPath);
            catalog.Catalogs.Add(mmefDirectorycatalog);
            var mmefContainer = new CompositionContainer(catalog);
            mmefContainer.ComposeParts(this);
            Debug.WriteLine($"Loaded : {Validators.Count()} sample validators");
        }

        public static classSampleValidatorManager Instance => m_instance ?? (m_instance = new classSampleValidatorManager());

        [ImportMany]
        public IEnumerable<Lazy<ISampleValidator, ISampleValidatorMetaData>> Validators { get; set; }
    }
}