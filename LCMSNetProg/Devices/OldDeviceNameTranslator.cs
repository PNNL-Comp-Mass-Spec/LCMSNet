using System.Collections.Generic;
using System.Linq;

namespace LcmsNet.Devices
{
    /// <summary>
    /// Class for translating devices from old namespaces and hungarian-notation names to current names
    /// </summary>
    public static class OldDeviceNameTranslator
    {
        private static readonly Dictionary<string, string> FullyQualifiedNameMapper = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> FullyQualifiedLowerNameMapper = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> LowerNameMapper = new Dictionary<string, string>();

        static OldDeviceNameTranslator()
        {
            CreateMappers();
        }

        private static void CreateMappers()
        {
            FullyQualifiedNameMapper.Add("ASIpump.AsiPump", "LcmsNetPlugins.ASIpump.AsiPump");
            FullyQualifiedNameMapper.Add("ASUTGen.Devices.Pumps.IDEXPump", "LcmsNetPlugins.IDEX.Pumps.IDEXPump");
            FullyQualifiedNameMapper.Add("ASUTGen.Devices.Valves.IDEXValve", "LcmsNetPlugins.IDEX.Valves.IDEXValve");
            FullyQualifiedNameMapper.Add("Agilent.Devices.Pumps.classPumpAgilent", "LcmsNetPlugins.Agilent.Pumps.AgilentPump");
            FullyQualifiedNameMapper.Add("Agilent.Devices.Pumps.enumPumpAgilentModes", "LcmsNetPlugins.Agilent.Pumps.AgilentPumpModes");
            FullyQualifiedNameMapper.Add("Agilent.Devices.Pumps.enumPurgePumpChannel", "LcmsNetPlugins.Agilent.Pumps.PumpPurgeChannel");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoClosure", "DemoPluginLibrary.DemoClosure");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoPAL", "DemoPluginLibrary.DemoPAL");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoPump", "DemoPluginLibrary.DemoPump");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoSPE", "DemoPluginLibrary.DemoSPE");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoSprayNeedle", "DemoPluginLibrary.DemoSprayNeedle");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoTee", "DemoPluginLibrary.DemoTee");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoUnion", "DemoPluginLibrary.DemoUnion");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoValve", "DemoPluginLibrary.DemoValve");
            //FullyQualifiedNameMapper.Add("DemoPluginLibrary.DemoValve2", "DemoPluginLibrary.DemoValve2");
            FullyQualifiedNameMapper.Add("Eksigent.Devices.Pumps.EksigentPump", "LcmsNetPlugins.Eksigent.Pumps.EksigentPump");
            FullyQualifiedNameMapper.Add("FailureInjector.Drivers.NotificationDriver", "LcmsNetPlugins.FailureInjector.Drivers.NotificationDriver");
            //FullyQualifiedNameMapper.Add("FluidicsPack.FluidicsColumn", "FluidicsPack.FluidicsColumn");
            //FullyQualifiedNameMapper.Add("FluidicsPack.FluidicsComponentBase", "FluidicsPack.FluidicsComponentBase");
            //FullyQualifiedNameMapper.Add("FluidicsPack.SprayNeedle", "FluidicsPack.SprayNeedle");
            //FullyQualifiedNameMapper.Add("FluidicsPack.Tee", "FluidicsPack.Tee");
            //FullyQualifiedNameMapper.Add("FluidicsPack.Union", "FluidicsPack.Union");
            //FullyQualifiedNameMapper.Add("FluidicsPack.WasteComponent", "FluidicsPack.WasteComponent");
            FullyQualifiedNameMapper.Add("FluidicsSDK.Base.EightPositionState", "LcmsNetPlugins.VICI.Valves.EightPositionState");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.BrukerStart.classBrukerStart", "LcmsNetPlugins.Bruker.BrukerStart");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.classContactClosureU12", "LcmsNetPlugins.LabJackU12.ContactClosure.ContactClosureU12");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosureRead.ContactClosureReadU12", "LcmsNetPlugins.LabJackU12.ContactClosureRead.ContactClosureReadU12");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosureRead.ContactClosureReadU12+ContactClosureState", "LcmsNetPlugins.LabJackU12.ContactClosureRead.ContactClosureReadU12+ContactClosureState");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU12Ports", "LcmsNetPlugins.LabJackU12.LabjackU12Ports");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU12InputPorts", "LcmsNetPlugins.LabJackU12.LabjackU12InPorts");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU12OutputPorts", "LcmsNetPlugins.LabJackU12.LabjackU12OutputPorts");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.classContactClosureU3", "LcmsNetPlugins.LabJackU3.ContactClosureU3");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU3Ports", "LcmsNetPlugins.LabJackU3.LabjackU3Ports");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU3InputPorts", "LcmsNetPlugins.LabJackU3.LabjackU3InPorts");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.ContactClosure.enumLabjackU3OutputPorts", "LcmsNetPlugins.LabJackU3.LabjackU3OutputPorts");

            FullyQualifiedNameMapper.Add("LcmsNetPlugins.PNNLDevices.ContactClosure.ContactClosureU12", "LcmsNetPlugins.LabJackU12.ContactClosure.ContactClosureU12");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.PNNLDevices.ContactClosureRead.ContactClosureReadU12", "LcmsNetPlugins.LabJackU12.ContactClosureRead.ContactClosureReadU12");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.PNNLDevices.ContactClosureRead.ContactClosureReadU12+ContactClosureState", "LcmsNetPlugins.LabJackU12.ContactClosureRead.ContactClosureReadU12+ContactClosureState");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.LabJack.LabjackU12Ports", "LcmsNetPlugins.LabJackU12.LabjackU12Ports");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.LabJack.LabjackU12InPorts", "LcmsNetPlugins.LabJackU12.LabjackU12InPorts");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.LabJack.LabjackU12OutputPorts", "LcmsNetPlugins.LabJackU12.LabjackU12OutputPorts");

            FullyQualifiedNameMapper.Add("LcmsNet.Devices.NetworkStart.Socket.classNetStartSocket", "LcmsNetPlugins.PNNLDevices.NetworkStart.Socket.NetStartSocket");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Pal.classPal", "LcmsNetPlugins.PALAutoSampler.Pal.Pal");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Pumps.classPumpIsco", "LcmsNetPlugins.Teledyne.Pumps.IscoPump");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Pumps.enumIscoOperationMode", "LcmsNetPlugins.Teledyne.Pumps.IscoOperationMode");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Pumps.enumISCOPumpChannels", "LcmsNetPlugins.Teledyne.Pumps.ISCOPumpChannels");

            // VICI Plugin: v4.0 to v5.2 refactor names
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICIBase", "LcmsNetPlugins.VICI.Valves.ValveVICIBase");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICI2Pos", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICI2pos4port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos04Port");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICI2Pos6Port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos06Port");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICI2pos10port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos10Port");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos4port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos04Port");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos6Port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos06Port");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos10port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos10Port");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICISPE", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICISPE");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICISixPortInjection", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICISixPortInjection");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICIMultiPos", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICIMultiPos");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICIMultipos9Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI08Position");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICIMultipos11Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI10Position");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.Valves.classValveVICIMultipos16Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI15Position");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICIMultiPos9Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI08Position");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICIMultiPos11Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI10Position");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICIMultiPos16Port", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI15Position");

            // VICI plugin: v5.0 to v5.2 refactor
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos04port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos04Port");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI2Pos06Port", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICI2Pos06Port");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICISPE", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICISPE");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICISixPortInjection", "LcmsNetPlugins.VICI.Valves.TwoPosition.ValveVICISixPortInjection");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI08Position", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI08Position");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI10Position", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI10Position");
            FullyQualifiedNameMapper.Add("LcmsNetPlugins.VICI.Valves.ValveVICI15Position", "LcmsNetPlugins.VICI.Valves.MultiPosition.ValveVICI15Position");

            FullyQualifiedNameMapper.Add("LcmsNet.Devices.classApplicationDevice", "LcmsNet.Devices.ApplicationDevice");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.classBlockDevice", "LcmsNet.Devices.BlockDevice");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.classErrorDevice", "LcmsNet.Devices.ErrorDevice");
            FullyQualifiedNameMapper.Add("LcmsNet.Devices.classLogDevice", "LcmsNet.Devices.LogDevice");
            FullyQualifiedNameMapper.Add("LcmsNetDataClasses.Devices.classTimerDevice", "LcmsNetSDK.Devices.TimerDevice");
            FullyQualifiedNameMapper.Add("Newport.ESP300.classNewportStage", "LcmsNetPlugins.Newport.ESP300.NewportStage");
            FullyQualifiedNameMapper.Add("LcmsNetDataClasses.classSampleData", "LcmsNetSDK.Data.ISampleInfo");
            FullyQualifiedNameMapper.Add("LcmsNetSDK.Data.SampleData", "LcmsNetSDK.Data.ISampleInfo");

            foreach (var map in FullyQualifiedNameMapper)
            {
                var lower = map.Key.ToLower();
                FullyQualifiedLowerNameMapper.Add(lower, map.Value);
                var name = lower.Split('.').Last();
                if (!LowerNameMapper.ContainsKey(name))
                {
                    LowerNameMapper.Add(name, map.Value);
                }
            }
        }

        /// <summary>
        /// Translate the old device name to the current device name
        /// </summary>
        /// <param name="oldDeviceFullName">device name, fully qualified</param>
        /// <returns>new device name, if the name was matched to an old name; otherwise, returns <param name="oldDeviceFullName"></param></returns>
        public static string TranslateOldDeviceFullName(string oldDeviceFullName)
        {
            if (string.IsNullOrWhiteSpace(oldDeviceFullName))
            {
                return oldDeviceFullName;
            }

            if (FullyQualifiedNameMapper.TryGetValue(oldDeviceFullName, out var fqCased))
            {
                return fqCased;
            }

            var lower = oldDeviceFullName.ToLower();
            if (FullyQualifiedLowerNameMapper.TryGetValue(lower, out var fqUncased))
            {
                return fqUncased;
            }

            var name = lower.Split('.').Last();
            if (LowerNameMapper.TryGetValue(name, out var nameUncased))
            {
                return nameUncased;
            }

            return oldDeviceFullName;
        }
    }
}
