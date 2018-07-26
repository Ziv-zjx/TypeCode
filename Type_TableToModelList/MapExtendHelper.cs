using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Type_TableToModelList
{
    public static class MapExtendHelper
    {

        /*
        Typeof()是运算符而GetType是方法
        GetType()是基类System.Object的方法，因此只有建立一个实例之后才能够被调用（初始化以后）
        Typeof()的参数只能是int,string,String,自定义类型，且不能是实例
 
        GetType() 和typeof()都返回System.Type的引用。
 
        TypeOf() 和GetType()的区别:  
        (1)TypeOf():得到一个Class的Type
        (2)GetType():得到一个Class的实例的Type
        */
        /// <summary>
        /// 扩展方法，将表转换为List集合
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IList<ModelT> TableToList<ModelT>(this DataTable dataTable) where ModelT : new()
        {
            if (dataTable != null)
            {
                IList<ModelT> modelTs = new List<ModelT>();

                foreach(DataRow row in dataTable.Rows)
                {
                    ModelT model = new ModelT();

                    Type type = typeof(ModelT);
                    PropertyInfo[] propertyInfos = type.GetProperties();
                    
                    foreach(var property in propertyInfos)
                    {
                        if (dataTable.Columns.Contains(property.Name))
                        {
                            object value = row[property.Name];

                            if (!property.CanWrite)
                                continue;
                            if (value != DBNull.Value)
                                ////SetValue值第一个参数指定是当前实例对象
                                property.SetValue(model, value, null);
                        }
                    }

                    modelTs.Add(model);
                }         
                return modelTs;
            }
            else
            {
                return null;
            }
        }
       
        /// <summary>
        /// list集合转为DataTable
        /// </summary>
        /// <typeparam name="ModelT"></typeparam>
        /// <param name="modelTs"></param>
        /// <returns></returns>
        public static DataTable ListToTable<ModelT>(this List<ModelT> modelTs)
        {
            if (modelTs != null && modelTs.Count > 0)
            {
                DataTable dataTable = new DataTable();
                //获取class类型的Type
                Type type = typeof(ModelT);
                PropertyInfo[] propertys = type.GetProperties();

                //先创建一个空表的结构
                foreach(PropertyInfo property in propertys)
                {
                    if (!property.CanRead)
                        continue;
                    dataTable.Columns.Add(property.Name,property.PropertyType);                   
                }
                //像空表中添加数据
                foreach (var model in modelTs)
                {
                    //获取实例对象的Type
                    Type t = model.GetType();
                    DataRow row = dataTable.NewRow();
                    foreach (PropertyInfo property in t.GetProperties())
                    {
                        //GetValue值第一个参数指定是当前实例对象
                        row[property.Name] = property.GetValue(model, null);
                    }
                    dataTable.Rows.Add(row);
                }
                return dataTable;
            }
            else
            {
                return null;
            }
           
        }

        /// <summary>
        /// 表的第一行转为对应的实体类
        /// </summary>
        /// <typeparam name="ModelT"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static ModelT TableToModel<ModelT>(this DataTable dataTable,ModelT model)
        {
            if (dataTable != null && model != null && dataTable.Rows.Count > 0)
            {
                PropertyInfo[] propertyInfos = model.GetType().GetProperties();
                foreach(PropertyInfo propertyInfo in propertyInfos)
                {
                    if (dataTable.Columns.Contains(propertyInfo.Name))
                    {
                        if (propertyInfo.CanWrite)
                            propertyInfo.SetValue(model,dataTable.Rows[0][propertyInfo.Name],null);
                    }
                }

                return model;
            }
            return default(ModelT);
        }

        /// <summary>
        /// 表的的某一行转为对应的实体类
        /// </summary>
        /// <typeparam name="ModelT"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static ModelT DataRowToModel<ModelT>(this DataRow dataRow, ModelT model)
        {
            if (dataRow != null && model != null)
            {
                PropertyInfo[] propertyInfos = model.GetType().GetProperties();
                try
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        propertyInfo.SetValue(model,dataRow[propertyInfo.Name],null);
                    }
                }
                catch { }
                return model;
            }
            return default(ModelT);
        }
        /// <summary>
        /// 将实体类转为指定数据表中的某一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static DataRow ToDataRow<ModelT>(this ModelT model,DataTable dataTable)
        {
            DataRow dRow = dataTable.NewRow();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                if (dataTable.Columns.Contains(propertyInfo.Name))
                {
                    dRow[propertyInfo.Name] = propertyInfo.GetValue(model, null) == null ? "" : propertyInfo.GetValue(model, null);
                }
            return dRow;
        }
    }
}
