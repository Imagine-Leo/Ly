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
        private static T _instance = default(T);
        private static object objectLock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    object obj;
                    Monitor.Enter(obj = objectLock);
                    try
                    {
                        if (_instance == null)
                            _instance = default(T) == null ? Activator.CreateInstance<T>() : default(T);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }
        public SingleInstance() { }
    }
    public class SingleInstanceComponent<T> where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    Ly.DebugTool.Debug.LogError("instance null Error,Waiting Awake init...");
                return instance;
            }
            protected set
            {
                instance = value;
            }
        }
        public void Awake()
        {
            if (instance == null)
                instance = this as T;
            else
            {
                UnityEngine.Debug.LogError("instance is already have one,i will destory this");
            }
        }
        public SingleInstanceComponent() { }
    }
}
