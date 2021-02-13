using DiegoG.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace DiegoG.MonoGame.GameTypes
{
    public abstract class GameEntity : DrawableGameComponent, IDrawable, IDynamic
    {
        public ID ID
        {
            get => IDField;
            set { IDField = value; IDChanged?.Invoke(this, new GenericEventArgs<ID>(value)); }
        }
        ID IDField;

        public DrawData DrawData { get; protected set; }
        public LengthVector2 Position { get; set; } = LengthVector2.Zero;

        public event EventHandler<EventArgs> IDChanged;

        public void Activate()
        {
            Enabled = true;
            Visible = true;
        }

        public void Deactivate()
        {
            Enabled = false;
            Visible = false;
        }

        protected GameEntity(Game game) : base(game) { }
        public virtual void Destroy() => ID.HolderList.Remove(ID);
    }
}
