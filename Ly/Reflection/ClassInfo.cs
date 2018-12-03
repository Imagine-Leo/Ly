using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using Newtonsoft.Json;
using Debug = Ly.DebugTool.Debug;


//http://www.cnblogs.com/zuozuo/archive/2011/09/29/2195309.html
namespace Ly.Reflection
{
    public class ClassInfo
    {
        public static void Test()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            InvokeMemberFunc(typeof(DebugTool.Debug), "DllLog");
        }
        public static void DebugAllInfo(Type type)
        {
            MemberInfo[] minss = type.GetMembers(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly);

            foreach (MemberInfo item in minss)
            {
                Debug.Instance.DllLog(String.Format("{0}", item));
            }
        }

        /// <summary>
        /// 方式1使用公共的构造函数实例化，带一个参数
        ///     使用程序集Assembly.CreateInstance()进行实例化
        ///     第一个参数：代表了要创建的类型实例的字符串名称
        ///     第二个参数：说明是不是大小写无关(Ignore Case)
        ///     第三个参数：在这里指定Default，意思是不使用BingdingFlags的策略(你可以把它理解成null，但是BindingFlags是值类型，所以不可能为null，必须有一个默认值，而这个Default就是它的默认值)；
        ///     第四个参数：是Binder，它封装了CreateInstance绑定对象(Calculator)的规则，我们几乎永远都会传递null进去，实际上使用的是预定义的DefaultBinder；
        ///     第五个参数：是一个Object[]数组类型，它包含我们传递进去的参数，有参数的构造函数将会使用这些参数；
        ///     第六个参数：是一个CultureInfo类型，它包含了关于语言和文化的信息(简单点理解就是什么时候ToString("c")应该显示“￥”，什么时候应该显示“＄”)。
        ///     第七个参数：是一个object[]数组，描述特性
        ///  方式2用的是 Activator.CreateInstance()进行实例化，返回一个ObjectHandle类的对象
        ///     需要Unwrap()才能返回object对象
        ///     Activator.CreateInstance()的参数说明
        ///     第一个参数：当前程序集的全名称，字符串的形式
        ///     第二个参数：代表了要创建的类型实例的字符串名称
        ///     第三个参数：说明是不是大小写无关(Ignore Case)
        ///     第四个参数：BindingFlags
        ///                   Default，意思是不使用BingdingFlags的策略
        ///                   NonPublic指定是非公共的类型
        ///     第五个参数：是Binder，它封装了CreateInstance绑定对象(Calculator)的规则，我们几乎永远都会传递null进去，实际上使用的是预定义的DefaultBinder；
        ///     第六个参数：是一个Object[]数组类型，它包含我们传递进去的参数，有参数的构造函数将会使用这些参数；
        /// </summary>
        /// <param name="type"></param>
        public static void InvokeMemberFunc(Type type, string funcName)
        {
            //公共构造函数创建
            object target;
            try
            {
                target = Assembly.GetExecutingAssembly().CreateInstance(type.FullName, true, BindingFlags.Default, null, new object[] { }, null, null);
            }
            catch (Exception ex)
            {
                Debug.Instance.DllLog("公共构造函数创建:" + ex.Message);
                Debug.Instance.DllLog("私有构造函数创建:");
                //私有构造函数创建
                target = Activator.CreateInstance(null, type.FullName, true, BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic, null, null, null, null, null).Unwrap();
            }
            try
            {
                Debug.Instance.DllLog("共有函数调用:");
                type.InvokeMember(funcName, BindingFlags.InvokeMethod, null, target, new string[] { "TestLog" }, null);
            }
            catch
            {
                Debug.Instance.DllLog("私有函数调用:");
                type.InvokeMember(funcName, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, target, new string[] { "TestLog" }, null);
            }
        }
    }
}
