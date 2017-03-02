using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using Core = EMSL.DocumentGenerator.Core;


namespace EMSL.DocumentGenerator.Core.Tests
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
            Bitmap test = new Bitmap(picturePath);
            Core.Model.ImageContent testContent = new Core.Model.ImageContent(test);
        }

        [Test]
        public void TestLoadFromFile()
        {
            Core.Model.ImageContent testContent = new Core.Model.ImageContent(picturePath);
        } 
    }
}
