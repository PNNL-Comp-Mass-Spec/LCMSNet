using System.Windows.Media;

namespace FluidicsSDK
{
    public interface IRenderable
    {
        void Render(DrawingContext g, byte alpha, float scale);
    }
}
