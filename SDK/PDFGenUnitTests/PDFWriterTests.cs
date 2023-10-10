using System;
using System.IO;
using NUnit.Framework;
using PDFGenerator.Core.Model;
using PDFGenerator.PDFSharp;

namespace PDFGenUnitTests
{
    [TestFixture]
    class PDFWriterTests
    {
        Document doc;
        string basePath;
        string path;
        string picturePath;

        [SetUp]
        public void Init()
        {
            doc = new Document();
            doc.Date = DateTime.Now;
            doc.Version = "Not Versioned";
            doc.DocumentWriter = new PDFWriter();
            basePath = PathUtils.TestOutputPath;
            path = string.Empty;
            picturePath = Path.Combine(PathUtils.TestFilesPath, "testbitmap.bmp");
        }

        [Test]
        public void TestWriteHeader()
        {
            path = basePath + "\\headertest.pdf";
            doc.Title = "headertest";
            doc.AddHeader(HeaderLevel.H1, "A Header");
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
            var image = new ImageContent(picturePath);
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
            var image = new ImageContent(picturePath);
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
            var image = new ImageContent();
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
            doc.AddHeader(HeaderLevel.H1, "Doc Header");
            var image = new ImageContent(picturePath);
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