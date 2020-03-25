using System;
using System.Runtime.InteropServices;

namespace KamopisScreensaver
{
    /// <summary>
    /// Win32API Wrapper
    /// </summary>
    /// <see cref="http://pinvoke.net/"/>
    public static class PInvoke
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        // This helper static method is required because the 32-bit version of user32.dll does not contain this API
        // (on any versions of Windows), so linking the method will fail at run-time. The bridge dispatches the request
        // to the correct function (GetWindowLong in 32-bit mode and GetWindowLongPtr in 64-bit mode)
        public static IntPtr SetWindowLongPtr(HandleRef hWnd, WindowLongFlags nIndex, IntPtr dwNewLong) =>
            IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(HandleRef hWnd, WindowLongFlags nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, WindowLongFlags nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, WindowLongFlags nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDPIAware();

        /// <summary>
        /// GetWindowLongPtr/SetWindowLongPtr で何の情報を取得/設定するのかを指定します
        /// DWLP_ は hWnd パラメータがダイアログボックスのハンドルである場合のみ使用可能
        /// </summary>
        /// <see cref="http://chokuto.ifdef.jp/urawaza/api/GetWindowLong.html"/>
        public enum WindowLongFlags : int
        {
            /// <summary>
            /// ウィンドウプロシージャのアドレス
            /// 直接ウィンドウプロシージャを呼び出すには CallWindowProc 関数を使う
            /// </summary>
            GWL_WNDPROC = -4,

            /// <summary>アプリケーションのインスタンスハンドル</summary>
            GWLP_HINSTANCE = -6,

            /// <summary>親ウィンドウのハンドル</summary>
            GWLP_HWNDPARENT = -8,

            /// <summary>ウィンドウ ID</summary>
            GWL_ID = -12,

            /// <summary>ウィンドウスタイル</summary>
            GWL_STYLE = -16,

            /// <summary>拡張ウィンドウスタイル</summary>
            GWL_EXSTYLE = -20,

            /// <summary>ウィンドウに関連付けられたアプリケーション定義の32ビット値</summary>
            GWL_USERDATA = -21,

            /// <summary>ダイアログボックスプロシージャで処理されたメッセージの戻り値</summary>
            DWLP_MSGRESULT = 0x0,

            /// <summary>
            /// ダイアログボックスプロシージャのアドレス
            /// 直接ダイアログボックスプロシージャを呼び出すには CallWindowProc 関数を使う
            /// </summary>
            DWLP_DLGPROC = 0x4,

            /// <summary>アプリケーション定義の情報（ハンドルやポインタなど）</summary>
            DWLP_USER = 0x8,
        }

        /// <summary>
        /// Window Styles.
        /// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
        /// </summary>
        [Flags]
        public enum WindowStyles : uint
        {
            /// <summary>The window has a thin-line border.</summary>
            WS_BORDER = 0x800000,

            /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
            WS_CAPTION = 0xc00000,

            /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
            WS_CHILD = 0x40000000,

            /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
            WS_CLIPCHILDREN = 0x2000000,

            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
            /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            /// </summary>
            WS_CLIPSIBLINGS = 0x4000000,

            /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
            WS_DISABLED = 0x8000000,

            /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
            WS_DLGFRAME = 0x400000,

            /// <summary>
            /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
            /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// </summary>
            WS_GROUP = 0x20000,

            /// <summary>The window has a horizontal scroll bar.</summary>
            WS_HSCROLL = 0x100000,

            /// <summary>The window is initially maximized.</summary>
            WS_MAXIMIZE = 0x1000000,

            /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
            WS_MAXIMIZEBOX = 0x10000,

            /// <summary>The window is initially minimized.</summary>
            WS_MINIMIZE = 0x20000000,

            /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
            WS_MINIMIZEBOX = 0x20000,

            /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
            WS_OVERLAPPED = 0x0,

            /// <summary>The window is an overlapped window.</summary>
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

            /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
            WS_POPUP = 0x80000000u,

            /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

            /// <summary>The window has a sizing border.</summary>
            WS_SIZEFRAME = 0x40000,

            /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
            WS_SYSMENU = 0x80000,

            /// <summary>
            /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
            /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            /// </summary>
            WS_TABSTOP = 0x10000,

            /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
            WS_VISIBLE = 0x10000000,

            /// <summary>The window has a vertical scroll bar.</summary>
            WS_VSCROLL = 0x200000
        }
    }
}
