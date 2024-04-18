using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Layer.Linq
{
    /// <summary>
    /// 基本支持类
    /// </summary>
    abstract public partial class LinqReponsitory : IReponsitory, IDisposable
    {
        //事件扩展
        //...待续

        DataContext dataContext;
        DataContext readonlier;
        bool isAutoSumit;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LinqReponsitory()
        {
            this.isAutoSumit = true;
            this.dataContext = this.InitDataContext();
            if (!this.dataContext.DatabaseExists())
            {
                try
                {
                    this.dataContext.CreateDatabase();
                }
                catch (Exception ex)
                {
                    //如果 报告 tcp 0 就是连接问题
                    //查看  connection string  是否正确
                    //否则就真的是网络问题
                    throw ex;
                }
            }

            LinqReleaser.Current.Enqueue(this);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isAutoSumit">是否自动提交</param>
        public LinqReponsitory(bool isAutoSumit) : this()
        {
            this.isAutoSumit = isAutoSumit;
        }

        internal DataContext DataContext
        {
            get { return dataContext; }
        }

        /// <summary>
        /// 初始化 表示 LINQ to SQL 框架的主入口点
        /// </summary>
        /// <returns>数据操作上下文</returns>
        abstract protected DataContext InitDataContext();

        public void Insert<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.dataContext.GetTable<T>().InsertOnSubmit(entity);
            this.AutoSubmit();
        }

        public void Insert<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            this.dataContext.GetTable<T>().InsertAllOnSubmit(entities);
            this.AutoSubmit();
        }

        public void Insert(object entity)
        {
            this.dataContext.GetTable(entity.GetType()).InsertOnSubmit(entity);
            this.AutoSubmit();
        }

        public void Update<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            
            try {
                IQueryable<T> iQueryable = this.dataContext.GetTable<T>().Where(lambda);

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
            catch (System.Data.Linq.ChangeConflictException ex)
            {
                foreach (System.Data.Linq.ObjectChangeConflict occ in this.dataContext.ChangeConflicts)
                {
                    //以下是解决冲突的三种方法，选一种即可
                    // 使用当前数据库中的值，覆盖Linq缓存中实体对象的值
                    occ.Resolve(System.Data.Linq.RefreshMode.OverwriteCurrentValues);
                    // 使用Linq缓存中实体对象的值，覆盖当前数据库中的值
                    occ.Resolve(System.Data.Linq.RefreshMode.KeepCurrentValues);
                    // 只更新实体对象中改变的字段的值，其他的保留不变
                    occ.Resolve(System.Data.Linq.RefreshMode.KeepChanges);
                }
                // 这个地方要注意，Catch方法中，我们前面只是指明了怎样来解决冲突，这个地方还需要再次提交更新，这样的话，值    //才会提交到数据库。
                this.dataContext.SubmitChanges();
            }

        }
        public void Update<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            IQueryable<T> iQueryable = this.dataContext.GetTable<T>().Where(lambda);

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
        public void Update<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, string>> orderbyLambda, params object[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var tArray = this.dataContext.GetTable<T>().Where(whereLambda).OrderBy(orderbyLambda).ToArray();

            Type etype = entities[0].GetType();
            var eproperties = etype.GetProperties();

            Type atype = typeof(T);

            for (int i = 0; i < tArray.Count(); i++)
            {
                var associate = tArray[i];
                for (int index = 0; index < eproperties.Length; index++)
                {
                    object evalue = eproperties[index].GetValue(entities[i]);
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

        public void Delete<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var iQueryable = this.dataContext.GetTable<T>().Where(lambda);
            this.dataContext.GetTable<T>().DeleteAllOnSubmit(iQueryable);
            this.AutoSubmit();
        }
        public IQueryable<T> GetTable<T>() where T : class
        {
            return this.dataContext.GetTable<T>();
        }
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
        public IQueryable<T> ReadTable<T>() where T : class
        {
            if (this.readonlier == null)
            {
                lock (this)
                {
                    if (this.readonlier == null)
                    {
                        this.readonlier = this.InitDataContext();
                        this.readonlier.CommandTimeout = 3600;
                        this.readonlier.ObjectTrackingEnabled = false;
                    }
                }
            }
            return this.readonlier.GetTable<T>();
        }

        void AutoSubmit()
        {
            if (this.isAutoSumit)
            {
                this.dataContext.SubmitChanges();
            }
        }
        virtual public void Submit()
        {
            this.dataContext.SubmitChanges();
        }

        public void Command(string command, params object[] parameters)
        {
            this.dataContext.ExecuteCommand(command, parameters);
        }

        public IEnumerable<T> Query<T>(string query, params object[] parameters)
        {
            return this.dataContext.ExecuteQuery<T>(query, parameters);
        }
        public System.Collections.IEnumerable Query(Type type, string query, params object[] parameters)
        {
            return this.dataContext.ExecuteQuery(type, query, parameters);
        }

        public void Dispose()
        {
            if (this.dataContext != null)
            {
                this.dataContext.Dispose();
            }

            if (this.readonlier != null)
            {
                this.readonlier.Dispose();
            }
        }
    }
}
