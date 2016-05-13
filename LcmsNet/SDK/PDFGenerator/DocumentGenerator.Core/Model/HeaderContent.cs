
namespace EMSL.DocumentGenerator.Core.Model
{
	public class HeaderContent
		: DocumentContent
	{
		public HeaderContent()
		{
			Text	= null;
			Level	= HeaderLevel.H1;
		}


		public string Text { get; set; }
		public HeaderLevel Level { get; set; }

		public override ItemType ItemType { get { return ItemType.HeaderItem; } }
	}
}
