using System.Collections.Generic;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    public static class LCMethodBuilder
    {
        public static LCMethod BuildMethod(List<LCEvent> stageEvents)
        {
            //
            // Copy???
            //
            var events = new List<LCEvent>();
            events.AddRange(stageEvents);

            if (events.Count == 0)
                return null;

            var firstEvent = events[0];
            var startTime = firstEvent.Start;

            //
            // Construct the method and set the start time.
            //     The duration is auto-calculated by the method.
            //
            var method = new LCMethod();
            method.Events = events;
            method.SetStartTime(firstEvent.Start);

            return method;
        }
    }
}