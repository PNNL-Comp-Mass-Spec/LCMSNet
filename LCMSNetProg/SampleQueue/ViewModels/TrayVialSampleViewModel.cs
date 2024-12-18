﻿using System;
using System.Collections.Generic;
using LcmsNet.Data;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class TrayVialSampleViewModel : ReactiveObject
    {
        public TrayVialSampleViewModel(SampleData sample, List<string> trayNames)
        {
            this.Sample = sample;
            this.trayNames = trayNames;
            Tray = trayNames.IndexOf(sample.PAL.PALTray) + 1;
            Vial = sample.PAL.Well;
            this.WhenAnyValue(x => x.Tray, x => x.Vial).Subscribe(x => dataChanged = true);
            dataChanged = false;
        }

        private readonly List<string> trayNames;
        private int tray;
        private int vial;
        private bool dataChanged;

        public SampleData Sample { get; }

        public int ColumnId => Sample.ColumnIndex + 1;

        public int Tray
        {
            get => tray;
            set => this.RaiseAndSetIfChanged(ref tray, value);
        }

        public int Vial
        {
            get => vial;
            set => this.RaiseAndSetIfChanged(ref vial, value);
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
