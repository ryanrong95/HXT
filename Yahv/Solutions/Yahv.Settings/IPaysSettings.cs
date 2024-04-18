namespace Yahv.Settings
{
    /// <summary>
    /// 财务配置
    /// </summary>
    public interface IPaysSettings : ISettings
    {
        /// <summary>
        /// 封账日
        /// </summary>
        int ClosedDay { get; set; }
    }
}