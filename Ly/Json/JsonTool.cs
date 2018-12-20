using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ly.Json
{
    public class JsonTool : Ly.Base.SingleInstance<JsonTool>
    {

        public static string[] GetVauleByJtoken(string[] jtokens, string fromData)
        {
            try
            {
                var res = JToken.Parse(fromData);
                string[] values = new string[jtokens.Length];
                for (int i = 0; i < jtokens.Length; i++)
                {
                    values[i] = res[jtokens[i]].ToString();
                }
                return values;
            }
            catch (Exception ex)
            {
                DebugTool.Debug.Instance.DllLog("jtoken error:" + ex.Message, DebugTool.LogType.UnityLogWarning);
                return null;
            }
        }
    }
}
