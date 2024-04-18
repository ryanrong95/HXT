using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Linq.Extends
{
    class DataContextExcutor
    {
        static public object run(SqlDataReader reader)
        {
            var list = new List<object>();
            //var properties = typeof(T).GetProperties();
            //while (reader.Read())
            //{
            //    dynamic obj = new System.Dynamic.ExpandoObject();

            //    var index = 0;
            //    foreach (var property in properties)
            //    {
            //        ((IDictionary<string, object>)obj)[property.Name] = reader[index++];
            //    }
            //    list.Add(obj);
            //}

            //return list;

            return null;

        }
    }
}
