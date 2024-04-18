namespace Yahv.Underly
{
    /// <summary>
    /// 货币信息接口
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        string ChineseName { get; }

        /// <summary>
        /// 短名称
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// 符号
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// 短符号
        /// </summary>
        string ShortSymbol { get; }
    }
}
