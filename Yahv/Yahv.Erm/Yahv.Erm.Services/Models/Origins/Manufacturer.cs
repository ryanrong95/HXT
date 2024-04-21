using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 优势品牌
    /// </summary>
    public class Manufacturer
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        [Description("品牌名称")]
        public string Name { set; get; }
        /// <summary>
        /// 是否代理
        /// </summary>
        [Description("是否代理")]
        public bool Agent { set; get; }
    }
    /// <summary>
    /// 优势型号
    /// </summary>
    public class PartNumber
    {
        /// <summary>
        /// 型号
        /// </summary>
        [Description("型号")]
        public string Name { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        [Description("品牌")]
        public string Manufacturer { set; get; }
    }
}
