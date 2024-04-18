using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类代码配置
    /// </summary>
    public class ProductClassifyConfig : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 归类类型：产品归类、产品预归类
        /// </summary>
        public Enums.ClassifyType Type { get; set; }

        /// <summary>
        /// 归类阶段：预处理一、预处理二、已完成、产品变更未处理、产品变更已处理
        /// </summary>
        public Enums.ClassifyStep Step { get; set; }

        /// <summary>
        /// 预归类公司类型：内单、Icgoo、外单、快包
        /// </summary>
        public Enums.CompanyTypeEnums? CompanyType { get; set; }

        /// <summary>
        /// 用于完成产品归类的类的名称
        /// </summary>
        public string ClassName { get; set; }

        public ProductClassifyConfig()
        {

        }

        public ProductClassifyConfig(Enums.ClassifyType type)
        {
            this.Type = type;
        }
    }

    /// <summary>
    /// 产品归类配置
    /// </summary>
    public sealed class OutsideProductConfig : ProductClassifyConfig
    {
        public OutsideProductConfig() : base(Enums.ClassifyType.ProductClassify)
        {
        }
    }

    /// <summary>
    /// 产品预归类配置
    /// </summary>
    public sealed class AdvanceProductConfig : ProductClassifyConfig
    {
        public AdvanceProductConfig() : base(Enums.ClassifyType.ProductPreClassify)
        {
        }
    }
}
