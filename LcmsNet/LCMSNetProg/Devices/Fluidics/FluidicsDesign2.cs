using System.Drawing;
using System.Windows.Forms;
using LcmsNet.Devices.Fluidics.ViewModels;

namespace LcmsNet.Devices.Fluidics
{
    public partial class FluidicsDesign2 : Form
    {
        private FluidicsDesignViewModel fluidicsVm;

        public FluidicsDesign2()
        {
            InitializeComponent();

            fluidicsVm = new FluidicsDesignViewModel();
            fluidicsDesignView.DataContext = fluidicsVm;
        }

        /// <summary>
        ///  save the current fluidics design as a bitmap
        /// </summary>
        /// <returns>the image of the current fluidics design</returns>
        public Bitmap GetImage()
        {
            return fluidicsVm.GetImage();
        }

        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration()
        {
            fluidicsVm.SaveConfiguration();
        }

        /// <summary>
        /// Saves the hardware configuration to the path.
        /// </summary>
        public void SaveConfiguration(string path)
        {
            fluidicsVm.SaveConfiguration(path);
        }

        /// <summary>
        /// Loads the default hardware configuration.
        /// </summary>
        public void LoadConfiguration()
        {
            fluidicsVm.LoadConfiguration();
        }

        /// <summary>
        /// Loads the hardware configuration from file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfiguration(string path)
        {
            fluidicsVm.LoadConfiguration(path);
        }
    }
}