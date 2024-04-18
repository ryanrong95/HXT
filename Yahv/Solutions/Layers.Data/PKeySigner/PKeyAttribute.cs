using System;

namespace Layers.Data
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
        /// 效验Sql
        /// </summary>
        public string CheckSql { get; private set; }


        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="pType">模式</param>
        /// <param name="length">补充长度</param>
        public PKeyAttribute(string name
            , PKeySigner.Mode pType = PKeySigner.Mode.Normal
            , int length = 10
            , string checkSql = "")
        {
            this.Name = name;
            this.Type = pType;
            this.Length = length;
            this.CheckSql = checkSql;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="length">补充长度</param>
        /// <param name="checkSql">效验Sql,格式要求：SELECT Max([ID]) FROM [PvbErm].[dbo].[Roles]  where ID like 'xxx%'</param>
        public PKeyAttribute(string name, int length = 10, string checkSql = "") : this(name, PKeySigner.Mode.Normal, length)
        {
            this.Name = name;
            this.Length = length;
        }
    }
}
