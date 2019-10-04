using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace doremi
{
    /// <summary>
    /// デスクトップ上のアイコン。
    /// </summary>
    class DesktopIcon
    {
        /// <summary>
        /// アイコンのインデックスを取得・設定します。
        /// </summary>
        private int Index { get; set; }

        /// <summary>
        /// アイコンの位置を取得します。
        /// </summary>
        public Point Location { get; protected set; } = Point.Empty;

        /// <summary>
        /// アイコンのテキストを取得します。
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="index">アイコンのインデックス。</param>
        /// <param name="text">アイコンのテキスト。</param>
        /// <param name="locaton">アイコンの座標。</param>
        public DesktopIcon(int index, string text, Point location)
        {
            Index = index;
            Text = text;
            Location = location;
        }

        /// <summary>
        /// アイコンの座標を移動します。
        /// </summary>
        /// <param name="x">アイコンのX座標。</param>
        /// <param name="y">アイコンのY座標。</param>
        public void SetLocation(int x, int y)
        {
            var hWnd = Win32Util.GetDesktopListView();
            var sendMessageRet = Win32Api.SendMessage(hWnd, Win32Api.LVM_SETITEMPOSITION, Index, Win32Util.MAKELPARAM(x, y));
            if (sendMessageRet == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}
