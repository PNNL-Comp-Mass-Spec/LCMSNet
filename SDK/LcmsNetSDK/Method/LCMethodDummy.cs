using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Method
{
    public class LCMethodDummy : ILCMethod
    {
        public string Name { get; set; }
        public List<LCEventDummy> Events { get; } = new List<LCEventDummy>();
        IReadOnlyList<ILCEvent> ILCMethod.Events => Events;
        public DateTime Start { get; private set; }
        public DateTime End { get; set; }
        public int Column { get; set; }

        public object Clone()
        {
            var other = new LCMethodDummy
            {
                Name = Name,
                Start = Start,
                End = End,
                Column = Column,
            };

            other.Events.AddRange(Events);

            return other;
        }
    }
}
