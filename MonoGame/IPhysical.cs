using tainicom.Aether.Physics2D.Dynamics;

namespace DiegoG.MonoGame
{
    public interface IPhysical
    {
        Body Body { get; }
        Mass Mass { get; set; }
        LengthVector2 LinearVelocity { get; set; }
        LengthVector2 Position { get; set; }
    }
}
