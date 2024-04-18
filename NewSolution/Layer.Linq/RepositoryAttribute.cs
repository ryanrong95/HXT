using System;

namespace Layer.Linq
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
        /// 构造函数
        /// </summary>
        /// <param name="name">标题</param>
        public RepositoryAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
