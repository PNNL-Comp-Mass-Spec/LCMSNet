using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
