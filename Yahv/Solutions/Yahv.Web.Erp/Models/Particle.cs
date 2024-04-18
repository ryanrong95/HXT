using Yahv.Linq;

namespace Yahv.Web.Erp.Models
{
    /// <summary>
    /// 颗粒化
    /// </summary>
    class Particle : IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }  
        /// <summary>
        /// 地址编码
        /// </summary>
        public string UrlCode { get; set; }
        /// <summary>
        /// 真实地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        public string Type { get; set; }
    }
}