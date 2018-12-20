using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Debug = Ly.DebugTool.Debug;


namespace Ly.CMDConsole
{
    //http://blog.csdn.net/cartzhang/article/details/49884507
    /// <summary>
    /// 使用方法：Initialize();
    ///           CMDUpdate()放入Update中
    /// </summary>
    public class CMDConsole
    {

        #region
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTitle(string lpConsoleTitle);
        #endregion

        private TextWriter oldOutput;
        public void Initialize(bool ISUNITY = false)
        {
            AllocConsole();
            oldOutput = Console.Out;

            try
            {
                IntPtr p = Win32.User32API.GetCurrentWindowHandle();
                Debug.Instance.DllLog("当前程序句柄" + p,DebugTool.LogType.UnityLog);
                Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(p, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                StreamWriter standardOutput = new StreamWriter(fileStream, Encoding.ASCII);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            catch (Exception e)
            {
                Debug.Instance.DllLog("Couldn't redirect output: " + e.Message,DebugTool.LogType.UnityLogError);
            }
        }
        public void Shutdown()
        {
            Console.SetOut(oldOutput);
            FreeConsole();
        }
        public void SetTitle(string strName)
        {
            SetConsoleTitle(strName);
        }
        //===========================================================输入===========================================================================
        //public delegate void InputText( string strInput );  
        public event System.Action<string> OnInputText;
        public string inputString;
        public void RedrawInputLine()
        {
            if (inputString.Length == 0) return;
            if (Console.CursorLeft > 0) _ClearLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(inputString);
        }

        public void CMDUpdate()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                _ClearLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("> " + inputString);

                var strtext = inputString;
                inputString = "";

                OnInputText?.Invoke(strtext);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (inputString.Length < 1) return;

                inputString = inputString.Substring(0, inputString.Length - 1);
                RedrawInputLine();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _ClearLine();
                inputString = "";
                return;
            }
            //空格
            if (Input.GetKeyDown(KeyCode.Space))
            {
                inputString += " ";
                RedrawInputLine();
                return;
            }
        }
        private void _ClearLine()
        {
            // System.Text.Encoding test = Console.InputEncoding;  
            Console.CursorLeft = 0;
            Console.Write(new String(' ', Console.BufferWidth));
            Console.CursorTop--;
            Console.CursorLeft = 0;
        }
    }
}
