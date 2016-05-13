using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMSL.DocumentGenerator.Core.Model;
using EMSL.DocumentGenerator.Core;

using PDFSharpWriter	= EMSL.DocumentGenerator.PDFSharp.PDFWriter;
using GranotWriter		= EMSL.DocumentGenerator.Granot.PDFWriter;
using EMSL.DocumentGenerator.Core.Services;


namespace CreateSampleDoc
{
	class Program
	{
		static void Main(string[] args)
		{
			Document		d		= new Document();
			IDocumentWriter writer0 = new PDFSharpWriter();
			IDocumentWriter writer1 = new GranotWriter();


			PopulateDocument(d);


			string path0 = "TestFile0.pdf";
			//string path1 = "TestFile1.pdf";

			d.DocumentWriter = writer0;
			d.WriteDocument(path0, true);

			//d.DocumentWriter = writer1;
			//d.WriteDocument(path1, true);

			try
			{
				System.Diagnostics.Process.Start(path0);
				//System.Diagnostics.Process.Start(path1);
			}
			catch (Exception ex)
			{
				string s = string.Empty;
				s = ex.ToString();
			}
		}

		private static void PopulateDocument(Document d)
		{
			d.FontSize = 17;
			d.Title = "Test Document.\nThis is the song that never ends. It goes on and on my friend.";
			d.Version = "0.1.0";
			d.UnderlinTitle = true;

			for (int i = 0; i < 10; i++)
			{
				d.AddHeader(HeaderLevel.H1, string.Format("Round {0}", i + 1));
                ImageContent testImage = new ImageContent(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\testbitmap.bmp");
                testImage.CaptionText = "Test Caption";                
				d.AddImage(testImage);
				d.AddParagraph("a b c d e f g h i j k l m n o p q r s t u v w x y z");
				d.AddParagraph("A B C D E F G H I J K L M N O P Q R S T U V W X Y Z");
				d.AddParagraph("The quick red fox jumped over the lazy brown dog.");
				d.AddParagraph(
					"Petter Piper picked a peck of pickled pepers. " +
					"A peck of pickled pepers, Petter Piper picked. " +
					"If Petter Piper picked a peck of pickled pepers, " +
					"where's the peck of pickled pepers Petter Piper picked?");
				d.AddParagraph(
					"El señor Salas sala su salsa en la sala con sal de salinas. " +
					"Con sal de salinas sala su salsa el señor Salas en la sala. ");
				d.AddParagraph(
					"Si Pancha plancha con cuarto planchas, " +
					"¿con cuántas planchas plancha Pancha?");
				d.AddHeader(HeaderLevel.H2, "End Round");
				d.AddParagraph(null);
			}
		}
	}
}
