using Microsoft.Xna.Framework;
using static MagicGame.Assets;

namespace DiegoG.MonoGame
{
    public interface IDrawable
    {
        ManagedTexture2D Texture { get; }
        Camera.DrawData DrawData { get; }
        Color Color { get; set; }
        Vector2 TextureScale { get; }
        Vector2 Origin { get; }
        Rectangle TextureSize { get; }
        LengthVector2 TextureOffset { get; }
        void Draw(GameTime gt);
    }
}
