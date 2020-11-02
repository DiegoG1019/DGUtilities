using Microsoft.Xna.Framework;
using static DiegoG.MonoGame.Assets;

namespace DiegoG.MonoGame
{
    public interface IDrawable
    {
        void Draw(GameTime gameTime, Camera camera);
        bool Visible { get; }
        ManagedTexture2D Texture { get; }
        Camera.DrawData DrawData { get; }
        Color Color { get; set; }
        Vector2 TextureScale { get; }
        Vector2 Origin { get; }
        Rectangle TextureSize { get; }
        LengthVector2 TextureOffset { get; }
    }
}
