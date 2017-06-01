namespace LcmsNet.Method.Drawing
{
    public class LCRendererFactory
    {
        public static classLCMethodRenderer GetRenderer(enumLCMethodRenderMode mode)
        {
            classLCMethodRenderer renderer = null;
            if (mode == enumLCMethodRenderMode.Column)
            {
                renderer = new classLCMethodColumnModeRenderer();
            }
            else if (mode == enumLCMethodRenderMode.Conversation)
            {
                renderer = new classLCMethodConversationRenderer();
            }
            return renderer;
        }

        public static LCMethodRenderer GetRendererWpf(enumLCMethodRenderMode mode)
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