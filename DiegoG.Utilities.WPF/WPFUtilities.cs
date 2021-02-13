using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiegoG.Utilities.WPF
{
    public static class WPFUtilities
    {
        public static Stream GetResource(string resource)
        {
            var stream = Application.GetResourceStream(new(resource, UriKind.Relative)).Stream;
            return stream is not null ? stream : throw new FileNotFoundException("Unable to find embedded resource", resource);
        }
        public static bool TryGetResource(string resource, [MaybeNullWhen(false)] out Stream stream)
        {
            stream = null;
            try { stream = Application.GetResourceStream(new(resource, UriKind.Relative)).Stream; }
            catch (IOException) { }
            return stream is not null;
        }
        public static CroppedBitmap[] CreateSpritesheet(BitmapImage imgsrc, int framewidth, int frameheight)
        {
            int count = 0;
            int verticalCount = imgsrc.PixelHeight / frameheight;
            int horizontalCount = imgsrc.PixelWidth / framewidth;

            var imglist = new CroppedBitmap[verticalCount * horizontalCount];

            for (int y = 0; y < verticalCount; y++)
                for (int x = 0; x < horizontalCount; x++)
                    imglist[count++] = new CroppedBitmap(imgsrc, new Int32Rect(x * framewidth, y * frameheight, framewidth, frameheight));
            return imglist;
        }
        public static CroppedBitmap[] CreateSpritesheet(string img, int framewidth, int frameheight)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();

            {//Local block, prevents local 's' to exist for the rest of the method, where it's unnecessary
                if (TryGetResource(img, out var s))
                    src.StreamSource = s;
                else
                    src.UriSource = new(img, UriKind.Relative);
            }

            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            return CreateSpritesheet(src, framewidth, frameheight);
        }
        public static Size GetElementPixelSize(UIElement element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
                transformToDevice = source.CompositionTarget.TransformToDevice;
            else
                using (var hwndsource = new HwndSource(new HwndSourceParameters()))
                    transformToDevice = hwndsource.CompositionTarget.TransformToDevice;

            return (Size)transformToDevice.Transform((Vector)element.DesiredSize);
        }
    }
}
