
namespace PDFGenerator.Core.Model
{
    public class ParagraphContent
        : DocumentContent
    {
        public ParagraphContent()
        {
            Text = null;
        }


        public string Text { get; set; }

        public override ItemType ItemType { get { return ItemType.ParagraphItem; } }
    }
}
