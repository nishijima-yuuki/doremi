using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace doremi
{
    static class Win32Util
    {
        /// <summary>
        /// デスクトップのリストビューのウィンドウハンドルを取得します。
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetDesktopListView()
        {
            var hWnd = Win32Api.FindWindow("Progman", "Program Manager");
            hWnd = Win32Api.FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
            return Win32Api.FindWindowEx(hWnd, IntPtr.Zero, "SysListView32", null);
        }

        /// <summary>
        /// SendMessageに渡すパラメータを返します。
        /// </summary>
        /// <param name="param1">パラメータ1。</param>
        /// <param name="param2">パラメータ2。</param>
        /// <returns></returns>
        public static int MAKELPARAM(int param1, int param2)
        {
            return ((param2 << 16) | (param1 & 0xFFFF));
        }
    }
}
