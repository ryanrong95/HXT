using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Layers.Linq
{
    /// <summary>
    /// Linq 基本支持类
    /// </summary>
    abstract public class LinqReponsitory : IReponsitory, IDisposable
    {
        /// <summary>
        /// 类型 Thread.Name的用法
        /// </summary>
        protected string Name { get; set; }

        //事件扩展
        //...待续

        /// <summary>
        /// 数据连接者
        /// </summary>
        internal protected DataContext DataContext { get; private set; }
        /// <summary>
        /// 只读连接者
        /// </summary>
        protected DataContext ReadContext { get; private set; }

        bool isAutoSumit;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public LinqReponsitory()
        {
            this.isAutoSumit = true;
            this.DataContext = this.InitDataContext();

            #region _bak

            //if (!this.DataContext.DatabaseExists())
            //{
            //    try
            //    {
            //        this.DataContext.CreateDatabase();
            //    }
            //    catch (Exception ex)
            //    {
            //        //如果 报告 tcp 0 就是连接问题
            //        //查看  connection string  是否正确
            //        //否则就真的是网络问题
            //        throw ex;
            //    }
            //}

            #endregion

            LinqReleaser.Current.Enqueue(this);
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="isAutoSumit">是否自动提交</param>
        public LinqReponsitory(bool isAutoSumit) : this()
        {
            this.isAutoSumit = isAutoSumit;
        }

        public LinqReponsitory(bool isAutoSumit, bool isFactory)
        {
            this.isAutoSumit = isAutoSumit;
            this.IsFactory = true;
            this.DataContext = this.InitDataContext();

            if (isFactory)
            {
                return;
            }

            LinqReleaser.Current.Enqueue(this);
        }

        /// <summary>
        /// 是否为工厂建立
        /// </summary>
        /// <remarks>陈翰临时开发，后期需要ctor化</remarks>
        internal bool IsFactory { get; private set; }

        /// <summary>
        /// 初始化 表示 LINQ to SQL 框架的主入口点
        /// </summary>
        /// <returns>数据操作上下文</returns>
        abstract internal protected DataContext InitDataContext();

        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">目标对象</param>
        public void Insert<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.DataContext.GetTable<T>().InsertOnSubmit(entity);
            this.AutoSubmit();
        }

        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        public void Insert<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.DataContext.GetTable<T>().InsertAllOnSubmit(entities);
            this.AutoSubmit();
        }

        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        public void Insert<T>(IEnumerable<T> entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.DataContext.GetTable<T>().InsertAllOnSubmit(entities);
            this.AutoSubmit();
        }

        /// <summary>
        /// 插入对象表数据
        /// </summary>
        /// <param name="entity">对象实例</param>
        public void Insert<T>(object entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.DataContext.GetTable(typeof(T)).InsertOnSubmit(entity);
            this.AutoSubmit();
        }

        /// <summary>
        /// 更新目标对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">目标对象</param>
        /// <param name="lambda">lambda</param>
        public void Update<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            IQueryable<T> iQueryable = this.DataContext.GetTable<T>().Where(lambda);

            Type etype = entity.GetType();
            var eproperties = etype.GetProperties();

            Type atype = typeof(T);
            var aproperties = atype.GetProperties();

            foreach (T associate in iQueryable)
            {
                for (int index = 0; index < eproperties.Length; index++)
                {
                    object evalue = eproperties[index].GetValue(entity);
                    var aproperty = atype.GetProperty(eproperties[index].Name);
                    if (aproperty == null)
                    {
                        throw new NotSupportedException($"Do not support the {eproperties[index].Name} call");
                    }
                    aproperty.SetValue(associate, evalue);
                }
            }

            this.AutoSubmit();
        }

        /// <summary>
        /// 更新目标对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">>目标对象</param>
        /// <param name="lambda">lambda</param>
        public void Update<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            IQueryable<T> iQueryable = this.DataContext.GetTable<T>().Where(lambda);

            Type etype = entity.GetType();
            var changedProperty = etype.GetField("changeds", BindingFlags.NonPublic | BindingFlags.Instance);
            if (changedProperty == null)
            {
                var eproperties = etype.GetProperties();
                foreach (T associate in iQueryable)
                {
                    for (int index = 0; index < eproperties.Length; index++)
                    {
                        object evalue = eproperties[index].GetValue(entity);
                        eproperties[index].SetValue(associate, evalue);
                    }
                }
            }
            else
            {
                var changeds = changedProperty.GetValue(entity) as List<string>;
                foreach (T associate in iQueryable)
                {
                    for (int index = 0; index < changeds.Count; index++)
                    {
                        var property = etype.GetProperty(changeds[index]);
                        object evalue = property.GetValue(entity);
                        property.SetValue(associate, evalue);
                    }
                }
            }
            this.AutoSubmit();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="lambda">lambda</param>
        public void Delete<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var iQueryable = this.DataContext.GetTable<T>().Where(lambda);
            this.DataContext.GetTable<T>().DeleteAllOnSubmit(iQueryable);
            this.AutoSubmit();
        }
        /// <summary>
        /// 获取目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <returns>目标对象表数据</returns>
        public IQueryable<T> GetTable<T>() where T : class
        {
            return this.DataContext.GetTable<T>();
        }
        /// <summary>
        /// 获取目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="isReadonly">是否只读</param>
        /// <returns>目标对象表数据</returns>
        public IQueryable<T> GetTable<T>(bool isReadonly) where T : class
        {
            if (isReadonly)
            {
                return this.ReadTable<T>();
            }
            else
            {
                return this.GetTable<T>();
            }
        }

        /// <summary>
        /// 获取目标对象只读表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <returns>目标对象只读表数据</returns>
        public IQueryable<T> ReadTable<T>() where T : class
        {
            if (this.ReadContext == null)
            {
                lock (this)
                {
                    if (this.ReadContext == null)
                    {
                        this.ReadContext = this.InitDataContext();
                        this.ReadContext.ObjectTrackingEnabled = false;
                    }
                }
            }
            return this.ReadContext.GetTable<T>();
        }


        void AutoSubmit()
        {
            if (this.isAutoSumit)
            {
                this.DataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        virtual public void Submit()
        {
            this.DataContext.SubmitChanges();
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>执行命令的修改的行数</returns>
        public int Command(string command, params object[] parameters)
        {
            return this.DataContext.ExecuteCommand(command, parameters);
        }
        /// <summary>
        /// 获取Sql数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="query">Sql query</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string query, params object[] parameters)
        {
            return this.DataContext.ExecuteQuery<T>(query, parameters);
        }

        /// <summary>
        /// 实现 释放函数
        /// </summary>
        virtual public void Dispose()
        {
            if (IsFactory)
            {
                if (!LinqFactory.TryDispose(this))
                {
                    return;
                }
            }


            if (this.DataContext != null)
            {
                this.DataContext.Dispose();
            }

            if (this.ReadContext != null)
            {
                this.ReadContext.Dispose();
            }
        }

        /// <summary>
        /// 将数据批量插入到数据库
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据源</param>
        //[Obsolete("废弃，建议使用临时表：")]
        public void SqlBulkCopyByDatatable(string tableName, DataTable dt)
        {
            //this.DataContext .Transaction.
            //this.DataContext.Transaction

            using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(DataContext.Connection.ConnectionString, SqlBulkCopyOptions.UseInternalTransaction))
            {
                sqlbulkcopy.DestinationTableName = tableName;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }
                sqlbulkcopy.WriteToServer(dt);
                //sqlbulkcopy.WriteToServerAsync()
            }
        }

        /// <summary>
        /// 对象类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="o">任意类型对象</param>
        /// <returns>目标类型对象</returns>
        public T ToRobject<T>(object o) where T : class, INotifyPropertyChanging, INotifyPropertyChanged, new()
        {
            if (o == null)
            {
                return default(T);
            }

            var opropertis = o.GetType().GetProperties();
            T t = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                var oproperty = opropertis.SingleOrDefault(item => item.Name == property.Name);
                if (oproperty == null)
                {
                    continue;
                }
                property.SetValue(property, oproperty.GetValue(o));
            }

       
            return t;
        }

        #region 陈翰增加的事物操作

        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns>连接对象</returns>
        public SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(this.DataContext.Connection.ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns>连接对象</returns>
        public DbConnection Connection
        {
            get
            {
                var connection = this.DataContext.Connection;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection;
            }
        }

        TSqlReponsitory tsql;

        /// <summary>
        /// TSql 支持者
        /// </summary>
        public TSqlReponsitory TSql
        {
            get
            {
                if (tsql == null)
                {
                    tsql = new TSqlReponsitory(this);
                }
                return tsql;
            }
        }


        /// <summary>
        /// 开启事物
        /// </summary>
        /// <param name="level">指定连接的事务锁定行为</param>
        /// <remarks>
        /// 总体上说：比scope方式要危险！使用者请慎重！
        /// </remarks>
        /// <returns>
        /// 数据库通用事物
        /// </returns>
        public DbTransaction OpenTransaction(IsolationLevel level = IsolationLevel.Serializable)
        {
            var context = this.DataContext;

            if (context.Transaction != null)
            {
                throw new Exception("不能重复建立事物");
            }

            var connection = context.Connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            return context.Transaction = context.Connection.BeginTransaction(level);
        }

        /// <summary>
        /// 开启事物
        /// </summary>
        /// <param name="transactions">调用者</param>
        /// <returns>数据库封装事物</returns>
        public LinqTransactions OpenTransaction(params LinqReponsitory[] reponsitories)
        {
            return new LinqTransactions(this.OpenTransaction(), reponsitories.Select(item => item.OpenTransaction()));
        }

        /// <summary>
        /// 开启事物
        /// </summary>
        /// <param name="transactions">调用者</param>
        /// <param name="level">指定连接的事务锁定行为</param>
        /// <returns>数据库封装事物</returns>
        public LinqTransactions OpenTransaction(IsolationLevel level = IsolationLevel.Serializable, params LinqReponsitory[] transactions)
        {
            return new LinqTransactions(this.OpenTransaction(level), transactions.Select(item => item.OpenTransaction(level)));
        }

        /// <summary>
        /// 由转换返回的对象的集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="reader">只读器</param>
        /// <returns>目标类型的集合</returns>
        public IEnumerable<T> Translate<T>(DbDataReader reader)
        {
            return this.DataContext.Translate<T>(reader);
        }

        #endregion
    }
}
