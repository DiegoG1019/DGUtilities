using System;
using System.Linq;
using static DiegoG.Utilities.DiegoGMath;

namespace DiegoG.MonoGame
{
    public class ID
    {
        public bool Active { get; set; }
        public object Holder { get; set; }
        public Type HolderType => Holder.GetType();
        public int Value { get; set; }

        public IntFormat Format { get; set; }

        public static implicit operator string(ID i) => $"{i.HolderType.Name}_{FormatInt(i.Value, i.Format)}";

        public static implicit operator int(ID a) => a.Value;
        public static bool operator ==(ID a, ID b) => a.Value == b.Value && a.HolderType.FullName == b.HolderType.FullName;
        public static bool operator !=(ID a, ID b) => !(a == b);
        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) => base.Equals(obj);
        public override string ToString() => this;
        public bool GetHolder<T>(out T holder) where T : class, IDynamic
        {
            if (typeof(T) != HolderType)
                throw new ArgumentException("Cannot request a holder type different from this ID's");
            if (Active)
                holder = (T)Holder ?? ;
        }

        public ID(int v, object holder) :
            this(v, holder, IntFormat.Hexadecimal)
        { }
        public ID(int v, Type holder) :
            this(v, holder, IntFormat.Hexadecimal)
        { }
        public ID(int v, object holder, IntFormat sf) :
            this(v, holder.GetType(), sf)
        { }
        public ID(int v, Type holder, IntFormat sf)
        {
            HolderType = holder;
            Value = v;
            Format = sf;
            Active = true;
        }

        public string Decimal
        {
            get
            {
                Format = IntFormat.Decimal;
                return this;
            }
        }
        public string Binary
        {
            get
            {
                Format = IntFormat.Binary;
                return this;
            }
        }
        public string Hexadecimal
        {
            get
            {
                Format = IntFormat.Hexadecimal;
                return this;
            }
        }
        public string Hexavigesimal
        {
            get
            {
                Format = IntFormat.Hexavigesimal;
                return this;
            }
        }
        public string Octal
        {
            get
            {
                Format = IntFormat.Octal;
                return this;
            }
        }
        public string Sexagesimal
        {
            get
            {
                Format = IntFormat.Sexagesimal;
                return this;
            }
        }
    }
}
