using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeCode_ex1
{
    class Program
    {
        static void Main(string[] args)
        {
            Love love = new Love();
            Type type = love.GetType();
            Object obj = type.InvokeMember(null,
                BindingFlags.DeclaredOnly |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.CreateInstance, null, null, args);
            //调用没有返回值的方法
            type.InvokeMember("Display", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { "zjx" });
            //调用有返回值的方法
            int i = (int)type.InvokeMember("GetInt", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { 1 });
            Console.WriteLine("有返回值的方法："+i);
            //设置属性值
            type.InvokeMember("Name", BindingFlags.SetProperty, null, obj, new string[] { "abc" });
            //获取属性值
            string str = (string)type.InvokeMember("Name", BindingFlags.GetProperty, null, obj, null);
            Console.WriteLine(str);
           //设置字段值
            type.InvokeMember("field1", BindingFlags.SetField, null, obj, new object[] { 444 });
            //获取字段值
            int f = (int)type.InvokeMember("field1", BindingFlags.GetField, null, obj, null);
            Console.WriteLine(f);
            Console.ReadLine();
        }
    }

    class Love    
    {
        public int field1;

        private string _name;
        public Love()
        {

        }

        public string Name
        {
            get
            {
                return _name;
            }
           set
            {
                _name = value;
            }
        }

        public int GetInt(int a)
        {
            return a;
        }

        public void Display(string str)
        {
            Console.WriteLine("无返回值的方法"+str);
        }

    }
}
