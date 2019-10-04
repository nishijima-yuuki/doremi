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
    /// デスクトップアイコンを収集します。
    /// </summary>
    static class DesktopIconCollector
    {
        /// <summary>
        /// デスクトップのアイコンを収集します。
        /// </summary>
        /// <returns>DesktopIconオブジェクトの列挙。</returns>
        public static IEnumerable<DesktopIcon> Collect()
        {
            for (int i = 0; i < GetDesktopIconCount(); i++)
            {
                yield return CreateDesktopIcon(i);
            }
        }

        /// <summary>
        /// デスクトップ上のアイコン数を取得します。
        /// </summary>
        /// <returns>デスクトップ上のアイコン数。</returns>
        static int GetDesktopIconCount()
        {
            return (int)Win32Api.SendMessage(Win32Util.GetDesktopListView(), Win32Api.LVM_GETITEMCOUNT, 0, 0);
        }

        /// <summary>
        /// DesktopIconオブジェクトを生成します。
        /// </summary>
        /// <param name="index">デスクトップアイコンのインデックス。</param>
        /// <returns>DesktopIconオブジェクト。</returns>
        static DesktopIcon CreateDesktopIcon(int index)
        {
            string text = GetDesktopIconText(index);
            Point location = GetDesktopIconLocation(index);
            return new DesktopIcon(index, text, location);
        }

        /// <summary>
        /// デスクトップアイコンの座標を取得します。
        /// </summary>
        /// <param name="index">デスクトップアイコンのインデックス。</param>
        /// <returns>デスクトップアイコンの座標。</returns>
        static Point GetDesktopIconLocation(int index)
        {
            using (var mem = DesktopListViewMemory.Allocate(IntPtr.Zero, Marshal.SizeOf(Point.Empty), Win32Api.MEM_COMMIT, Win32Api.PAGE_READWRITE))
            {
                Point result = Point.Empty;
                Win32Api.SendMessage(Win32Util.GetDesktopListView(), Win32Api.LVM_GETITEMPOSITION, index, mem.Ptr);

                if (!Win32Api.ReadProcessMemory(mem.ProcessPtr, mem.Ptr, out result, Marshal.SizeOf(result), IntPtr.Zero))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                return result;
            }
        }

        /// <summary>
        /// デスクトップアイコンのテキストを取得します。
        /// </summary>
        /// <param name="index">デスクトップアイコンのインデックス。</param>
        /// <returns>デスクトップアイコンのテキスト。</returns>
        static string GetDesktopIconText(int index)
        {
            using (var lviMem = DesktopListViewMemory.Allocate(IntPtr.Zero, Marshal.SizeOf(new Win32Api.LVITEM()), Win32Api.MEM_COMMIT, Win32Api.PAGE_READWRITE))
            {
                using (var txtMem = DesktopListViewMemory.Allocate(IntPtr.Zero, sizeof(char) * (Win32Api.MAX_LVMSTRING + 1), Win32Api.MEM_COMMIT, Win32Api.PAGE_READWRITE))
                {
                    // テキストを受け取るためのLVITEM構造体を生成しリストビューのプロセスのメモリにコピー。
                    Win32Api.LVITEM listviewItem = new Win32Api.LVITEM() { cchTextMax = Win32Api.MAX_LVMSTRING, iSubItem = 0, pszText = txtMem.Ptr };
                    Win32Api.WriteProcessMemory(lviMem.ProcessPtr, lviMem.Ptr, ref listviewItem, Marshal.SizeOf(listviewItem), IntPtr.Zero);

                    // テキスト取得
                    Win32Api.SendMessage(Win32Util.GetDesktopListView(), Win32Api.LVM_GETITEMTEXTW, index, lviMem.Ptr);

                    // string型に変換
                    var strBuf = new StringBuilder(Win32Api.MAX_LVMSTRING);
                    if (!Win32Api.ReadProcessMemory(txtMem.ProcessPtr, txtMem.Ptr, strBuf, strBuf.Capacity, IntPtr.Zero))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    return strBuf.ToString();
                }
            }
        }
    }
}
