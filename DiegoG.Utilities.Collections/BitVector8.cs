using System;
using System.Collections.Generic;
using System.IO;

namespace DiegoG.Utilities.Collections
{
    [Serializable]
    public struct BitVector8 : IEquatable<BitVector8>//, IEnumerable<bool>
    {
        public struct Section
        {
            public byte Offset
            {
                get => OffsetField;
                set
                {
                    Validate(value, Width);
                    OffsetField = value;
                }
            }
            byte OffsetField;
            public int Limit => Width + Offset;
            public byte Width
            {
                get => WidthField;
                set
                {
                    Validate(Offset, value);
                    WidthField = value;
                }
            }
            byte WidthField;
            private static byte Validate(byte offset, byte width) => (offset + width) < 8 ? (byte)0 : throw new ArgumentOutOfRangeException($"Offset and Width must be less than 8. offset:{offset} + width:{width} = {offset + width}");
        }


        private static bool Null;

        public byte Data { get; private set; }

        public bool this[int key]
        {
            get => CheckBounds(key) && (Data & (1 << key)) != 0;
            set
            {
                if (CheckBounds(key) && value)
                {
                    Data |= (byte)(1 << key);
                }
                else
                {
                    Data &= (byte)~(1 << key);
                }
            }
        }
        private static bool CheckBounds(int key) => key is >0 and <8 ? true : throw new ArgumentOutOfRangeException(nameof(key), $"Must be within 0 and 7");

        public BitVector8(bool b1 = false, bool b2 = false, bool b3 = false, bool b4 = false, bool b5 = false, bool b6 = false, bool b7 = false, bool b8 = false)
        {
            Data = 0;
            this[0] = b1;
            this[1] = b2;
            this[2] = b3;
            this[3] = b4;
            this[4] = b5;
            this[5] = b6;
            this[6] = b7;
            this[7] = b8;
        }
        public BitVector8(byte data) => Data = data;

        public void ClearAll() => Data = 0;

        public void SetAll() => Data = byte.MaxValue;

        public void FlipAll() => Data = (byte)~Data;

        public void Retrieve(ref bool b0) => Retrieve(ref b0, ref Null, ref Null, ref Null, ref Null, ref Null, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1) => Retrieve(ref b0, ref b1, ref Null, ref Null, ref Null, ref Null, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2) => Retrieve(ref b0, ref b1, ref b2, ref Null, ref Null, ref Null, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2, ref bool b3) => Retrieve(ref b0, ref b1, ref b2, ref b3, ref Null, ref Null, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2, ref bool b3, ref bool b4) => Retrieve(ref b0, ref b1, ref b2, ref b3, ref b4, ref Null, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2, ref bool b3, ref bool b4, ref bool b5) => Retrieve(ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref Null, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2, ref bool b3, ref bool b4, ref bool b5, ref bool b6) => Retrieve(ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref Null);

        public void Retrieve(ref bool b0, ref bool b1, ref bool b2, ref bool b3, ref bool b4, ref bool b5, ref bool b6, ref bool b7)
        {
            b0 = this[0];
            b1 = this[1];
            b2 = this[2];
            b3 = this[3];
            b4 = this[4];
            b5 = this[5];
            b6 = this[6];
            b7 = this[7];
        }

        public void Input(bool b0, bool? b1 = null, bool? b2 = null, bool? b3 = null, bool? b4 = null, bool? b5 = null, bool? b6 = null, bool? b7 = null)
        {
            this[0] = b0;
            this[1] = b1 ?? this[1];
            this[2] = b2 ?? this[2];
            this[3] = b3 ?? this[3];
            this[4] = b4 ?? this[4];
            this[5] = b5 ?? this[5];
            this[6] = b6 ?? this[6];
            this[7] = b7 ?? this[7];
        }

        public static BitVector8[] ComposeBitsBytesChain(bool optimizeLength, params bool[] flags)
        {
            int num = flags.Length;
            int num2 = 0;
            while (num > 0)
            {
                num2++;
                num -= 7;
            }
            BitVector8[] array = new BitVector8[num2];
            byte num3 = 0;
            byte num4 = 0;
            for (byte i = 0; i < flags.Length; i++)
            {
                array[num4][num3] = flags[i];
                num3++;
                if (num3 == 7 && num4 < num2 - 1)
                {
                    array[num4][num3] = true;
                    num3 = 0;
                    num4++;
                }
            }
            if (optimizeLength)
            {
                int num5 = array.Length - 1;
                while (array[num5] == 0 && num5 > 0)
                {
                    array[num5 - 1][7] = false;
                    num5--;
                }
                Array.Resize(ref array, num5 + 1);
            }
            return array;
        }

        public static BitVector8[] DecomposeBitsBytesChain(BinaryReader reader)
        {
            List<BitVector8> list = new List<BitVector8>();
            BitVector8 item;
            do
            {
                item = reader.ReadByte();
                list.Add(item);
            }
            while (item[7]);
            return list.ToArray();
        }

        public static bool operator ==(BitVector8 a, BitVector8 b) => a.Equals(b);

        public static bool operator !=(BitVector8 a, BitVector8 b) => !a.Equals(b);

        public static implicit operator byte(BitVector8 bb) => bb.Data;

        public static implicit operator BitVector8(byte b) => new(b);

        public bool Equals(BitVector8 other) => Data == other.Data;

        public override bool Equals(object obj)
        {
            if (obj is not null && obj is BitVector8 o)
            {
                return Equals(o);
            }

            return false;
        }

        public override int GetHashCode() => Data;


        //public IEnumerator<bool> GetEnumerator()
        //{
        //    for (int i = 0; i < 8; i++)
        //        yield return this[i];
        //}

        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}