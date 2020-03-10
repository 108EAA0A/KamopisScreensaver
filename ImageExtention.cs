using System.Drawing;
using System.Drawing.Drawing2D;

namespace KamopisScreensaver
{
    static class ImageExtention
    {
        public static Image Resized(this Image image, double dw, double dh)
        {
            double hi;
            double imagew = image.Width;
            double imageh = image.Height;

            if ((dh / dw) <= (imageh / imagew))
            {
                hi = dh / imageh;
            }
            else
            {
                hi = dw / imagew;
            }

            var result = new Bitmap((int)(imagew * hi), (int)(imageh * hi));
            var g = Graphics.FromImage(result);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, 0, 0, result.Width, result.Height);

            return result;
        }

        public static Image Resized(this Image image, SizeF size) => Resized(image, size.Width, size.Height);
    }
}
