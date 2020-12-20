using DiegoG.Utilities;
using DiegoG.Utilities.Enumerations;
using System;
using System.Windows.Controls;

namespace DiegoG.WPF
{
    public class NumericBox : TextBox
    {
        public NumericBox() : base() => base.TextChanged += NumericBox_TextChanged;

        public new event TextChangedEventHandler TextChanged;

        private NumberTypes NumericTypeField = NumberTypes.Int32;
        public NumberTypes NumericType
        {
            get => NumericTypeField;
            set
            {
                PrevValue = Other.AutoCast(PrevValue, NumericTypeField);
                NumericTypeField = value;
            }
        }

        public void SetNumber<T>(T number)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
            => Text = number.ToString();

        private string TryParse<T>(string input)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            if (Other.GenericTryParse(input, out T result))
            {
                TextChangedSuccess = true;
                return result.ToString();
            }
            return PrevValue.ToString();
        }

        private bool TextChangedSuccess;
        private object PrevValue = 0;
        private void NumericBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChangedSuccess = false;
            Text = NumericType switch
            {
                NumberTypes.Byte => TryParse<byte>(Text),
                NumberTypes.SByte => TryParse<sbyte>(Text),
                NumberTypes.Int16 => TryParse<short>(Text),
                NumberTypes.UInt16 => TryParse<ushort>(Text),
                NumberTypes.Int32 => TryParse<int>(Text),
                NumberTypes.UInt32 => TryParse<uint>(Text),
                NumberTypes.Int64 => TryParse<long>(Text),
                NumberTypes.UInt64 => TryParse<ulong>(Text),
                NumberTypes.Half => throw new NotSupportedException(),
                NumberTypes.Single => TryParse<float>(Text),
                NumberTypes.Double => TryParse<double>(Text),
                NumberTypes.Decimal => TryParse<decimal>(Text),
                _ => throw new NotImplementedException(),
            };
            if (TextChangedSuccess && TextChanged is not null)
                TextChanged(this, e);
        }
    }
}
