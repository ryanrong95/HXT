using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public partial interface IReponsitory
    {
        void CommandWithLog(string command, params object[] parameters);
        void DeleteWithLog<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void InsertWithLog<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void InsertWithLog<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void InsertWithLog(object entity);
        void UpdateWithLog<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
        void UpdateWithLog<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged;
    }
}
