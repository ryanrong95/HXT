namespace Yahv.Underly
{
    /// <summary>
    /// 海关包装类型
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// 包装种类代码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 包装种类名称
        /// </summary>
        string Name { get; }
    }
}
