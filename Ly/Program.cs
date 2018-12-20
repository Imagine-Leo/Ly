using Ly.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.Runtime.Remoting;
using System.Configuration;

namespace Ly
{
    class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine(new string('-', 40));
            Console.WriteLine(ConfigurationManager.AppSettings["version"]);

            TestObj t1 = new TestObj();
            TestObj t2 = new TestObj();

            Console.WriteLine("Equals:{0}", Equals(t1,t2));
            Console.WriteLine("self Equals:{0}", t1.Equals(t2));
            Console.WriteLine("==:{0}", t1 == t2);
            t1.a = 1;
            Console.WriteLine("Equals:{0}",t1.Equals(t2));
            Console.ReadKey();
            return 0;
        }

        public class TestObj : EqualsClass<TestObj>
        {
            public int a;
            public string b;
            public object c;
        }
    }
}
