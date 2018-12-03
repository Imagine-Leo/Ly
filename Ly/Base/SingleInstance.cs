using Ly.DebugTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = Ly.DebugTool.Debug;

namespace Ly.Base
{
    public class SingleInstance<T> where T : new()
    {
        private static T instance = default(T);
        private static object objectLock = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    object obj;
                    Monitor.Enter(obj = objectLock);
                    try
                    {
                        if (instance == null)
                            instance = default(T) == null ? Activator.CreateInstance<T>() : default(T);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(ex.Message);
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return instance;
            }
        }
        private SingleInstance() { }
    }
    public class SingleInstanceComponent<T> where T : MonoBehaviour
    {
        private static T instance = null;
        private static object objectLock = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                    Ly.DebugTool.Debug.LogError("instance null Error,Waiting Awake init...");
                return instance;
            }
            private set
            {
                instance = value;
            }
        }
        public virtual void Awake()
        {
            if (instance == null)
                instance = this as T;
        }
        private SingleInstanceComponent() { }
    }
}
