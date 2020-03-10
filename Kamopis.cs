using System;
using System.Drawing;
using Vector = System.Windows.Vector;

namespace KamopisScreensaver
{
    public class Kamopis
    {
        private readonly Image Image;
        public PointF Pos;
        public Vector Speed;
        public double Theta;
        public double SpeedT;
        public SizeF Size => this.Image.Size;

        public Rectangle Rect => new Rectangle((int)this.Pos.X, (int)this.Pos.Y, (int)this.Size.Width, (int)this.Size.Height);
        public RectangleF RectF => new RectangleF(this.Pos, this.Size);

        public Kamopis(double x, double y, double vx, double vy, double t, double vt, double w, double h)
        {
            this.Pos.X = (float)x;
            this.Pos.Y = (float)y;
            this.Speed.X = vx;
            this.Speed.Y = vy;
            this.Theta = t;
            this.SpeedT = vt;
            this.Image = Resource.KAMOPIS.Resized(w, h);
        }

        public Kamopis(PointF pos, Vector speed, double t, double vt, SizeF size)
            : this(pos.X, pos.Y, speed.X, speed.Y, t, vt, size.Width, size.Height) { }

        public void Update(SizeF clientSize)
        {
            this.Pos.X += (float)Speed.X;
            this.Pos.Y += (float)Speed.Y;

            this.Theta += this.SpeedT;
            this.Theta %= 360;

            if ((this.RectF.Right  > clientSize.Width  && this.Speed.X > 0) || (0 > this.Pos.X && this.Speed.X < 0)) this.Speed.X *= -1.0F;
            if ((this.RectF.Bottom > clientSize.Height && this.Speed.Y > 0) || (0 > this.Pos.Y && this.Speed.Y < 0)) this.Speed.Y *= -1.0F;
        }

        public void Draw(Graphics g)
        {
            //g.DrawRectangle(new Pen(Color.Black), this.Rect);

            //g.DrawImage(this.Image, this.RectF);

            g.DrawImage(this.Image, GetRotatedPointFs(this.Theta));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="theta"></param>
        /// <returns>左上隅、右上隅、左下隅の3箇所の座標</returns>
        /// <see cref="https://dobon.net/vb/dotnet/graphics/skewing.html"/>
        private PointF[] GetRotatedPointFs(double theta)
        {
            // ラジアン単位に変換
            double rad = theta / (180 / Math.PI);

            // 新しい座標位置を計算する
            var s = (float)Math.Sin(rad);
            var c = (float)Math.Cos(rad);

            var centerX = this.Pos.X + this.Size.Width  / 2;
            var centerY = this.Pos.Y + this.Size.Height / 2;

            float x1 = centerX + this.Size.Width  * c;
            float y1 = centerY + this.Size.Width  * s;
            float x2 = centerX - this.Size.Height * s;
            float y2 = centerY + this.Size.Height * c;

            // PointF配列を作成
            PointF[] destinationPoints = {
                new PointF(centerX, centerY),
                new PointF(x1, y1),
                new PointF(x2, y2),
            };
            return destinationPoints;
        }
    }
}
