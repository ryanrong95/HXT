namespace Yahv.Underly
{
    /// <summary>
    /// 地域信息接口
    /// </summary>
    public interface IDistrict
    {
        /// <summary>
        /// 短名称
        /// </summary>
        string ShortName { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 域名
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        string ShowName { get; }

        /// <summary>
        /// 中文名称
        /// </summary>
        string ChineseName { get; }
    }
}
