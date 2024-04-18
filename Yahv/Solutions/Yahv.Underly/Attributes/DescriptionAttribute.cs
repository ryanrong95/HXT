using System;

namespace Yahv.Underly.Attributes
{
    /// <summary>
    /// 描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed public class DescriptionAttribute : Attribute
    {
        readonly string[] contexts;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="context">内容</param>
        public DescriptionAttribute(string context)
        {
            this.contexts = new[] { context };
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="arry">描述内容数组</param>
        public DescriptionAttribute(params string[] arry)
        {
            this.contexts = arry;
        }

        /// <summary>
        /// 描述内容
        /// </summary>
        public string Context
        {
            get { return contexts[0]; }
        }

        /// <summary>
        /// 描述内容数组
        /// </summary>
        public string[] Contexts
        {
            get { return contexts; }
        }
    }
}
