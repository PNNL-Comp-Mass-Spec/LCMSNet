namespace LcmsNet.Method.Drawing
{
    public class LCRendererFactory
    {
        public static LCMethodRenderer GetRenderer(enumLCMethodRenderMode mode)
        {
            LCMethodRenderer renderer = null;
            if (mode == enumLCMethodRenderMode.Column)
            {
                renderer = new LCMethodColumnModeRenderer();
            }
            else if (mode == enumLCMethodRenderMode.Conversation)
            {
                renderer = new LCMethodConversationRenderer();
            }
            return renderer;
        }
    }
}