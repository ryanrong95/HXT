using System;

namespace Needs.Overall
{
    /// <summary>
    /// 主键属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class PKeyAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public PKeySigner.Mode Type { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">标题</param>
        public PKeyAttribute(string name
            , PKeySigner.Mode pType = PKeySigner.Mode.Normal
            , int length = 10)
        {
            this.Name = name;
            this.Type = pType;
            this.Length = length;
        }
    }
}
