using LcmsNet.IO.DMS.Data;
using LcmsNetData.Data;
using NUnit.Framework;

namespace LcmsnetUnitTest
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        [TestCase("A1", 1)]
        [TestCase("H1", 85)] // 96-well: 8x12
        [TestCase("H12", 96)]
        [TestCase("Z12", 312)] // random tests
        [TestCase("AA1", 313)]
        [TestCase("AA2", 314)]
        [TestCase("ZZ12", 8424)]
        [TestCase("A24", 24, 24)] // 384-well: 16x24
        [TestCase("P1", 361, 24)]
        [TestCase("P24", 384, 24)]
        [TestCase("A1", 1, 8)] // 48-well: 6x8
        [TestCase("A8", 8, 8)]
        [TestCase("F1", 41, 8)]
        [TestCase("F8", 48, 8)]
        [TestCase("A01", 1, 48)] // 1536-well: 32x48
        [TestCase("A48", 48, 48)]
        [TestCase("AF01", 1489, 48)]
        [TestCase("AF48", 1536, 48)]
        [TestCase("A01", 1, 72)] // 3456-well: 48x72
        [TestCase("A72", 72, 72)]
        [TestCase("AV01", 3385, 72)]
        [TestCase("AV72", 3456, 72)]
        [TestCase("AB1C", 9999)] // Errors
        [TestCase("ABC11", 9999)]
        [TestCase("A15", 15)] // Should probably be an error
        public void TestConvertVialToInt(string position, int expectedResult, byte columnsPerRow = 12)
        {
            var intPosition = ConvertVialPosition.ConvertVialToInt(position, columnsPerRow);
            Assert.AreEqual(expectedResult, intPosition);
        }
    }
}
