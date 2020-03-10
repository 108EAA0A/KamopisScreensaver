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
        public float Theta { get; private set; }
        public float SpeedT { get; private set; }

        public float X => this.Pos.X;
        public float Y => this.Pos.Y;
        public SizeF Size => this.Image.Size;

        public PointF Center => this.Pos + new SizeF(this.Size.Width / 2, this.Size.Height / 2);
        public Rectangle Rect => new Rectangle((int)this.Pos.X, (int)this.Pos.Y, (int)this.Size.Width, (int)this.Size.Height);
        public RectangleF RectF => new RectangleF(this.Pos, this.Size);

        /// <summary>KAMOPISを生成する</summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        /// <param name="vx">加速度X</param>
        /// <param name="vy">加速度Y</param>
        /// <param name="t">角度</param>
        /// <param name="vt">角加速度</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Kamopis(float x, float y, float vx, float vy, float t, float vt, float w, float h)
        {
            this.Pos.X = x;
            this.Pos.Y = y;
            this.Speed.X = vx;
            this.Speed.Y = vy;
            this.Theta = t;
            this.SpeedT = vt;
            this.Image = Resource.KAMOPIS.Resized(w, h);
        }

        public Kamopis(PointF pos, Vector speed, float t, float vt, SizeF size)
            : this(pos.X, pos.Y, (float)speed.X, (float)speed.Y, t, vt, size.Width, size.Height) { }

        public void Update(SizeF clientSize)
        {
            this.Pos.X += (float)Speed.X;
            this.Pos.Y += (float)Speed.Y;

            this.Theta += this.SpeedT;
            this.Theta %= 360;

            // 画面外へ移動中の場合に加速度のベクトルを反転
            if ((this.RectF.Right  > clientSize.Width  && this.Speed.X > 0) || (0 > this.Pos.X && this.Speed.X < 0)) this.Speed.X *= -1.0F;
            if ((this.RectF.Bottom > clientSize.Height && this.Speed.Y > 0) || (0 > this.Pos.Y && this.Speed.Y < 0)) this.Speed.Y *= -1.0F;
        }

        public void Draw(Graphics g)
        {
            // 四角い枠線を表示する(デバッグ用)
            //g.DrawRectangle(new Pen(Color.Black), this.Rect);

            g.DrawImage(this.Image, GetRotatedPointFs(this.Theta));
            g.DrawEllipse(new Pen(Color.Black), this.RectF);
        }

        /// <summary>
        /// 回転して表示するための回転後の座標を計算して返す
        /// </summary>
        /// <param name="theta"></param>
        /// <returns>左上隅、右上隅、左下隅の3箇所の座標</returns>
        /// <see cref="https://dobon.net/vb/dotnet/graphics/skewing.html"/>
        private PointF[] GetRotatedPointFs(float theta)
        {
            // ラジアン単位に変換
            var rad = theta / (180 / Math.PI);

            // 三角関数の呼び出しを減らすため先に計算する
            var s = (float)Math.Sin(rad);
            var c = (float)Math.Cos(rad);

            // 新しい座標位置を計算する
            var topLeft    = AddPointF(this.Center, RotatedPointFImpl(SubtractPointF(this.RectF.Location, this.Center), s, c));
            var topRight   = AddPointF(this.Center, RotatedPointFImpl(SubtractPointF(new PointF(this.RectF.Right, this.Y), this.Center), s, c));
            var bottomLeft = AddPointF(this.Center, RotatedPointFImpl(SubtractPointF(new PointF(this.X, this.RectF.Bottom), this.Center), s, c));

            // PointF配列を作成
            PointF[] destinationPoints = {
                topLeft,
                topRight,
                bottomLeft,
            };
            return destinationPoints;
        }

        private static PointF RotatedPointFImpl(PointF p, float sin, float cos) => new PointF(p.X * cos - p.Y * sin, p.Y * cos + p.X * sin);

        /// <summary>原点を中心に点Pを回転させる</summary>
        /// <param name="p">点P</param>
        /// <see cref="https://mathwords.net/heimenkaiten"/>
        private static PointF RotatedPointF(PointF p, float theta)
        {
            var rad = theta / (180 / Math.PI);
            var s = (float)Math.Sin(rad);
            var c = (float)Math.Cos(rad);
            return RotatedPointFImpl(p, s, c);
        }

        private static PointF AddPointF(PointF lhs, PointF rhs) => new PointF(lhs.X + rhs.X, lhs.Y + rhs.Y);
        private static PointF SubtractPointF(PointF lhs, PointF rhs) => new PointF(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }
}
