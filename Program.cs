using System;
using System.Threading;
using System.Windows.Forms;

namespace KamopisScreensaver
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (var mutex = new Mutex(false, Application.ProductName))
            {
                // 二重起動を禁止する
                if (!mutex.WaitOne(0, false)) return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                /// <see cref="http://www7b.biglobe.ne.jp/~whitetiger/cs/cs2010001.html"/>
                if (args.Length > 0)
                {
                    var option = args[0].ToLower().Trim().Substring(0, 2);
                    if (option == "/s")
                    {
                        // スクリーンセーバーを実行
                        foreach (var screen in Screen.AllScreens)
                        {
                            var screensaver = new ScreenForm(screen.Bounds);
                            screensaver.Show();
                        }
                        Application.Run();
                    }
                    else if (option == "/p")
                    {
                        // プレビュー画面を表示
                        // args[1] はプレビューウィンドウのハンドル(HWND)
                        Application.Run(new ScreenForm(new IntPtr(long.Parse(args[1]))));
                    }
                    else if (option == "/c")
                    {
                        // スクリーンセーバーのオプション表示
                    }
                }
                else
                {
                    // 引数なしの場合
                    // 渡される引数がない場合、これはユーザーがファイルを右クリックして
                    //「構成」を選んだときに発生します。通常はオプションフォームを表示します。
                }
            }
        }
    }
}