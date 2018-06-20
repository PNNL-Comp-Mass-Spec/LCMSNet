using System;
using System.Windows.Media.Imaging;
using NUnit.Framework;

namespace DocumentGeneratorTests
{
    [TestFixture]
    class ModelTests
    {
        string picturePath;
        [SetUp]
        public void init()
        {
            picturePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\testbitmap.bmp";
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
