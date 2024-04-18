namespace Needs.Overall.Models
{
    /// <summary>
    /// 语言
    /// </summary>
    public class Language : ILanguage
    {
        /// <summary>
        /// 短名称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 英文名称（国际名称）
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataName { get; set; }
    }
}
