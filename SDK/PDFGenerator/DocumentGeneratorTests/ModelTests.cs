using System;
using System.IO;
using System.Windows.Media.Imaging;
using NUnit.Framework;

namespace DocumentGeneratorTests
{
    [TestFixture]
    class ModelTests
    {
        string picturePath;
        [SetUp]
        public void Init()
        {
            picturePath = Path.Combine(PathUtils.TestFilesPath, "testbitmap.bmp");
        }

        [Test]
        public void TestFromImage()
        {
            var test = new BitmapImage(new Uri(picturePath));
            EMSL.DocumentGenerator.Core.Model.ImageContent testContent = new EMSL.DocumentGenerator.Core.Model.ImageContent(test);
        }

        [Test]
        public void TestLoadFromFile()
        {
            EMSL.DocumentGenerator.Core.Model.ImageContent testContent = new EMSL.DocumentGenerator.Core.Model.ImageContent(picturePath);
        }
    }
}
