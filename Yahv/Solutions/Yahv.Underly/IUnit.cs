namespace Yahv.Underly
{
    /// <summary>
    /// 海关计量单位
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// 计量单位代码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 计量单位名称
        /// </summary>
        string Name { get; }
    }
}
