using System;
using System.IO;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using PDFGenerator.Core.Model;

namespace PDFGenUnitTests
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
            ImageContent testContent = new ImageContent(test);
        }

        [Test]
        public void TestLoadFromFile()
        {
            ImageContent testContent = new ImageContent(picturePath);
        }
    }
}
