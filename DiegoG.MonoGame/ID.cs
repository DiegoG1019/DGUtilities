using Microsoft.Xna.Framework;
using System;
using static DiegoG.MonoGame.LoadedLists;
using static DiegoG.Utilities.DiegoGMath;

namespace DiegoG.MonoGame
{
    public interface IDynamic : IGameComponent
    {
        public ID ID { get; set; }
        public void Destroy();
        public event EventHandler<EventArgs> IDChanged;
    }
    public sealed record ID : IEquatable<ID>
    {
        public bool Active { get; private set; }
        public object Holder => HolderList.GetItem(this);
        public Type HolderType => Holder.GetType();
        public IntFormat Format { get; set; }

        public ILoadedList HolderList { get; init; }
        public int Value { get; init; }

        public static implicit operator string(ID i) => $"{i.HolderType.Name}_{FormatInt(i.Value, i.Format)}";

        public static implicit operator int(ID a) => a.Value;

        private readonly int HashCode;
        public override int GetHashCode() => HashCode;

        public bool Equals(ID obj) => HolderList == obj.HolderList && Value == obj.Value;

        public override string ToString() => this;

        public string ToString(IntFormat format) => $"{HolderType.Name}_{FormatInt(Value, format)}";

        public void Activate(object holder) => Active = true;
        public void Deactivate() => Active = false;
        public bool GetHolder<T>(out T holder) where T : class, IDynamic
        {
            if (typeof(T) != HolderType)
            {
                throw new ArgumentException("Cannot request a holder type different from this ID's");
            }

            holder = (T)Holder;
            return Active;
        }

        public ID(int v, ILoadedList holderList) :
            this(v, holderList, IntFormat.Hexadecimal)
        { }
        public ID(int v, ILoadedList holderList, IntFormat sf)
        {
            Value = v;
            Format = sf;
            HolderList = holderList;
            Active = false;
            HashCode = System.HashCode.Combine(Value, HolderList.GetType().FullName);
        }

        public string Decimal => ToString(IntFormat.Decimal);
        public string Binary => ToString(IntFormat.Binary);
        public string Hexadecimal => ToString(IntFormat.Hexadecimal);
        public string Hexavigesimal => ToString(IntFormat.Hexavigesimal);
        public string Octal => ToString(IntFormat.Octal);
        public string Sexagesimal => ToString(IntFormat.Sexagesimal);
    }
}
