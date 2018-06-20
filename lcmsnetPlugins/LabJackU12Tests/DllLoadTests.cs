using System;
using LabJack;
using NUnit.Framework;

namespace LabJackU12Tests
{
    [TestFixture]
    public class DllLoadTests
    {
        [Test]
        public void TestDll()
        {
            Console.WriteLine("Driver Version: {0}", LabJackU12Wrapper.GetDriverVersion());
        }
    }
}
