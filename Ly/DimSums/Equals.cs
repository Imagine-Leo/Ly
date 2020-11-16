using System;
using System.Collections.Generic;
using Ly.Tools;
using Newtonsoft.Json;

namespace Ly
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

            if (other.GetType() != GetType())
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
                var type = other.GetType();
                Debug.Instance.DllLog("Type:" + JsonConvert.SerializeObject(type));
                filedList = ClassInfo.ClassAllMembersInfo(type);
                for (var i = 0; i < filedList.Count; i++)
                    if (filedList[i].StartsWith("."))
                    {
                        filedList.RemoveAt(i);
                        i--;
                    }

                Debug.Instance.DllLog(JsonConvert.SerializeObject(filedList));
            }

            var str1 = JsonConvert.SerializeObject(this);
            var str2 = JsonConvert.SerializeObject(other);
            return StringArrayEquals(filedList, JsonTool.GetValuesByToken(filedList.ToArray(), str1),
                JsonTool.GetValuesByToken(filedList.ToArray(), str2)
            );
        }

        private bool StringArrayEquals(List<string> keys, string[] str1, string[] str2)
        {
            if (str1 == null || str2 == null)
                return false;
            if (str1.Length != str2.Length)
                return false;
            for (var i = 0; i < str1.Length; i++)
                if (str1[i] != str2[i])
                {
                    Console.WriteLine(keys[i] + ": " + str1[i] + " != " + str2[i]);
                    return false;
                }
                else
                {
                    Console.WriteLine(keys[i] + ": " + str1[i] + " = " + str2[i]);
                }

            return true;
        }
    }
}