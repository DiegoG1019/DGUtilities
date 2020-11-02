using DiegoG.Utilities;
using tainicom.Aether.Physics2D.Dynamics;

namespace DiegoG.MonoGame
{
    public interface IPhysical
    {
        /// <summary>
        /// Defines the physical body of the object
        /// </summary>
        Body Body { get; }
        /// <summary>
        /// Defines the physical mass of the object's body
        /// </summary>
        Mass Mass { get; set; }
        /// <summary>
        /// Defines the physical LinearVelocity of the object's body
        /// </summary>
        LengthVector2 LinearVelocity { get; set; }
        /// <summary>
        /// Defines the physical, absolute position of of the object's body
        /// </summary>
        LengthVector2 Position { get; set; }
    }
}
