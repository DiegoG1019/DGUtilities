using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DiegoG.WPF
{
    public static class Extensions
    {
        public static void Deconstruct(this Thickness thickness, out double Left, out double Top, out double Right, out double Bottom)
        {
            Top = thickness.Top;
            Left = thickness.Left;
            Right = thickness.Right;
            Bottom = thickness.Bottom;
        }
    }
}
