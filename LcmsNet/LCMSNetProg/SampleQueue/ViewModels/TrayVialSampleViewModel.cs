using System;
using System.Collections.Generic;
using LcmsNetDataClasses;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class TrayVialSampleViewModel : ReactiveObject
    {
        public TrayVialSampleViewModel(classSampleData sample, List<string> trayNames)
        {
            this.sample = sample;
            this.trayNames = trayNames;
            Tray = trayNames.IndexOf(sample.PAL.PALTray) + 1;
            Vial = sample.PAL.Well;
            this.WhenAnyValue(x => x.Tray, x => x.Vial).Subscribe(x => dataChanged = true);
            dataChanged = false;
        }

        private readonly List<string> trayNames;
        private readonly classSampleData sample;
        private int tray;
        private int vial;
        private bool dataChanged;

        public classSampleData Sample => sample;

        public int ColumnId => Sample.ColumnData.ID + 1;

        public int Tray
        {
            get { return tray; }
            set { this.RaiseAndSetIfChanged(ref tray, value); }
        }

        public int Vial
        {
            get { return vial; }
            set { this.RaiseAndSetIfChanged(ref vial, value); }
        }

        public void StoreTrayVialToSample()
        {
            if (!dataChanged)
            {
                return;
            }
            if (Tray == 0)
            {
                Sample.PAL.PALTray = null;
            }
            else
            {
                Sample.PAL.PALTray = trayNames[Tray - 1];
            }
            Sample.PAL.Well = Vial;
        }
    }
}
