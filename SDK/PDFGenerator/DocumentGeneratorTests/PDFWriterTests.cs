﻿using System;
using System.IO;
using NUnit.Framework;

namespace DocumentGeneratorTests
{
    [TestFixture]
    class PDFWriterTests
    {
        EMSL.DocumentGenerator.Core.Document doc;
        string basePath;
        string path;
        string picturePath;

        [SetUp]
        public void Init()
        {
            doc = new EMSL.DocumentGenerator.Core.Document();
            doc.Date = DateTime.Now;
            doc.Version = "Not Versioned";
            doc.DocumentWriter = new EMSL.DocumentGenerator.PDFSharp.PDFWriter();
            basePath = PathUtils.TestOutputPath;
            path = string.Empty;
            picturePath = Path.Combine(PathUtils.TestFilesPath, "testbitmap.bmp");
        }

        [Test]
        public void TestWriteHeader()
        {
            path = basePath + "\\headertest.pdf";
            doc.Title = "headertest";
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "A Header");
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }

        [Test]
        public void TestWriteText()
        {
            path = basePath + "\\writetest.pdf";
            doc.Title = "writetest";
            doc.AddParagraph("This is a test file.");
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }

        [Test]
        public void TestDrawImageWithoutCaption()
        {
            doc.Title = "drawtestwithoutcaption";
            EMSL.DocumentGenerator.Core.Model.ImageContent image = new EMSL.DocumentGenerator.Core.Model.ImageContent(picturePath);
            doc.AddImage(image);
            path = basePath + "\\drawtest1.pdf";
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }

        [Test]
        public void TestDrawImageWithCaption()
        {
            doc.Title = "drawtestwithcaption";
            EMSL.DocumentGenerator.Core.Model.ImageContent image = new EMSL.DocumentGenerator.Core.Model.ImageContent(picturePath);
            image.CaptionText = "Figure 1: Random Test Image.";
            doc.AddImage(image);
            path = basePath + "\\drawtest2.pdf";
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }

        [Test]
        public void TestDrawImageNullImage()
        {
            var ex = Assert.Throws<NullReferenceException>(() => DrawImageNullImage());
            Assert.That(ex.Message.Equals("Image reference is null."));
        }

        private void DrawImageNullImage()
        {
            doc.Title = "drawtestInvalidImage";
            EMSL.DocumentGenerator.Core.Model.ImageContent image = new EMSL.DocumentGenerator.Core.Model.ImageContent();
            image.CaptionText = "Figure 1: Random Test Image.";
            doc.AddImage(image);
            path = basePath + "\\drawtest3.pdf";
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }


        [Test]
        public void TestMoveToNextPage()
        {
            path = basePath + "\\pgbrktest.pdf";
            doc.Title = "page break test";
            doc.WriteDocument(path);
            Assert.AreEqual(true, File.Exists(path));
            //File.Delete(path);
        }

        [Test]
        public void TestEntireDocWrite()
        {
            path = basePath + "\\wholedoctest.pdf";
            doc.Title = "wholedoctest";
            doc.Date = DateTime.Now;
            doc.DateFormatString = "MMMM dd, yyyy";
            doc.AddHeader(EMSL.DocumentGenerator.Core.Model.HeaderLevel.H1, "Doc Header");
            EMSL.DocumentGenerator.Core.Model.ImageContent image = new EMSL.DocumentGenerator.Core.Model.ImageContent(picturePath);
            doc.AddParagraph("This is a test document. We'll be back after the break!");
            image.CaptionText = "Figure 1: Random Test Image.";
            doc.AddImage(image);
            doc.AddPageBreak();
            doc.AddParagraph("And we're back!");
            doc.WriteDocument(path);
            //File.Delete(path);
        }

    }
}