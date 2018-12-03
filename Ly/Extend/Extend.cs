using Ly.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ly.Extend
{
    public static class Extend
    {
        public static void ExtendLogClipName(this AudioClip audioClip)
        {
            Ly.DebugTool.Debug.Log(audioClip.name);
        }
    }
}
