using System.Collections.Generic;
using LcmsNetDataClasses.Method;

namespace LcmsNet.Method
{
    public static class classLCMethodBuilder
    {
        public static classLCMethod BuildMethod(List<classLCEvent> stageEvents)
        {
            //
            // Copy???
            //
            var events = new List<classLCEvent>();
            events.AddRange(stageEvents);

            if (events.Count == 0)
                return null;

            var firstEvent = events[0];
            var startTime = firstEvent.Start;

            //
            // Construct the method and set the start time.
            //     The duration is auto-calculated by the method.
            //
            var method = new classLCMethod();
            method.Events = events;
            method.SetStartTime(firstEvent.Start);

            return method;
        }
    }
}