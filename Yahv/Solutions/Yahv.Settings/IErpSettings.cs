namespace Yahv.Settings
{
    /// <summary>
    /// 测试使用
    /// </summary>
    public interface IErpSettings : ISettings
    {
        /// <summary>
        /// 支付宝线上交易最大额度
        /// </summary>
        decimal Llaot { get; }

        /// <summary>
        /// 购物车最大数量
        /// </summary>
        int Tnsc { get; }
    }
}
