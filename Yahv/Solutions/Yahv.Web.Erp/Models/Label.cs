using Yahv.Utils.Converters.Contents;

namespace Yahv.Web.Erp.Models
{
    /// <summary>
    /// 标签
    /// </summary>
    class Label
    {
        public string ID { get { return new string[] { this.Name, this.jField, this.Type }.MD5(); } }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string jField { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
    }
}
