using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    abstract public partial class LinqReponsitory
    {
        /// <summary>
        /// 不用实现
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public void CommandWithLog(string command, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lambda"></param>
        public void DeleteWithLog<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            ProxyGenerator generator = new ProxyGenerator();
            IReponsitory iReponsitory = generator.CreateInterfaceProxyWithTarget<IReponsitory>(this, new DeleteInterceptor<T>(lambda, this));
            iReponsitory.Delete<T>(lambda);
        }

        /// <summary>
        /// 单个插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void InsertWithLog<T>(T entity) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            ProxyGenerator generator = new ProxyGenerator();
            IReponsitory iReponsitory = generator.CreateInterfaceProxyWithTarget<IReponsitory>(this, new EntityInsertInterceptor<T>(entity, this));
            iReponsitory.Insert<T>(entity);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void InsertWithLog<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            ProxyGenerator generator = new ProxyGenerator();
            IReponsitory iReponsitory = generator.CreateInterfaceProxyWithTarget<IReponsitory>(this, new EntitiesInsertInterceptor<T>(entities, this));
            iReponsitory.Insert<T>(entities);
        }

        /// <summary>
        /// 不用实现
        /// </summary>
        /// <param name="entity"></param>
        public void InsertWithLog(object entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// T 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="lambda"></param>
        public void UpdateWithLog<T>(T entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            ProxyGenerator generator = new ProxyGenerator();
            IReponsitory iReponsitory = generator.CreateInterfaceProxyWithTarget<IReponsitory>(this, new TUpdateInterceptor<T>(entity, lambda, this));
            iReponsitory.Update<T>(entity, lambda);
        }

        /// <summary>
        /// object 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="lambda"></param>
        public void UpdateWithLog<T>(object entity, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            ProxyGenerator generator = new ProxyGenerator();
            IReponsitory iReponsitory = generator.CreateInterfaceProxyWithTarget<IReponsitory>(this, new ObjectUpdateInterceptor<T>(entity, lambda, this));
            iReponsitory.Update<T>(entity, lambda);
        }

        public void UpdateWithLog<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, string>> orderbyLambda, params object[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            //ProxyGenerator generator = new ProxyGenerator();
            //LinqReponsitory iReponsitory = generator.CreateClassProxyWithTarget<LinqReponsitory>(this, new ObjectsUpdateInterceptor<T>(entities, whereLambda, orderbyLambda, this));

            T[] updatings = this.ReadTable<T>().Where(whereLambda).OrderBy(orderbyLambda).ToArray();

            this.Update<T>(whereLambda, orderbyLambda, entities);

            T[] updateds = this.ReadTable<T>().Where(whereLambda).OrderBy(orderbyLambda).ToArray();

            ObjectsUpdateInterceptor<T> objectsUpdateInterceptor = new ObjectsUpdateInterceptor<T>(updatings, updateds, this);
            objectsUpdateInterceptor.Intercept();
        }

    }
}
