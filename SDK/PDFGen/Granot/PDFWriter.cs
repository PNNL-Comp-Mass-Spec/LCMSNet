using System;
using PDFGenerator.Core.Model;
using PDFGenerator.Core.Services;

// Requires the PdfFileWriter Nuget package by Uzi Granot
// This also depends on System.Drawing.FontStyle, and was never completed
//using PdfFileWriter;

namespace PDFGenerator.Granot
{
    [Obsolete("Code is incomplete", true)]
    public class PDFWriter
        : IDocumentWriter
    {
        public void SaveDocument(string path, Document document, DocumentContent[] documentContents, bool overwrite = true, bool titlePage = false)
        {
            //PdfDocument pdfDoc = new PdfDocument(8.5, 11.0, UnitOfMeasure.Inch, path);
            //
            //CreateTitlePage(document, pdfDoc);
            //
            //pdfDoc.CreateFile();
        }

        //private void CreateTitlePage(Document document, PdfDocument pdfDoc)
        //{
        //    // Can't underline font, this is a limit with the writer we are using.
        //    PdfFont titleFont = PdfFont.CreatePdfFont(
        //        pdfDoc,
        //        document.TitleFont,
        //        FontStyle.Regular,
        //        true);
        //
        //    PdfPage frontPage = new PdfPage(pdfDoc);
        //    PdfContents pageContents = new PdfContents(frontPage);
        //
        //    pageContents.DrawText(titleFont, document.TitleFontSize, document.Title.Replace('\n', ' '));
        //    pageContents.SaveGraphicsState();
        //}
    }
}
