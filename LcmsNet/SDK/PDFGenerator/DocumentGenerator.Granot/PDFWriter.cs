using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using EMSL.DocumentGenerator.Core;
using EMSL.DocumentGenerator.Core.Services;

using PdfFileWriter;


namespace EMSL.DocumentGenerator.Granot
{
	public class PDFWriter
		: IDocumentWriter
	{
		public void SaveDocument(string path, Document document, Core.Model.DocumentContent[] documentContents, bool overwrite = true, bool titlePage = false)
		{
			PdfDocument pdfDoc = new PdfDocument(8.5, 11.0, UnitOfMeasure.Inch);

			CreateTitlePage(document, pdfDoc);

			pdfDoc.CreateFile(path);
		}

		private void CreateTitlePage(Document document, PdfDocument pdfDoc)
		{
			// Can't underline font, this is a limit with the writer we are using.
			PdfFont titleFont = new PdfFont(
				pdfDoc,
				document.TitleFont,
				FontStyle.Regular,
				true);

			PdfPage frontPage = new PdfPage(pdfDoc);
			PdfContents pageContents = new PdfContents(frontPage);

			pageContents.DrawText(titleFont, document.TitleFontSize, document.Title.Replace('\n', ' '));
			pageContents.SaveGraphicsState();
		}
	}
}
