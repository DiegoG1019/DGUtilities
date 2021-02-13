using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame
{
    public interface IGameObjectDependant
    {
        Game Game { get; set; }
        GraphicsDeviceManager GraphicsManager { get; set; }
    }
}
