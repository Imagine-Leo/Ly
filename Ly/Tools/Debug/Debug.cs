using System;

namespace Ly.Tools
{
    public class Debug : SingleInstance<Debug>
    {
        public static RunTimeEnvironment runTimeEnvironment = RunTimeEnvironment.ConsoleView;
        public static string logPrefix = "";
        public static string warningPrefix = "!";
        public static string errorPrefix = "!!!!";
        public static string consolePrefix = ">>>>";

        public Debug()
        {
            DllLog("Init Debug instance!");
            if (runTimeEnvironment == RunTimeEnvironment.ConsoleView)
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
            }
        }

        public void DllLog(string str, LogType logType = LogType.Console)
        {
            if (runTimeEnvironment == RunTimeEnvironment.Unity)
            {
                Log(str, "cyan", "Dll:");
            }

            else if (runTimeEnvironment == RunTimeEnvironment.ConsoleView)
            {
                Console.WriteLine(str);
                Console.WriteLine(new string('=', 40));
            }
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