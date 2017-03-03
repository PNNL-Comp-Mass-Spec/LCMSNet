using System.Drawing;

namespace FluidicsSDK
{
    public interface IRenderable
    {
        void Render(Graphics g, int alpha, float scale);
    }
}
