using System;
using System.Collections.Generic;

using EMSL.DocumentGenerator.Core.Model;
using EMSL.DocumentGenerator.Core.Services;


namespace EMSL.DocumentGenerator.Core
{
    public class Document
    {
        // Any attributes that are only meant to be accessed
        // by their Properties are with said Property.
        #region Attributes
        private static readonly char[] ParagraphSplitChars = new char[] { '\n' };

        private List<DocumentContent> m_content;
        #endregion


        #region Constructors
        public Document()
        {
            m_content = new List<DocumentContent>(10);

            Font                = "Times New Roman";
            FontSize            = 12;
            Date                = DateTime.Now;
            DateFormatString    = null;

            HeaderFont          = null;
            HeaderFontSizeOffset = 3;

            Title               = "Title Goes Here";
            TitleFont           = null;
            UnderlinTitle       = false;
            TitleFontSize       = 30;
        }
        #endregion


        #region Properties
        public IDocumentWriter DocumentWriter { get; set; }

        public string Title { get; set; }
        public bool UnderlinTitle { get; set; }

        public string TitleFont
        {
            get { return m_titleFont ?? Font; }
            set { m_titleFont = value; }
        }
        private string m_titleFont;

        public double TitleFontSize
        {
            get { return m_titleFontSizeHasBeenSet ? m_titleFontSize : FontSize + 6; }
            set
            {
                m_titleFontSize = value;
                m_titleFontSizeHasBeenSet = true;
            }
        }
        private double  m_titleFontSize;
        private bool    m_titleFontSizeHasBeenSet = false;

        public string Font { get; set; }
        public double FontSize { get; set; }

        public string HeaderFont
        {
            get { return m_headerFont ?? Font; }
            set { m_headerFont = value; }
        }
        private string m_headerFont;

        public double HeaderFontSizeOffset { get; set; }

        public string Version { get; set; }
        public DateTime Date { get; set; }
        public string DateFormatString { get; set; }
        #endregion


        #region Methods

        #region Add Content Items
        public void AddPageBreak()
        {
            m_content.Add(new PageBreak());
        }

        public void AddHeader(HeaderLevel headerLevel, string text)
        {
            m_content.Add(new HeaderContent() { Level = headerLevel, Text = text });
        }

        public void AddTable(object table)
        {
            throw new NotImplementedException();
        }

        public void AddImage(ImageContent image)
        {
            if (image != null)
                m_content.Add(image);
        }

        public void AddParagraph(string text)
        {
            if (text == null)
            {
                m_content.Add(new ParagraphContent() { Text = string.Empty });
                return;
            }

            text = text.Replace("\r", string.Empty);
            string[] breakdown = text.Split(ParagraphSplitChars, StringSplitOptions.None);
            
            foreach (var s in breakdown)
            {
                if (string.IsNullOrWhiteSpace(s))
                    m_content.Add(new ParagraphContent() { Text = string.Empty });
                else
                    m_content.Add(new ParagraphContent() { Text = s });
            }
        }
        #endregion

        public double GetHeaderFontSize(HeaderLevel level)
        {
            double fontSize = FontSize + ((int) level) * HeaderFontSizeOffset;
            return fontSize;
        }

        public void WriteDocument(string path, bool overwrite = true)
        {
            if (this.DocumentWriter == null)
                throw new InvalidOperationException("A document writer has not been set.");

            DocumentWriter.SaveDocument(path, this, m_content.ToArray(), overwrite);
        }
        #endregion
    }
}
