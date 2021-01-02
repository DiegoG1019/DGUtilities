using System;
using System.Linq;
using static DiegoG.MonoGame.LoadedLists;
using static DiegoG.Utilities.DiegoGMath;

namespace DiegoG.MonoGame
{
    public interface IDynamic
    {
        public ID ID { get; set; }
        public void Destroy();
    }
    public class ID
    {
        public bool Active { get; private set; }
        public object Holder { get; private set; }
        public Type HolderType => Holder.GetType();
        public ILoadedList HolderList { get; private set; }
        public int Value { get; set; }
        public IntFormat Format { get; set; }

        public static implicit operator string(ID i) => $"{i.HolderType.Name}_{FormatInt(i.Value, i.Format)}";

        public static implicit operator int(ID a) => a.Value;
        public static bool operator ==(ID a, ID b) => a.Value == b.Value && a.HolderType.FullName == b.HolderType.FullName;
        public static bool operator !=(ID a, ID b) => !(a == b);
        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) => base.Equals(obj);
        public override string ToString() => this;
        public string ToString(IntFormat format) => $"{HolderType.Name}_{FormatInt(Value, format)}";
        public void Activate(object holder, ILoadedList list)
        {
            HolderList = list;
            Holder = holder;
            Active = true;
        }
        public void Deactivate()
        {
            HolderList = null;
            Holder = null;
            Active = false;
        }
        public bool GetHolder<T>(out T holder) where T : class, IDynamic
        {
            if (typeof(T) != HolderType)
                throw new ArgumentException("Cannot request a holder type different from this ID's");
            holder = (T)Holder;
            return Active;
        }

        public ID(int v) :
            this(v, IntFormat.Hexadecimal)
        { }
        public ID(int v, IntFormat sf)
        {
            Value = v;
            Format = sf;
            Active = false;
        }

        public string Decimal => ToString(IntFormat.Decimal);
        public string Binary => ToString(IntFormat.Binary);
        public string Hexadecimal => ToString(IntFormat.Hexadecimal);
        public string Hexavigesimal => ToString(IntFormat.Hexavigesimal);
        public string Octal => ToString(IntFormat.Octal);
        public string Sexagesimal => ToString(IntFormat.Sexagesimal);
    }
}
