﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ly.Tools
{
    internal class User32API
    {
        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        private static Hashtable processWnd;

        static User32API()
        {
            if (processWnd == null) processWnd = new Hashtable();
        }

        public static IntPtr GetCurrentWindowHandle()
        {
            var ptrWnd = IntPtr.Zero;
            var uiPid = (uint) Process.GetCurrentProcess().Id; // 当前进程 ID
            var objWnd = processWnd[uiPid];
            if (objWnd != null)
            {
                ptrWnd = (IntPtr) objWnd;
                if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd)) // 从缓存中获取句柄
                    return ptrWnd;

                Debug.Instance.DllLog("再次查询null", LogType.UnityLog);
                ptrWnd = IntPtr.Zero;
            }

            var bResult = EnumWindows(EnumWindowsProc, uiPid);
            // 枚举窗口返回 false 并且没有错误号时表明获取成功
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = processWnd[uiPid];
                if (objWnd != null) ptrWnd = (IntPtr) objWnd;
            }
            else
            {
                Debug.Instance.DllLog("error", LogType.UnityLogError);
            }

            return ptrWnd;
        }

        private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint uiPid = 0;
            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, out uiPid);
                if (uiPid == lParam) // 找到进程对应的主窗口句柄
                {
                    processWnd[uiPid] = hwnd; // 把句柄缓存起来
                    SetLastError(0); // 设置无错误
                    Debug.Instance.DllLog("主题名字" + Process.GetCurrentProcess().MainWindowTitle, LogType.UnityLog);
                    return false; // 返回 false 以终止枚举窗口
                }
            }

            return true;
        }

        #region

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(uint dwErrCode);

        #endregion
    }
}