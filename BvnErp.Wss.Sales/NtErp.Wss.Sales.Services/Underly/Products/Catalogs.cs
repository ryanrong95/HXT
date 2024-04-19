using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products
{
    /// <summary>
    /// 分目录
    /// </summary>
    public class Catalogs
    {
        /// <summary>
        /// 暂时只考虑一个产品一个分类
        /// </summary>
        [System.Xml.Serialization.XmlElement("Product")]
        public Categories Product { get; set; }

        /// <summary>
        /// 海关税则
        /// </summary>
        public HSCodes HSCode { get; set; }

        /// <summary>
        /// 税务税则
        /// </summary>
        public TaxCodes TaxCode { get; set; }



        /// <summary>
        /// 构造函数
        /// </summary>
        public Catalogs()
        {
            this.HSCode = new HSCodes();
            this.TaxCode = new TaxCodes();
            this.Product = new Categories();
        }
    }
}
