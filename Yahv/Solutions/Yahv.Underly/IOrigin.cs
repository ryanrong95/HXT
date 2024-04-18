namespace Yahv.Underly
{
    /// <summary>
    /// 原产地
    /// </summary>
    public interface IOrigin
    {
        /// <summary>
        /// 原产地代码
        /// </summary>
        string Code { get; }
        
        /// <summary>
        /// 原产地中文名称
        /// </summary>
        string ChineseName { get; }

        /// <summary>
        /// 原产地英文名称
        /// </summary>
        string EnglishName { get; }
    }
}
