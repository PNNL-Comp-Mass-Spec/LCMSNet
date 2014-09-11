using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMSL.DocumentGenerator.Core;
using EMSL.DocumentGenerator.Core.Model;
using EMSL.DocumentGenerator.Core.Services;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.Specialized;
using System.Windows;



namespace EMSL.DocumentGenerator.PDFSharp
{
	public class PDFWriter
		: IDocumentWriter
	{
		#region Attributes
        // by default PDFSharp uses "point" as the unit, for reference: 72pts = 1 inch as defined by the DTP(or PostScript) point. 
        // Originally, this margin was set to an odd 25pt or 0.375 inch, "standard" margins are usually 0.5 or 1 inch, aka,
        // 36 or 72 pt, so this was changed to 36pt. The library could be enhanced by moving the margin to the document 
        // model and letting the user control the margins through it, instead of using a constant
		private const double PAGE_MARGIN = 36;
		#endregion

        /// <summary>
        /// Save the document to a file
        /// </summary>
        /// <param name="path">the file path</param>
        /// <param name="document">the document model to save</param>
        /// <param name="documentContents">an array of contents of the document model</param>
        /// <param name="overwrite">determines if the file should be overwritten or not</param>
        /// <param name="titlePage">determines if a title page should be written or not</param>
		public void SaveDocument(string path, Document document, DocumentContent[] documentContents, bool overwrite = true, bool titlePage = false)
		{
			PdfDocument pSharpDoc = new PdfDocument();
			pSharpDoc.Info.Title = document.Title;

            if (titlePage)
            {
                WriteTitlePage(document, pSharpDoc);
            }
			WriteContent(document, pSharpDoc, documentContents);

			pSharpDoc.Save(path);
		}

        /// <summary>
        /// write the title page of the document to file
        /// </summary>
        /// <param name="document">the document whose title page to write</param>
        /// <param name="pSharpDoc">the pdfsharp library document to write to</param>
		private void WriteTitlePage(Document document, PdfDocument pSharpDoc)
		{
			XFont titleFont = new XFont(
				document.TitleFont,
				document.TitleFontSize,
				document.UnderlinTitle ? XFontStyle.Underline : XFontStyle.Regular);
			PdfPage frontPage = pSharpDoc.AddPage();
			XGraphics gfx = XGraphics.FromPdfPage(frontPage);
			XTextFormatter tf = new XTextFormatter(gfx);

			tf.Alignment = XParagraphAlignment.Center;

			XSize titleSize = gfx.MeasureString(document.Title, titleFont);
			double textHeight = FindTextHeight(document.Title, titleFont, gfx, frontPage.Width - (2 * PAGE_MARGIN));
			double y = Math.Max((frontPage.Height / 2) - textHeight, PAGE_MARGIN);

			tf.DrawString(
				document.Title,
				titleFont,
				XBrushes.Black,
				new XRect(
					PAGE_MARGIN,
					y,
					frontPage.Width - (2 * PAGE_MARGIN),
					textHeight + PAGE_MARGIN));

			if (document.Version != null)
			{
				if (document.UnderlinTitle)
					titleFont = new XFont(
						document.TitleFont,
						document.TitleFontSize - 2);

				tf.DrawString(
					document.Version,
					titleFont,
					XBrushes.DarkGray,
					new XRect(
						PAGE_MARGIN,
						frontPage.Height / 2 + PAGE_MARGIN,
						frontPage.Width - (2 * PAGE_MARGIN),
						frontPage.Height / 2));
			}

            if (document.Date != null)
            {
                string date = document.Date.ToString(document.DateFormatString);
                textHeight = FindTextHeight(date, titleFont, gfx, frontPage.Width - (2 * PAGE_MARGIN));
                if (document.UnderlinTitle)
                    titleFont = new XFont(
                        document.TitleFont,
                        document.TitleFontSize - 2);

                tf.DrawString(
                    date,
                    titleFont,
                    XBrushes.Black,
                    new XRect(
                        PAGE_MARGIN,
                        frontPage.Height / 2 + textHeight  + PAGE_MARGIN,
                        frontPage.Width - (2 * PAGE_MARGIN),
                        frontPage.Height / 2 + textHeight));
            }
		}

        /// <summary>
        /// Write document contents to file
        /// </summary>
        /// <param name="document">the document to write</param>
        /// <param name="pSharpDoc">the pdfsharp library document to write to</param>
        /// <param name="documentContents">the contents of the document to be written</param>
		private void WriteContent(Document document, PdfDocument pSharpDoc, DocumentContent[] documentContents)
		{
			XFont	font		= new XFont(document.Font, document.FontSize);
			XFont	captionFont = new XFont(document.Font, document.FontSize - 2, XFontStyle.Italic);

			Dictionary<HeaderLevel, XFont> headerFonts = new Dictionary<HeaderLevel, XFont>();
			headerFonts.Add(HeaderLevel.H1, new XFont(document.HeaderFont, document.GetHeaderFontSize(HeaderLevel.H1)));
			headerFonts.Add(HeaderLevel.H2, new XFont(document.HeaderFont, document.GetHeaderFontSize(HeaderLevel.H2)));
			headerFonts.Add(HeaderLevel.H3, new XFont(document.HeaderFont, document.GetHeaderFontSize(HeaderLevel.H3)));

			PdfPage			currentPage		= pSharpDoc.AddPage();
			double			currentHeight	= PAGE_MARGIN;
			XGraphics		gfx				= XGraphics.FromPdfPage(currentPage);
			XTextFormatter	tf				= new XTextFormatter(gfx);

			foreach (DocumentContent contentItem in documentContents)
			{
				switch (contentItem.ItemType)
				{
				case ItemType.HeaderItem:
					HeaderContent h = (HeaderContent) contentItem;
					WriteHeader(h, pSharpDoc, font, headerFonts, ref currentPage, ref currentHeight, ref gfx, ref tf);
					break;

				case ItemType.ImageItem:
					ImageContent i = (ImageContent) contentItem;
					DrawImage(i, pSharpDoc, captionFont, ref currentPage, ref currentHeight, ref gfx, ref tf);
					break;

				case ItemType.PageBreakItem:
					MoveToNextPage(pSharpDoc, ref currentPage, ref currentHeight, ref gfx, ref tf);
					break;

				case ItemType.ParagraphItem:
					ParagraphContent p = (ParagraphContent) contentItem;
					WriteText(p.Text, pSharpDoc, font, ref currentPage, ref currentHeight, ref gfx, ref tf);
					break;

				case ItemType.TableItem:
					break;

				default:
                    // getting to this point should be impossible. Except perhaps via memory corruption
                    throw new Core.Model.InvalidDocumentContent("Invalid Document Content.");				
				}
			}
		}

        /// <summary>
        /// Draw an image to the pdf file
        /// </summary>
        /// <param name="image">the image content to write</param>
        /// <param name="pSharpDoc">the pdfsharp library document to write to</param>
        /// <param name="captionFont">font to write the caption in</param>
        /// <param name="currentPage">current pdfsharp library document page</param>
        /// <param name="currentHeight">defines area of page already written to</param>
        /// <param name="gfx">pdfsharp library graphics object to draw with</param>
        /// <param name="tf">pdfsharp library text formatter object to write caption with</param>
		private void DrawImage(ImageContent image, PdfDocument pSharpDoc, XFont captionFont, ref PdfPage currentPage, 
			ref double currentHeight, ref XGraphics gfx, ref XTextFormatter tf)
		{
            //to draw an image, we must have an image...
            if (image.SourceImage == null)
                throw new NullReferenceException("Image reference is null.");
			XImage imageItem = null;            
            imageItem = XImage.FromBitmapSource(image.SourceImage);
            XSize pdfImageSize = imageItem.Size;
            //define where we'll draw the image
            XRect pdfImageRect = new XRect(new XPoint(PAGE_MARGIN, currentHeight), pdfImageSize); 
            //define the height of the caption text
            double textHeight = 0;
            
            if(image.CaptionText != null)
                textHeight = FindTextHeight(image.CaptionText, captionFont, gfx, currentPage.Width - 2 * PAGE_MARGIN);

            // if the image and text are larger in size than the drawable area of a single page, throw an error, else we end up in an infinite loop.
            if (pdfImageSize.Height + textHeight > currentPage.Height - (PAGE_MARGIN * 2))            
                throw new InvalidImageException("Image and caption do not fit on one page.");            

            //if the height of the image and text are too big to fit on the current page, start a new page and recalculate
           
            if (currentPage.Height - PAGE_MARGIN < currentHeight + pdfImageSize.Height + textHeight + PAGE_MARGIN)
            {
                MoveToNextPage(pSharpDoc, ref currentPage, ref currentHeight, ref gfx, ref tf);
                DrawImage(image, pSharpDoc, captionFont, ref currentPage, ref currentHeight, ref gfx, ref tf);
            }
            else
            {
                if (image.CaptionText != null)
                {
                    // draw caption and image in proper location, center caption text at top or bottom, centering does not work
                    // on current version of library
                    XStringFormat format = XStringFormats.TopLeft;
                    //format = XStringFormats.Center;                    
                    if (image.CaptionPlacement == CaptionPlacement.Top)
                    {
                        tf.DrawString(image.CaptionText, captionFont, XBrushes.Black, new XRect(PAGE_MARGIN, currentHeight, 
                            currentPage.Width - 2 * PAGE_MARGIN, textHeight), format);
                        pdfImageRect = new XRect(new XPoint(PAGE_MARGIN, currentHeight + textHeight), pdfImageSize);
                        gfx.DrawImage(imageItem, pdfImageRect);

                    }
                    else
                    {
                        gfx.DrawImage(imageItem, pdfImageRect);
                        tf.DrawString(image.CaptionText, captionFont, XBrushes.Black, new XRect(PAGE_MARGIN, currentHeight + pdfImageSize.Height,
                            currentPage.Width - 2 * PAGE_MARGIN, textHeight), format);
                    }                   
                }
                else
                {
                    gfx.DrawImage(imageItem, pdfImageRect);                    
                }
                //set the current height of the page, textHeight will be 0 if no caption text was specified, and so will have no effect on height.
                currentHeight += pdfImageSize.Height + textHeight; 
            }               
		}


        /// <summary>
        /// write header to the pdf file
        /// </summary>
        /// <param name="header">header content to write</param>
        /// <param name="pSharpDoc">pdfsharp library document to write to</param>
        /// <param name="headerFonts">fonts to write with</param>
        /// <param name="currentPage">current pdfsharp library page to write to</param>
        /// <param name="currentHeight">defines currently used area of the page</param>
        /// <param name="gfx">pdfsharp library graphics object</param>
        /// <param name="tf">pdfsharp library text formatter</param>
		private void WriteHeader(HeaderContent header, PdfDocument pSharpDoc, XFont font, Dictionary<HeaderLevel, XFont> headerFonts, 
			ref PdfPage currentPage, ref double currentHeight, ref XGraphics gfx, ref XTextFormatter tf)
		{
            XFont hFont = headerFonts[header.Level];
            // add line space equivalent to the size of the normal document font before the header
            if (currentHeight > 0)
            {
                currentHeight += FindTextHeight(header.Text, font, gfx, currentPage.Width - PAGE_MARGIN * 2);
            }
            WriteText(header.Text, pSharpDoc, hFont, ref currentPage, ref currentHeight, ref gfx, ref tf);      
		}

		private void WriteText(string text, PdfDocument pSharpDoc, XFont font, ref PdfPage currentPage, 
			ref double currentHeight, ref XGraphics gfx, ref XTextFormatter tf)
		{           
			if (string.IsNullOrEmpty(text))
				text = " ";

			double textHeight = FindTextHeight(
				text, 
				font, 
				gfx, 
				currentPage.Width - PAGE_MARGIN * 2);

			// If the paragraph doesn't fit on this page, move to the next page.
			if ((currentPage.Height - PAGE_MARGIN) < (currentHeight + textHeight + PAGE_MARGIN))
				MoveToNextPage(pSharpDoc, ref currentPage, ref currentHeight, ref gfx, ref tf);

            tf.DrawString(
                text,
                font,
                XBrushes.Black,
                new XRect(
                    PAGE_MARGIN,
                    currentHeight,
                    currentPage.Width - PAGE_MARGIN * 2,
                    textHeight));
            currentHeight += textHeight;             
		}

		// Needs improvement
        /// <summary>
        /// find the height dimension on the page that a string will take up.
        /// </summary>
        /// <param name="text">text whose height to find</param>
        /// <param name="textFont">font of the text</param>
        /// <param name="textDrawer">pdfsharp library graphics object</param>
        /// <param name="maxWidth">max width across page text is allowed to be, usually would be 
        /// page width - (2 * PAGE_MARGIN)</param>
        /// <returns></returns>
		private double FindTextHeight(string text, XFont textFont, XGraphics textDrawer, double maxWidth)
		{
			XSize textSize = textDrawer.MeasureString(text, textFont);

			if (textSize.Width < maxWidth)
				return textSize.Height;

            // height of font * number of rows, ok, but why maxwidth - 18?
			double height = textSize.Height * Math.Ceiling(textSize.Width / (maxWidth - 18));
			return height;
		}

        /// <summary>
        /// create new pdfhsarp library page
        /// </summary>
        /// <param name="pDoc">pdfsharp library document</param>
        /// <param name="currentPage">current page reference</param>
        /// <param name="currentHeight">current height reference</param>
        /// <param name="gfx">pdfsharp library graphics object reference</param>
        /// <param name="tf">pdfsharp library text formatter reference</param>
		private void MoveToNextPage(PdfDocument pDoc, ref PdfPage currentPage, ref double currentHeight, 
			ref XGraphics gfx, ref XTextFormatter tf)
		{
            //create new page and make current, set height to margin, create new graphics object and 
            //textformatter for page.
			currentPage = pDoc.AddPage();
			currentHeight = PAGE_MARGIN;
			gfx = XGraphics.FromPdfPage(currentPage);
			tf = new XTextFormatter(gfx);
		}

        /// <summary>
        /// write a table of data to pdf file
        /// </summary>
        /// <param name="rows">the number of rows contained in the table</param>
        /// <param name="cols">the number of columns contained in the table</param>
        /// <param name="tabledata">a jagged array of string data</param>
        /// <param name="pDoc">a PdfDocument type</param>
        /// <param name="currentPage">the current page</param>
        /// <param name="currentHeight">an integer representing the current height in pts</param>
        /// <param name="gfx"> a pdfsharp library graphics object</param>
        /// <param name="tf">a pdfsharp library textformatter object</param>
        private void WriteTable(int rows, int cols, string[][] tabledata, PdfDocument pDoc, 
            ref PdfPage currentPage, ref double currentHeight, ref XGraphics gfx, ref XTextFormatter tf)
        {
            // Algorithm: Determine amount of space taken up by one cell, determine number of cells that
            // can fit on the rest of the page. Draw cells and their contents to the page, if more data
            // is in tabledata, move to next page and repeat. Repeat the whole process untill tabledata 
            // has been exhausted.
            throw new NotImplementedException();
        }
	}
}
