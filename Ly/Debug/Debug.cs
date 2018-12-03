using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ly.DebugTool
{
    public class Debug
    {
        public static RunTimeEnvironment runTimeEnvironment = RunTimeEnvironment.ConsoleView;
        public static string logPrefix = "";
        public static string warningPrefix = "!";
        public static string errorPrefix = "!!!!";
        public static string consolePrefix = ">>>>";

        private static object getLock = new object();
        private static object setLock = new object();

        private static Debug _Instance = null;
        public static Debug Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (getLock)
                    {
                        if (_Instance == null)
                            _Instance = new Debug();
                    }
                }

                return _Instance;
            }
            set
            {
                lock (setLock)
                {
                    _Instance = value;
                }
            }
        }
        private Debug()
        {
            DllLog("init");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void ABCDebug(string str, string color = "white", LogType logType = LogType.UnityLog)
        {
            if (string.IsNullOrEmpty(str))
                return;
            switch (logType)
            {
                case LogType.UnityLog:
                    Log(str, color, logPrefix);
                    break;
                case LogType.UnityLogWarning:
                    LogWarning(str, color, warningPrefix);
                    break;
                case LogType.UnityLogError:
                    LogError(str, color, errorPrefix);
                    break;
                case LogType.Console:
                    Console.WriteLine(consolePrefix + str);
                    break;
                case LogType.None:
                    break;
                default:
                    break;
            }
        }

        public void DllLog(string str,LogType logType=LogType.Console)
        {
#if UNITY_EDITOR
            Log(str,"cyan","Dll:");
#else
            Console.WriteLine(str);
            Console.WriteLine(new string('=', 40));
#endif
        }

        public static void Log(string str, string color = "white", string prefix = "")
        {
            UnityEngine.Debug.Log(string.Format("<color={0}>" + prefix + str + "</color>", color));
        }
        public static void LogWarning(string str, string color = "yellow", string prefix = "!")
        {
            UnityEngine.Debug.Log(string.Format("<color={0}>" + prefix + str + "</color>", color));
        }
        public static void LogError(string str, string color = "red", string prefix = "!!!!")
        {
            UnityEngine.Debug.LogError(string.Format("<color={0}>" + prefix + str + "</color>", color));
        }
    }
}