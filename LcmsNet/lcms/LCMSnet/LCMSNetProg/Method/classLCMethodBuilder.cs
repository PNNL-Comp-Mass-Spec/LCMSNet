using System;
using System.Collections.Generic;

using LcmsNetDataClasses.Method;

namespace LcmsNet.Method
{
    public static class classLCMethodBuilder
    {
        public static classLCMethod BuildMethod(List<classLCEvent> stageEvents)
        {
            /// 
            /// Copy???
            /// 
            List<classLCEvent> events = new List<classLCEvent>();            
            events.AddRange(stageEvents);

            if (events.Count == 0)
                return null;

            classLCEvent firstEvent = events[0];
            DateTime startTime      = firstEvent.Start;

            /// 
            /// Construct the method and set the start time.           
            ///     The duration is auto-calculated by the method.
            /// 
            classLCMethod method = new classLCMethod();
            method.Events        = events;
            method.SetStartTime(firstEvent.Start);

            return method;
        }
    }
}
