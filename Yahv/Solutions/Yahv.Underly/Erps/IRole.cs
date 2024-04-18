using System.Collections.Generic;

namespace Yahv.Underly.Erps
{
    /// <summary>
    /// Erp 角色接口
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 角色名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 系统超级管理员角色
        /// </summary>
        bool IsSuper { get; }

        /// <summary>
        /// 角色类型
        /// </summary>
        RoleType Type { get; }

      
    }
}
