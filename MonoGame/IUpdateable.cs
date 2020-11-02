using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.MonoGame
{
    public interface IUpdateable
    {
        void Update(GameTime gt);
        bool Active { get; }
    }
}
