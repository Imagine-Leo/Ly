using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = Ly.DebugTool.Debug;
using Ly.DebugTool;
using Newtonsoft.Json;
using System.Reflection;
using Ly.Json;

namespace Ly.Base
{
    public class EqualsClass<T> : IEquatable<T>
    {
        bool IEquatable<T>.Equals(T other)
        {
            if (ReferenceEquals(null, other))
            {
                Debug.Instance.DllLog("arg is null", LogType.UnityLogWarning);
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                Debug.Instance.DllLog("same Ref", LogType.UnityLogWarning);
                return true;
            }

            if (other.GetType() != this.GetType())
            {
                Debug.Instance.DllLog("type is different", LogType.UnityLogWarning);
                return false;
            }
            return Equals(other);

        }

        public bool Equals(object other, List<string> filedList = null)
        {
            if (filedList == null)
            {
                filedList = new List<string>();
                Type type = other.GetType();
                Debug.Instance.DllLog("Type:" + JsonConvert.SerializeObject(type));
                filedList = Ly.Reflection.ClassInfo.ClassAllMembersInfo(type);
                for (int i = 0; i < filedList.Count; i++)
                {
                    if (filedList[i].StartsWith("."))
                    {
                        filedList.RemoveAt(i);
                        i--;
                    }
                }
                Debug.Instance.DllLog(JsonConvert.SerializeObject(filedList));
            }
            string str1 = JsonConvert.SerializeObject(this);
            string str2 = JsonConvert.SerializeObject(other);
            return StringArrayEquals(filedList, JsonTool.GetVauleByJtoken(filedList.ToArray(), str1),
                JsonTool.GetVauleByJtoken(filedList.ToArray(), str2)
                );
        }

        private bool StringArrayEquals(List<string> keys, string[] str1, string[] str2)
        {
            if (str1 == null || str2 == null)
                return false;
            if (str1.Length != str2.Length)
                return false;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    Console.WriteLine(keys[i] + ": " + str1[i] + " != " + str2[i]);
                    return false;
                }
                else
                {
                    Console.WriteLine(keys[i] + ": " + str1[i] + " = " + str2[i]);
                }
            }
            return true;
        }
    }
}
