namespace Needs.Overall.Models
{
    /// <summary>
    /// 语言
    /// </summary>
    public interface ILanguage
    {
        /// <summary>
        /// 短名称
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 英文名称（国际名称）
        /// </summary>
        string EnglishName { get; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        string DataName { get; }
    }
}
