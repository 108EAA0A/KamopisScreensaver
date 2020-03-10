using System.Drawing;
using Vector = System.Windows.Vector;

namespace KamopisScreensaver
{
    public class Kamopis
    {
        private readonly Image Image;
        public PointF Pos;
        public Vector Speed;
        public SizeF Size => this.Image.Size;

        public Rectangle Rect => new Rectangle((int)this.Pos.X, (int)this.Pos.Y, (int)this.Size.Width, (int)this.Size.Height);
        public RectangleF RectF => new RectangleF(this.Pos, this.Size);

        public Kamopis(double x, double y, double vx, double vy, double w, double h)
        {
            this.Pos.X = (float)x;
            this.Pos.Y = (float)y;
            this.Speed.X = vx;
            this.Speed.Y = vy;
            this.Image = Resource.KAMOPIS.Resized(w, h);
        }

        public Kamopis(PointF pos, Vector speed, SizeF size) : this(pos.X, pos.Y, speed.X, speed.Y, size.Width, size.Height) { }

        public void Update(SizeF clientSize)
        {
            this.Pos.X += (float)Speed.X;
            this.Pos.Y += (float)Speed.Y;

            if ((this.RectF.Right  > clientSize.Width  && this.Speed.X > 0) || (0 > this.Pos.X && this.Speed.X < 0)) this.Speed.X *= -1.0F;
            if ((this.RectF.Bottom > clientSize.Height && this.Speed.Y > 0) || (0 > this.Pos.Y && this.Speed.Y < 0)) this.Speed.Y *= -1.0F;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(this.Image, this.RectF);
        }
    }
}
