using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ly.DebugTool
{
    public enum LogType
    {
        UnityLog=1,
        UnityLogWarning=2,
        UnityLogError=3,
        Console=4,
        None=5
    }
    public enum RunTimeEnvironment
    {
        Unity=1,
        ConsoleView=2
    }
}
