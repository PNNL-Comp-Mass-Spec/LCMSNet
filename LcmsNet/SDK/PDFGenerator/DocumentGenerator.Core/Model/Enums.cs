
namespace EMSL.DocumentGenerator.Core.Model
{
    /// <summary>
    /// This enum contains the different levels that a
    /// Head may be.
    /// </summary>
    public enum HeaderLevel
    {
        H1 = 3,
        H2 = 2,
        H3 = 1
    }

    /// <summary>
    /// This enum contains the different types of content
    /// the Document class supports.
    /// </summary>
    public enum ItemType
    {
        HeaderItem,
        ImageItem,
        PageBreakItem,
        ParagraphItem,
        TableItem
    }

    /// <summary>
    /// This enum contains the different locations that
    /// an image caption can be placed.
    /// </summary>
    public enum CaptionPlacement
    {
        Top,
        Bottom
    }
}
