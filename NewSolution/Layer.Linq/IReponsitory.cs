using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Layer.Linq
{
    public partial interface IReponsitory : IDisposable
    {
        void Command(string command, params object[] parameters);
        void Delete<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        IQueryable<T> GetTable<T>() where T : class;
        IQueryable<T> GetTable<T>(bool isReadonly) where T : class;
        void Insert<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void Insert<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void Insert(object entity);
        IEnumerable<T> Query<T>(string query, params object[] parameters);
        IQueryable<T> ReadTable<T>() where T : class;
        void Submit();
        void Update<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void Update<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
    }
}