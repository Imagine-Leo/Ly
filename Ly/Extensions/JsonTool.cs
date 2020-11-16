using System;
using Ly.Tools;
using Newtonsoft.Json.Linq;

namespace Ly
{
    public static class JsonTool
    {
        public static string[] GetValuesByToken(string[] jTokens, string fromData)
        {
            try
            {
                var res = JToken.Parse(fromData);
                var values = new string[jTokens.Length];
                for (var i = 0; i < jTokens.Length; i++) values[i] = res[jTokens[i]].ToString();

                return values;
            }
            catch (Exception ex)
            {
                Debug.Instance.DllLog("jToken error:" + ex.Message, LogType.UnityLogWarning);
                return null;
            }
        }
    }
}