﻿
using PDFGenerator.Core.Model;

namespace PDFGenerator.Core.Services
{
    public interface IDocumentWriter
    {
        void SaveDocument(string path, Document document, DocumentContent[] documentContents, bool overwrite = true, bool titlePage = false);
    }
}
