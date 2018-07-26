using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Type_TableToModelList
{
    /// <summary>
    /// 利用映射来实现
    /// </summary>
    class Program
    {
        static SQLDatabase db = new SQLDatabase();
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //从数据库中获取的表
            DataTable dataTable = GetDbTable();
            //从自定义表中获取的表
            //DataTable dataTable = GetTable();
            List<People> peoples = (List<People>)dataTable.TableToList<People>();

            Console.WriteLine("展示集合中的结果集：");
            foreach (var people in peoples)
            {
                Console.WriteLine("年龄是"+people.Age+"，姓名是"+people.Name);
            }

            Console.WriteLine("DataTable结果集的展示：");

            var listToTable = peoples.ListToTable<People>();

            foreach (DataRow peopleRow in listToTable.Rows)
            {
                Console.WriteLine("年龄是" + peopleRow["Age"] + "，姓名是" + peopleRow["Name"]);
            }

            Console.WriteLine();
            Console.WriteLine("展示一个对象的数据");

            People p = new People();
            p = listToTable.TableToModel<People>(p);
            Console.WriteLine("年龄是" + p.Age + "，姓名是" + p.Name);

            Console.WriteLine("某张表中的某一行转为实体类");
            People p1 = new People();
            p1=listToTable.Rows[2].DataRowToModel(p1);
            Console.WriteLine("年龄是" + p1.Age + "，姓名是" + p1.Name);

            Console.WriteLine("实体转为某张表中的某一行");
            DataRow row = p.ToDataRow(listToTable);
            Console.WriteLine("年龄是" + row["Age"] + "，姓名是" + row["Name"]);
            Console.ReadKey();
        }
        /// <summary>
        /// 自定义表
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTable()
        {
            DataTable dataTable = new DataTable();
            //列的类型默认是字符串类型，如果想要映射成对应的类型，必须加上指定的类型
            dataTable.Columns.AddRange(new DataColumn[] { new DataColumn("Age",typeof(int)), new DataColumn("Id",typeof(int)), new DataColumn("Name") });         
            DataRow row = dataTable.NewRow();
            row["Age"] = 24;
            row["Id"] = 1;
            row["Name"] = "zjx";
            dataTable.Rows.Add(row);

            return dataTable;
        }
        /// <summary>
        /// 从数据库获取的表
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDbTable()
        {
            string sql = "select * from T_People";
            DataTable table = db.QueryDataTable(sql);

            return table;
            
        }
        /// <summary>
        /// 通过反射获得一个对象的方法
        /// </summary>
        private static void GetReMethod()
        {
            Math_Type m = new Math_Type();

            Type type = typeof(Math_Type);

            MethodInfo type_method_add = type.GetMethod("Add", new Type[] { typeof(int), typeof(int) });

            if (type_method_add != null)
            {
                //invoke 函数中如果类型是静态的就不需要传入第一个参数
                var d = type_method_add.Invoke(m, new object[] { 12, 23 });
                Console.WriteLine(d);
            }
        }
    }
}
