using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace doremi
{
    /// <summary>
    /// デスクトップのリストビューのメモリラッパー。
    /// </summary>
    class DesktopListViewMemory : IDisposable
    {
        /// <summary>
        /// 確保したメモリのポインタ。
        /// </summary>
        public IntPtr Ptr { get; protected set; } = IntPtr.Zero;

        /// <summary>
        /// メモリを確保するプロセスのポインタ。
        /// </summary>
        public IntPtr ProcessPtr { get; protected set; } = IntPtr.Zero;

        /// <summary>
        /// VirtualAllocExのラッパー。メモリ割り当てに失敗するとWin32Exceptionをスローします。
        /// </summary>
        public static DesktopListViewMemory Allocate(IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect)
        {
            int pid;
            Win32Api.GetWindowThreadProcessId(Win32Util.GetDesktopListView(), out pid);
            var hProc = Win32Api.OpenProcess(Win32Api.PROCESS_VM_OPERATION |
                                             Win32Api.PROCESS_VM_READ |
                                             Win32Api.PROCESS_VM_WRITE |
                                             Win32Api.PROCESS_QUERY_INFORMATION, false, pid);

            if (hProc == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var result = new DesktopListViewMemory();
            result.ProcessPtr = hProc;
            result.Ptr = Win32Api.VirtualAllocEx(hProc, lpAddress, dwSize, flAllocationType, flProtect);
            if (result.Ptr == IntPtr.Zero)
            {
                result.Dispose();
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return result;
        }

        /// <summary>
        /// 初期化します。
        /// </summary>
        protected DesktopListViewMemory()
        {
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                if (Ptr != IntPtr.Zero)
                {
                    Win32Api.VirtualFreeEx(ProcessPtr, Ptr, 0, Win32Api.MEM_RELEASE);
                    Ptr = IntPtr.Zero;
                }

                if (ProcessPtr != IntPtr.Zero)
                {
                    Win32Api.CloseHandle(ProcessPtr);
                    ProcessPtr = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~DesktopListViewMemory()
        {
          // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
          Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
