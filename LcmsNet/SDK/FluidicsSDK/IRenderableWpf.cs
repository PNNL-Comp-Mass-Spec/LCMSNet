using System.Windows.Media;

namespace FluidicsSDK
{
    public interface IRenderableWpf
    {
        void Render(DrawingContext g, byte alpha, float scale);
    }
}
