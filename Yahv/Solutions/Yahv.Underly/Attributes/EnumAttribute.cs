using System;

namespace Yahv.Underly.Attributes
{
    /// <summary>
    /// 枚举特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = true)]
    public class EnumNamingAttribute : Attribute
    {
        /// <summary>
        /// 重命名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name">重命名</param>
        public EnumNamingAttribute(string name)
        {
            this.Name = name;
        }
    }
}
