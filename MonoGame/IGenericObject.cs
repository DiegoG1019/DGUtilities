using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame
{
    public interface IGenericObject : IUpdateable, IDynamic
    {
        void Scale(float w, float h, bool adjustMass = true);
        void Scale(float s, bool adjustMass = true);
    }
}
