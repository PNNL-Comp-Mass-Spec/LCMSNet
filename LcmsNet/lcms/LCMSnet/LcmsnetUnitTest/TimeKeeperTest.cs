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
        public void DoDatesNotSpanTransition()
        {
            //These two dates do not span a daylight savings transition, as they are both after the fall transition and before the spring transition.
            var start = new DateTime(2014, 11, 9);
            var end = new DateTime(2014, 11, 10);
            NUnit.Framework.Assert.IsFalse(TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(start, end));
        }

        [Test]
        public void DoDatesSpanTransition()
        {
            //These two dates span a daylight savings transition as they are one second before, and one second after the switch.
            var start = new DateTime(2014, 11, 2, 1, 59, 59);
            var end = new DateTime(2014, 11, 2, 2, 0, 1);
            NUnit.Framework.Assert.IsTrue(TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(start, end));
        }

        [Test]
        public void MatchAustralianEasternTimeZoneViaConversionMethod()
        {
            var pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var aet = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            TimeKeeper.Instance.TimeZone = pst;
            var now = DateTime.UtcNow;
            var timeKeeperTime = TimeKeeper.Instance.Now;
            var nowAet = TimeZoneInfo.ConvertTimeFromUtc(now, aet);
            // timekeeper time should be waaay different from aet time as it's set to pst.
            // but mathematically, they would be the same(or close to the same) so we check the strings
            // to see if they actually are different
            NUnit.Framework.Assert.IsFalse(nowAet.ToString() == timeKeeperTime.ToString());
            TimeKeeper.Instance.TimeZone = aet;
            NUnit.Framework.Assert.IsTrue(nowAet.ToString() ==
                                          TimeKeeper.Instance.ConvertToTimeZone(timeKeeperTime,
                                              "AUS Eastern Standard Time").ToString());
        }

        [Test]
        public void MatchPST()
        {
            var pst = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var now = DateTime.UtcNow;
            TimeKeeper.Instance.TimeZone = pst;
            var timeKeeperTime = TimeKeeper.Instance.Now;
            var nowPst = TimeZoneInfo.ConvertTimeFromUtc(now, pst);
            NUnit.Framework.Assert.IsTrue(nowPst.ToString() == timeKeeperTime.ToString());
        }
    }
}