using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASIpump;
using ReactiveUI;

namespace ASIpumpTest
{
    public class AsiTestViewModel : ReactiveObject
    {
        public AsiTestViewModel()
        {
            asiUIVM = new AsiUIViewModel();
            var pump = new ASIpump.AsiPump();
            pump.PortName = "COM3";

            asiUIVM.Pump = pump;
        }

        private readonly AsiUIViewModel asiUIVM;

        public AsiUIViewModel AsiUIVM => asiUIVM;
    }
}
