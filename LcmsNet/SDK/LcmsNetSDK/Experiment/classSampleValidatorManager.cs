using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace LcmsNetDataClasses.Experiment
{
    public class classSampleValidatorManager
    {
        private const string CONST_VALIDATOR_PATH = @"LCMSNet\SampleValidators";
        private static classSampleValidatorManager m_instance;
        private readonly CompositionContainer mmef_container;
        private readonly DirectoryCatalog mmef_directorycatalog;

        private classSampleValidatorManager()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof (classSampleValidatorManager).Assembly));

            var validatorPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var validatorFolder = new DirectoryInfo(Path.Combine(validatorPath, CONST_VALIDATOR_PATH));

            if (!validatorFolder.Exists)
                validatorFolder.Create();

            mmef_directorycatalog = new DirectoryCatalog(validatorPath);
            catalog.Catalogs.Add(mmef_directorycatalog);
            this.mmef_container = new CompositionContainer(catalog);
            this.mmef_container.ComposeParts(this);
            System.Diagnostics.Debug.WriteLine(string.Format("Loaded : {0} sample validators", Validators.Count()));
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