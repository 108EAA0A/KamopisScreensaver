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
        readonly bool IsPreviewMode;

        #region Constructors

        // このコンストラクタは、フォームを全画面で表示する
        // ノーマルモードで使用される (/s)
        public ScreenForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.IsPreviewMode = false;

            this.TransparencyKey = this.BackColor;
            this.Bounds = Bounds;
            Cursor.Hide();
        }

        // このコンストラクタは、スクリーンセーバーの選択ダイアログボックスの小窓で使用される
        // プレビューモードで使用される (/p)
        public ScreenForm(IntPtr PreviewHandle)
        {
            InitializeComponent();
            this.IsPreviewMode = true;

            // このウィンドウの親ウィンドウを設定する
            PInvoke.SetParent(this.Handle, PreviewHandle);

            // この子ウィンドウを作成する
            // スクリーンセーバーの設定ダイアログボックスが閉じられると終了する
            var dwNewLong = new IntPtr((uint)PInvoke.GetWindowLongPtr(this.Handle, -16) | (uint)PInvoke.WindowStyles.WS_CHILD);
            PInvoke.SetWindowLongPtr(new HandleRef(this, this.Handle), -16, dwNewLong);

            // 親ウィンドウのサイズに設定する
            PInvoke.GetClientRect(PreviewHandle, out var ParentRect);
            this.Size = ParentRect.Size;

            // ロケーションを設定
            this.Location = Point.Empty;

            // プレビューでわかりやすくする
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }
        #endregion

        private void ScreenForm_Load(object sender, EventArgs e)
        {
            Initalize();
        }

        private void Initalize()
        {
            this.timerUpdate.Interval = 33;
            this.timerUpdate.Start();

            var rand = new Random();

            Kamopises.Capacity = 50;
            for (int i = 0; i < Kamopises.Capacity; i++)
            {
                var x = this.ClientSize.Width * rand.NextDouble();
                var y = this.ClientSize.Height * rand.NextDouble();
                var vx = 30 * (rand.NextDouble() - 0.5);
                var vy = 30 * (rand.NextDouble() - 0.5);
                var s = KAMOPIS_SIZE * (rand.NextDouble() + 0.5);
                this.Kamopises.Add(new Kamopis(x, y, vx, vy, s, s));
            }
        }

        private const int KAMOPIS_SIZE = 200;

        private List<Kamopis> Kamopises = new List<Kamopis>();

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
