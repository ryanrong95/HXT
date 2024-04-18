using System;
using System.Linq;
using System.Reflection;
using Yahv.Linq;
using Yahv.Services.Models;
namespace Yahv.Services
{
    /// <summary>
    /// 视图工厂泛型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class ViewFactoryAttribute : Attribute
    {
        readonly Type type;
        public ViewFactoryAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get { return this.type; }
        }
    }


    /// <summary>
    /// 工厂开发工厂
    /// </summary>
    /// <remarks>未来一定会挪动位置，暂时不要滥用</remarks>
    abstract public class ViewFactory
    {
        /// <summary>
        /// 工厂
        /// </summary>
        /// <param name="iReponsitory">支持者</param>
        /// <returns>指定的唯一性可查询集</returns>
        /// <remarks>适合在逻辑层中使用</remarks>
        static public UniqueBase<T> Create<T>(Layers.Linq.IReponsitory iReponsitory) where T : IUnique
        {
            var attribute = typeof(Client).GetCustomAttribute<ViewFactoryAttribute>();
            if (!attribute.Type.IsGenericType)
            {
                throw new NotImplementedException("必须指定泛型类型");
            }

            if (!attribute.Type.IsSubclassOf(typeof(UniqueBase<T>)))
            {
                throw new NotImplementedException("调用类的继承方式有严重错误！");
            }

            var reponsitory = iReponsitory.GetType();
            Type genericType = attribute.Type;
            Type implementType = genericType.MakeGenericType(reponsitory);

            return Activator.CreateInstance(implementType, iReponsitory) as UniqueBase<T>;
        }

        /// <summary>
        /// 工厂
        /// </summary>
        /// <param name="iReponsitory">支持者</param>
        /// <returns>指定的唯一性可查询集</returns>
        /// <param name="arry">其他参数</param>
        /// <remarks>适合在逻辑层中使用</remarks>
        static public UniqueBase<T> Create<T>(Layers.Linq.IReponsitory iReponsitory, Underly.Business business) where T : IUnique
        {
            var attribute = typeof(Client).GetCustomAttribute<ViewFactoryAttribute>();
            if (!attribute.Type.IsGenericType)
            {
                throw new NotImplementedException("必须指定泛型类型");
            }

            if (!attribute.Type.IsSubclassOf(typeof(UniqueBase<T>)))
            {
                throw new NotImplementedException("调用类的继承方式有严重错误！");
            }

            var reponsitory = iReponsitory.GetType();
            Type genericType = attribute.Type;
            Type implementType = genericType.MakeGenericType(reponsitory);

            return Activator.CreateInstance(implementType, iReponsitory, business) as UniqueBase<T>;
        }
    }

    /// <summary>
    /// 工厂开发工厂
    /// </summary>
    abstract public class ViewFactory<T> where T : IUnique
    {
        /// <summary>
        /// 工厂
        /// </summary>
        /// <param name="iReponsitory"></param>
        /// <returns></returns>
        static public UniqueBase<T> Create()
        {
            var assembly = Assembly.GetCallingAssembly();
            var type = assembly.GetTypes().Where(item => item.Name == "ReponsitoryHelper").FirstOrDefault();

            if (type == null)
            {
                throw new NotImplementedException("没有实现 类：ReponsitoryHelper");
            }

            var defaultName = type.GetField("DefaultName");

            if (defaultName == null)
            {
                throw new NotImplementedException("没有实现 字段 ReponsitoryHelper.DefaultName");
            }

            var attribute = typeof(T).GetCustomAttribute<ViewFactoryAttribute>();
            if (!attribute.Type.IsGenericType)
            {
                throw new NotImplementedException("必须指定泛型类型");
            }

            if (!attribute.Type.IsSubclassOf(typeof(UniqueBase<T>)))
            {
                throw new NotImplementedException("调用类的继承方式有严重错误！");
            }

            string name = $"Layers.Data.Sqls.{defaultName.GetValue(null)}Reponsitory,Layers.Data";
            var reponsitory = Type.GetType(name);
            Type genericType = attribute.Type;
            Type implementType = genericType.MakeGenericType(reponsitory);

            return Activator.CreateInstance(implementType) as UniqueBase<T>;
        }
    }
}
