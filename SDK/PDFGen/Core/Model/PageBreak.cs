
namespace PDFGenerator.Core.Model
{
    public class PageBreak
        : DocumentContent
    {
        public override ItemType ItemType { get { return ItemType.PageBreakItem; } }
    }
}
