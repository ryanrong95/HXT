using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq;
using System.Data;
using System.Reflection;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.Collections.Concurrent;

namespace Layer.Data
{
    abstract public class LinqReponsitory<T> : Linq.LinqReponsitory where T : DataContext, new()
    {
        public LinqReponsitory()
        {
        }

        public LinqReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }

        protected override DataContext InitDataContext()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<DatabaseAttribute>(true);

            string connection = ConfigurationManager.ConnectionStrings[attribute.Name + "ConnectionString"].ConnectionString;
            return Activator.CreateInstance(typeof(T), connection) as T;
        }
    }
}
