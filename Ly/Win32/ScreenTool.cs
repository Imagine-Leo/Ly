﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ly.Win32
{
    public class ScreenTool
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public static void MaxScreen()
        {
            ShowWindow(User32API.GetCurrentWindowHandle(), 3);
        }
        public static void MinScreen()
        {
            ShowWindow(User32API.GetCurrentWindowHandle(), 7);
        }
        public int GetCurrentWindowHandle()
        {
            return (int)User32API.GetCurrentWindowHandle();
        }
    }
}
