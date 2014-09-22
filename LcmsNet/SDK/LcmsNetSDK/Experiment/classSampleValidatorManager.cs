using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace LcmsNetDataClasses.Experiment
{
    public class classSampleValidatorManager
    {

        private const string CONST_VALIDATOR_PATH = "LCMSNet\\SampleValidators";
        private CompositionContainer mmef_container;
        private DirectoryCatalog mmef_directorycatalog;
        private static classSampleValidatorManager m_instance;

        public classSampleValidatorManager()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof(classSampleValidatorManager).Assembly));
            string validatorPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            validatorPath = System.IO.Path.Combine(validatorPath, CONST_VALIDATOR_PATH);
            mmef_directorycatalog = new DirectoryCatalog(validatorPath);
            catalog.Catalogs.Add(mmef_directorycatalog);
            this.mmef_container = new CompositionContainer(catalog);
            this.mmef_container.ComposeParts(this);
            System.Diagnostics.Debug.WriteLine(string.Format("Loaded : {0} sample validators", Validators.Count()));
        }

        public classSampleValidatorManager Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new classSampleValidatorManager();
                }
                return m_instance;
            }
        }

        [ImportMany]
        public IEnumerable<Lazy<ISampleValidator, ISampleValidatorMetaData>> Validators
        {
            get;
            set;
        }
    }
}
