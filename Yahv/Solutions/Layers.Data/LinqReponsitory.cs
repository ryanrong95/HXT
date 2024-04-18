using System;
using System.Data.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.Collections.Concurrent;
using System.Data;

namespace Layers.Data
{
    /// <summary>
    /// 只在本地使用，抽象实现 Linq 基本支持类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract public class LinqReponsitory<T> : Linq.LinqReponsitory where T : DataContext, new()
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public LinqReponsitory()
        {
        }

        /// <summary>
        /// 不建议使用，构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        public LinqReponsitory(bool isAutoSumit) : base(isAutoSumit)
        {

        }

        /// <summary>
        /// 工厂构造器
        /// </summary>
        /// <param name="isAutoSumit">自动提交</param>
        /// <param name="isFactory">是否为工厂行数</param>
        public LinqReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        {

        }



        ///// <summary>
        ///// 统一连接
        ///// </summary>
        //IDbConnection connection;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="connection"></param>
        //protected LinqReponsitory(IDbConnection connection)
        //{
        //    this.connection = connection;
        //    var attribute = typeof(T).GetCustomAttribute<DatabaseAttribute>(true);
        //    //connection.ChangeDatabase(attribute.Name);
        //}

        //protected LinqReponsitory(bool isAutoSumit, bool isFactory) : base(isAutoSumit, isFactory)
        //{

        //}


        static ConcurrentDictionary<Type, string> connectionStrings;

        static LinqReponsitory()
        {
            connectionStrings = new ConcurrentDictionary<Type, string>();
        }

        /// <summary>
        /// 带锁实现数据上线文连接实例化
        /// </summary>
        /// <returns>LINQ to SQL 框架的主入口点</returns>
        protected override DataContext InitDataContext()
        {
            Type current = typeof(T);
            //if (this.connection != null)
            //{
            //    return Activator.CreateInstance(current, this.connection) as T;
            //}

            string connection = connectionStrings.GetOrAdd(current, type =>
            {
                var attribute = type.GetCustomAttribute<DatabaseAttribute>(true);

                var name = attribute.Name;

                if (name.EndsWith("Overalls", StringComparison.OrdinalIgnoreCase))
                {
                    name = "Overalls";
                }

                var configer = ConfigurationManager.ConnectionStrings[name + "ConnectionString"];

                if (attribute == null)
                {
                    throw new Exception("DatabaseAttribute  is not exsit!");
                }

                if (configer == null)
                {
                    throw new Exception($"config:{name}ConnectionString  is not exsit!");
                }

                return configer.ConnectionString;
            });
            return Activator.CreateInstance(current, connection) as T;
        }

        /// <summary>
        /// 只读连接者
        /// </summary>
        public T Current
        {
            get
            {
                return base.DataContext as T;
            }
        }
    }
}
