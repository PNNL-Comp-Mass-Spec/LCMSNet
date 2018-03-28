namespace LcmsNet.Method.Drawing
{
    public class LCRendererFactory
    {
        public static LCMethodRenderer GetRenderer(LCMethodRenderMode mode)
        {
            LCMethodRenderer renderer = null;
            if (mode == LCMethodRenderMode.Column)
            {
                renderer = new LCMethodColumnModeRenderer();
            }
            else if (mode == LCMethodRenderMode.Conversation)
            {
                renderer = new LCMethodConversationRenderer();
            }
            return renderer;
        }
    }
}