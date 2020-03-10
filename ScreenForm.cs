using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KamopisScreensaver
{
    /// <summary>
    /// スクリーンセーバーとして表示されるフォーム
    /// </summary>
    /// <see cref="http://www7b.biglobe.ne.jp/~whitetiger/cs/cs2010001.html"/>
    public partial class ScreenForm : Form
    {
        private readonly bool IsPreviewMode;

        #region Constructors

        private ScreenForm()
        {
            InitializeComponent();
            this.timerUpdate.Interval = 33;        // TODO: 60FPS固定なので可変にする
            this.timerUpdate.Start();
            Cursor.Hide();
        }

        // このコンストラクタは、フォームを全画面で表示する
        // ノーマルモードで使用される (/s)
        public ScreenForm(Rectangle Bounds) : this()
        {
            this.IsPreviewMode = false;
            this.Bounds = Bounds; // Boundsに代入するとSizeも変わる
        }

        // このコンストラクタは、スクリーンセーバーの選択ダイアログボックスの小窓で使用される
        // プレビューモードで使用される (/p)
        public ScreenForm(IntPtr PreviewHandle) : this()
        {
            this.IsPreviewMode = true;

            // このウィンドウの親ウィンドウを設定する
            PInvoke.SetParent(this.Handle, PreviewHandle);

            // この子ウィンドウを作成する
            // スクリーンセーバーの設定ダイアログボックスが閉じられると終了する
            var dwNewLong = new IntPtr((uint)PInvoke.GetWindowLongPtr(this.Handle, -16) | (uint)PInvoke.WindowStyles.WS_CHILD);
            PInvoke.SetWindowLongPtr(new HandleRef(this, this.Handle), -16, dwNewLong);

            // 親ウィンドウのサイズと同じにする
            PInvoke.GetClientRect(PreviewHandle, out var ParentRect);
            this.Size = ParentRect.Size;

            // ウィンドウの位置を左上隅に
            this.Location = Point.Empty;

            // プレビューでわかりやすくする
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }
        #endregion

        private void ScreenForm_Load(object sender, EventArgs e)
        {
            KamopisInitalize(Math.Min(this.Size.Width, this.Size.Height) / 6);
        }

        private void KamopisInitalize(int KamopisBaseSize, int n = 20)
        {
            var rand = new Random();

            Kamopises.Capacity = n;
            for (int i = 0; i < Kamopises.Capacity; i++)
            {
                var x = this.ClientSize.Width  * rand.NextDouble();
                var y = this.ClientSize.Height * rand.NextDouble();
                var vx = KamopisBaseSize / 10 * (rand.NextDouble() - 0.5);
                var vy = KamopisBaseSize / 10 * (rand.NextDouble() - 0.5);
                var s = KamopisBaseSize * (rand.NextDouble() + 0.5);
                this.Kamopises.Add(new Kamopis(x, y, vx, vy, s, s));

        private static Image GetScreenCaptureImage()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 画面全体をコピーする
                g.CopyFromScreen(Point.Empty, Point.Empty, bmp.Size);
            }
            return bmp;
        }

        private readonly Image CapturedScreenImage = GetScreenCaptureImage();

        private readonly List<Kamopis> Kamopises = new List<Kamopis>();

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            foreach (var kamopis in Kamopises)
            {
                kamopis.Update(this.ClientSize);
            }

            // 再描画
            Invalidate();
        }

        private void ScreenForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.CapturedScreenImage, Point.Empty);

            foreach (var kamopis in Kamopises)
            {
                kamopis.Draw(e.Graphics);
            }
        }

        #region User Input

        private void ScreenForm_KeyDown(object sender, KeyEventArgs e)
        {
            // プレビューを実行していないときプレビュー画面用に無効にする
            if (!this.IsPreviewMode) Environment.Exit(0);
        }

        private void ScreenForm_Click(object sender, EventArgs e)
        {
            // プレビューを実行していないときプレビュー画面用に無効にする
            if (!this.IsPreviewMode) Environment.Exit(0);
        }

        // XとY int.MaxValueのとOriginalLoctionを始める
        // カーソルがその位置にすることは不可能なので。
        // この変数がまだ設定されている場合
        private Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);

        private void ScreenForm_MouseMove(object sender, MouseEventArgs e)
        {
            // プレビューのとき終了関数を無効にする
            if (this.IsPreviewMode) return;

            // originallocationが設定されているかどうかを確認
            if (this.OriginalLocation.X == int.MaxValue &&
                this.OriginalLocation.Y == int.MaxValue) OriginalLocation = e.Location;

            // マウスが20 pixels 以上動いたかどうかを監視
            // 動いた場合はアプリケーションを終了
            if (Math.Abs(e.X - this.OriginalLocation.X) > 20 ||
                Math.Abs(e.Y - this.OriginalLocation.Y) > 20) Environment.Exit(0);
        }
        #endregion
    }
}
