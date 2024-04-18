namespace Yahv.Underly.Rings
{
    /// <summary>
    /// Erp 角色接口
    /// </summary>
    public interface IRingRole
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
    }
}
