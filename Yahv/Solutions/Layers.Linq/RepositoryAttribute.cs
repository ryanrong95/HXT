using System;

namespace Layers.Linq
{
    /// <summary>
    /// 调用支持属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public sealed class RepositoryAttribute : Attribute
    {
        /// <summary>
        /// Repository 类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="type">类型</param>
        public RepositoryAttribute(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// 创建指定类型的 IReponsitory
        /// </summary>
        /// <returns>返回：IReponsitory</returns>
        public IReponsitory Create()
        {
            return Activator.CreateInstance(this.Type) as IReponsitory;
        }
    }
}
