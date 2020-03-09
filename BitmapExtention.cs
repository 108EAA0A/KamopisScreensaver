using System.Drawing;
using System.Drawing.Drawing2D;

namespace KamopisScreensaver
{
    static class BitmapExtention
    {
        public static Bitmap ResizeImage(this Bitmap image, double dw, double dh)
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

        public static Bitmap ResizeImage(this Bitmap image, SizeF size) => ResizeImage(image, size.Width, size.Height);
    }
}
