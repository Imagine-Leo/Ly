using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Ly.Tools.CMDConsole
{
    //http://blog.csdn.net/cartzhang/article/details/49884507
    /// <summary>
    ///     使用方法：Initialize();
    ///     CMDUpdate()放入Update中
    /// </summary>
    public class CMDConsole
    {
        public string inputString;

        private TextWriter oldOutput;

        public void Initialize(bool ISUNITY = false)
        {
            AllocConsole();
            oldOutput = Console.Out;

            try
            {
                var p = User32API.GetCurrentWindowHandle();
                Debug.Instance.DllLog("当前程序句柄" + p, LogType.UnityLog);
                var safeFileHandle = new SafeFileHandle(p, true);
                var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                var standardOutput = new StreamWriter(fileStream, Encoding.ASCII);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            catch (Exception e)
            {
                Debug.Instance.DllLog("Couldn't redirect output: " + e.Message, LogType.UnityLogError);
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
        public event Action<string> OnInputText;

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
            }
        }

        private void _ClearLine()
        {
            // System.Text.Encoding test = Console.InputEncoding;
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.BufferWidth));
            Console.CursorTop--;
            Console.CursorLeft = 0;
        }

        #region

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        #endregion
    }
}