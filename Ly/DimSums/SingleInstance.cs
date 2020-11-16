using System;
using System.Threading;
using UnityEngine;
using Debug = Ly.Tools.Debug;
using Object = UnityEngine.Object;

namespace Ly
{
    public class SingleInstance<T> where T : new()
    {
        private static T _instance;
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
                        Debug.LogError(ex.Message);
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }

                return _instance;
            }
        }
    }

    public class SingleInstanceComponent<T> where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    Debug.Log($"init manager {typeof(T).Name}");
                    instance = Object.FindObjectOfType<T>();
                    if (!instance)
                    {
                        instance = new GameObject().AddComponent<T>();
                        instance.gameObject.name = typeof(T).ToString();
                    }
                }

                return instance;
            }
            protected set { instance = value; }
        }

        public void Awake()
        {
            if (instance == null)
                instance = this as T;
        }
    }
}