using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 颗粒化
    /// </summary>
    public class Particle : IUnique
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