using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Layers.Linq
{
    /// <summary>
    /// 支持者接口
    /// </summary>
    public interface IReponsitory : IDisposable
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="lambda">lambda</param>
        void Delete<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;

        /// <summary>
        /// 获取目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <returns>目标对象表数据</returns>
        IQueryable<T> GetTable<T>() where T : class;
        /// <summary>
        /// 获取目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="isReadonly">是否只读</param>
        /// <returns>目标对象表数据</returns>
        IQueryable<T> GetTable<T>(bool isReadonly) where T : class;
        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">目标对象</param>
        void Insert<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        void Insert<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;

        /// <summary>
        /// 插入对象表数据
        /// </summary>
        /// <param name="entity">对象实例</param>
        void Insert<T>(object entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;

        /// <summary>
        /// 获取Sql数据
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="query">Sql query</param>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string query, params object[] parameters);

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">sql</param>
        /// <param name="parameters">参数</param>
        int Command(string command, params object[] parameters);

        /// <summary>
        /// 获取目标对象只读表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <returns>目标对象只读表数据</returns>
        IQueryable<T> ReadTable<T>() where T : class;

        /// <summary>
        /// 提交
        /// </summary>
        void Submit();

        /// <summary>
        /// 更新目标对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">>目标对象</param>
        /// <param name="lambda">lambda</param>
        void Update<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;

        /// <summary>
        /// 更新目标对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entity">目标对象</param>
        /// <param name="lambda">lambda</param>
        void Update<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
    }
}