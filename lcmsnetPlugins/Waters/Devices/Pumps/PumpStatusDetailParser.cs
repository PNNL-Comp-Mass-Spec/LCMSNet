using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Waters.Devices.Pumps
{
    public class PumpStatusDetailParser
    {
        /// <summary>
        /// Parses the data and returns status information from the status detail.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public PumpStatusInfo ParseData(string xml)
        {
            PumpStatusInfo info = new PumpStatusInfo();

            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);

                XmlNode statusRoot   = document.SelectSingleNode("Status");
                XmlNode root         = statusRoot.SelectSingleNode("nAcquityBSMStatusDetail");
                
                XmlNode ledState     = root.SelectSingleNode("LedState");
                XmlNode pressure     = root.SelectSingleNode("Pressure");
                XmlNode compositionB = root.SelectSingleNode("CompositionB");
                XmlNode flow         = root.SelectSingleNode("Flow");
                XmlNode status       = root.SelectSingleNode("State");

                info.Status         = status.InnerText;
                info.FlowrateB      = Convert.ToDouble(flow.InnerText);
                info.CompositionB   = Convert.ToDouble(compositionB.InnerText);
                info.LedStatus      = ledState.InnerText;
                info.Pressure       = Convert.ToDouble(pressure.InnerText);
            }
            catch 
            { 
            }
            return info;
        }
    }

    public class PumpStatusInfo
    {
        public PumpStatusInfo()
        {
            LedStatus    = "";
            Pressure     = 0;
            FlowrateA    = 0;
            FlowrateB    = 0;
            CompositionB = 0;
            Status = "";
        }
        public string Status { get; set; }
        public string LedStatus { get; set; }
        public double Pressure { get; set; }
        public double FlowrateA { get; set; }
        public double FlowrateB { get; set; }
        public double CompositionB{get;set;}
    }
}
