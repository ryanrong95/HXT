//using Layers.Linq.McStor;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data.Common;
//using System.Data.Linq;
//using System.Data.Linq.Mapping;
//using System.Data.Linq.SqlClient.Implementation;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace Layers.Linq
//{
//    /// <summary>
//    /// Sql 的所类型
//    /// </summary>
//    public enum SqlLockType
//    {
//        Nolock
//    }


//    /// <summary>
//    /// 只读表数据
//    /// </summary>
//    /// <typeparam name="T">对象类型</typeparam>
//    public class NolockTable<T> : IQueryable<T> where T : class
//    {
//        DataContext context;
//        IQueryable<T> iQuery;

//        /// <summary>
//        /// 目标Type
//        /// </summary>
//        static bool IsUnChanged { get; set; } = true;

//        public NolockTable(IQueryable<T> query)
//        {
//            this.iQuery = query;
//            this.context = query.GetType().
//                GetField(nameof(this.context), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(query) as DataContext;
//        }

//        public Type ElementType
//        {
//            get
//            {
//                return this.iQuery.ElementType;
//            }
//        }

//        public Expression Expression
//        {
//            get
//            {
//                return this.iQuery.Expression;
//            }
//        }

//        public IQueryProvider Provider
//        {
//            get
//            {
//                //return new McStor.DbQueryProvider(this.context.Connection); 
//                return new MyQueryProvider<T>(this.iQuery);
//                //return ((IQueryable<T>)this.table).Provider;
//            }
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }

//        /// <summary>
//        /// 已重写 ToString()
//        /// </summary>
//        /// <returns>linq 表达式</returns>
//        public override string ToString()
//        {
//            return this.iQuery.ToString();
//        }
//    }

//    /// <summary>
//    /// 只读表数据
//    /// </summary>
//    /// <typeparam name="T">对象类型</typeparam>
//    public class ReadTable<T> : IQueryable<T> where T : class
//    {
//        DataContext context;
//        Table<T> table;
//        public ReadTable(Table<T> table)
//        {
//            this.context = table.Context;
//            this.table = table;
//        }

//        public Type ElementType
//        {
//            get
//            {
//                return ((IQueryable<T>)this.table).ElementType;
//            }
//        }

//        public Expression Expression
//        {
//            get
//            {
//                return ((IQueryable<T>)this.table).Expression;
//            }
//        }

//        public IQueryProvider Provider
//        {
//            get
//            {
//                //return new MyQueryProvider(((IQueryable<T>)this.table).Provider);
//                return ((IQueryable<T>)this.table).Provider;
//            }
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }

//        /// <summary>
//        /// 已重写 ToString()
//        /// </summary>
//        /// <returns>linq 表达式</returns>
//        public override string ToString()
//        {
//            return this.table.ToString();
//        }

//        /// <summary>
//        /// 目标Type
//        /// </summary>
//        static bool IsUnChanged { get; set; } = true;

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="lockType"></param>
//        /// <returns></returns>
//        /// <remarks>
//        /// 为了未来的拓展性
//        /// 锁类型、所力度、指定索引等
//        /// </remarks>
//        public IQueryable<T> With(SqlLockType lockType)
//        {
//            if (IsUnChanged)
//            {
//                IsUnChanged = false;
//                var type = typeof(T);
//                string typeName = $"dbo.Nolock{type.Name}View";
//                //var attribute = type.GetCustomAttribute<TableAttribute>();
//                var mapping = this.context.Mapping.GetTable(type);
//                var field = mapping.GetType().GetField("tableName", BindingFlags.Instance | BindingFlags.NonPublic);
//                field.SetValue(mapping, typeName);
//            }

//            return this.context.GetTable<T>();
//            //return new ReadTable<T>(this.context.GetTable<T>());
//        }

//        /// <summary>
//        /// 无锁查询
//        /// </summary>
//        /// <returns></returns>
//        public IQueryable<T> WithNolock()
//        {
//            return this.With(SqlLockType.Nolock);
//        }

//    }

//    class MyQueryProvider<T> : IQueryProvider where T : class
//    {
//        DataContext context;
//        IQueryable<T> iQuery;
//        IQueryProvider provider;

//        public MyQueryProvider(IQueryable<T> query)
//        {
//            this.provider = query.Provider;
//            this.iQuery = query;
//            this.context = query.GetType().
//                GetField(nameof(this.context), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(query) as DataContext;
//        }

//        public IQueryable CreateQuery(Expression expression)
//        {
//            return provider.CreateQuery(expression);
//        }

//        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
//        {
//            return provider.CreateQuery<TElement>(expression);
//        }

//        public object Execute(Expression expression)
//        {
//            //new McStor.QueryTranslator(expression);
//            var connection = this.context.Connection;

//            //TranslateResult result = this.Translate(expression);

//            var iQuery = this.CreateQuery<T>(expression);
//            DbCommand commond = this.context.GetCommand(iQuery);
//            if (connection.State != System.Data.ConnectionState.Open)
//            {
//                connection.Open();
//            }
//            DbDataReader reader = commond.ExecuteReader();
//            var items = context.Translate<T>(reader);

//            var query = provider.Execute(expression);

//            //System.Data.Linq.SqlClient.SqlProvider provider = new System.Data.Linq.SqlClient.SqlProvider();


//            return null;

//            //var command = context.GetCommand(query);

//            //var sql = command.CommandText;

//            //if (command.Connection.State != System.Data.ConnectionState.Open)
//            //{
//            //    command.Connection.Open();
//            //}
//            //var reader = command.ExecuteReader();
//            //var items = context.Translate<T>(reader);

//            //return items;
//        }

//        public TResult Execute<TResult>(Expression expression)
//        {
//            var query = provider.CreateQuery<T>(expression);
//            var command = context.GetCommand(query);

//            var sql = command.CommandText;

//            if (command.Connection.State != System.Data.ConnectionState.Open)
//            {
//                command.Connection.Open();
//            }
//            var reader = command.ExecuteReader();
//            var items = context.Translate<T>(reader);
//            return default(TResult);
//        }

//        public override string ToString()
//        {
//            return context.ToString();
//        }
//    }

//    static public class NolockTableExtent
//    {
//        static public NolockTable<T> NolockTable<T>(this IQueryable<T> iQuery) where T : class
//        {
//            return new Linq.NolockTable<T>(iQuery);
//        }

//    }

//}
