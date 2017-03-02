using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace LcmsNetDataClasses.Experiment
{
    public class classSampleValidatorManager
    {
        private const string CONST_VALIDATOR_PATH = @"LCMSNet\SampleValidators";
        private static classSampleValidatorManager m_instance;

        private classSampleValidatorManager()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof (classSampleValidatorManager).Assembly));

            var validatorPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var validatorFolder = new DirectoryInfo(Path.Combine(validatorPath, CONST_VALIDATOR_PATH));

            if (!validatorFolder.Exists)
                validatorFolder.Create();

            var mmefDirectorycatalog = new DirectoryCatalog(validatorPath);
            catalog.Catalogs.Add(mmefDirectorycatalog);
            var mmefContainer = new CompositionContainer(catalog);
            mmefContainer.ComposeParts(this);
            System.Diagnostics.Debug.WriteLine($"Loaded : {Validators.Count()} sample validators");
        }

        public static classSampleValidatorManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new classSampleValidatorManager();
                }
                return m_instance;
            }
        }

        [ImportMany]
        public IEnumerable<Lazy<ISampleValidator, ISampleValidatorMetaData>> Validators { get; set; }
    }
}