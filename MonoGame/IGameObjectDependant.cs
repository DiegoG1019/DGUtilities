using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.MonoGame
{
    public interface IGameObjectDependant
    {
        Game Game { get; set; }
        GraphicsDeviceManager GraphicsManager { get; set; }
    }
}
