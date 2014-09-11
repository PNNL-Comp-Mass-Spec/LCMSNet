using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LcmsNetSDK;

namespace LcmsnetUnitTest
{
    [TestFixture]
    public class TimeKeeperUnitTests
    {
        [Test]
        public void MatchPST()
        {
            TimeZoneInfo pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime now = DateTime.UtcNow;
            TimeKeeper.Instance.TimeZone = pst;
            DateTime timeKeeperTime = TimeKeeper.Instance.Now;
            DateTime nowPst = TimeZoneInfo.ConvertTimeFromUtc(now, pst);
            NUnit.Framework.Assert.IsTrue(nowPst.ToString() == timeKeeperTime.ToString());            
        }

        [Test]
        public void MatchAustralianEasternTimeZoneViaConversionMethod()
        {
            TimeZoneInfo pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo aet = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            TimeKeeper.Instance.TimeZone = pst;
            DateTime now = DateTime.UtcNow;            
            DateTime timeKeeperTime = TimeKeeper.Instance.Now; 
            DateTime nowAet = TimeZoneInfo.ConvertTimeFromUtc(now, aet);
            // timekeeper time should be waaay different from aet time as it's set to pst. 
            // but mathematically, they would be the same(or close to the same) so we check the strings
            // to see if they actually are different
            NUnit.Framework.Assert.IsFalse(nowAet.ToString() == timeKeeperTime.ToString());
            TimeKeeper.Instance.TimeZone = aet;            
            NUnit.Framework.Assert.IsTrue(nowAet.ToString() == TimeKeeper.Instance.ConvertToTimeZone(timeKeeperTime, "AUS Eastern Standard Time").ToString());
        }

        [Test]
        public void DoDatesSpanTransition()
        {
            //These two dates span a daylight savings transition as they are one second before, and one second after the switch.
            DateTime start = new DateTime(2014, 11, 2, 1, 59, 59);
            DateTime end = new DateTime(2014, 11, 2, 2, 0, 1);
            NUnit.Framework.Assert.IsTrue(TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(start, end));
        }

        [Test]
        public void DoDatesNotSpanTransition()
        {
            //These two dates do not span a daylight savings transition, as they are both after the fall transition and before the spring transition.
            DateTime start = new DateTime(2014, 11, 9);
            DateTime end = new DateTime(2014, 11, 10);
            NUnit.Framework.Assert.IsFalse(TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(start, end));
        }



    }
}
