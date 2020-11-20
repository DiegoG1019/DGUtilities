using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiegoG.WPF
{
    public static class Extensions
    {
        public static T GetParent<T>(this Control control) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(control);
            while (!(parent is T) && parent != null)
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }
        public static void Deconstruct(this Thickness thickness, out double Left, out double Top, out double Right, out double Bottom)
        {
            Top = thickness.Top;
            Left = thickness.Left;
            Right = thickness.Right;
            Bottom = thickness.Bottom;
        }
    }
}
