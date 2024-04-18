using Needs.Linq;

namespace Needs.Wl.Client.Services.PageModels
{
    /// <summary>
    /// WebApi的归类视图
    /// </summary>
    public class ApiClassifyProduct : IUnique
    {
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsForbid { get; set; }

        /// <summary>
        /// 第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 第二单位
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 监管条件
        /// </summary>
        public string RegulatoryCode { get; set; }

        /// <summary>
        /// 检验检疫
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 进口最惠圆税率
        /// </summary>
        public decimal MFN { get; set; }

        /// <summary>
        /// 普通税率
        /// </summary>
        public decimal General { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal AddedValue { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? Consume { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }
    }
}
