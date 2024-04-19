using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    /// <summary>
    /// 附加费
    /// </summary>
    public class Premium : PremiumBase<PremiumProduct>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlIgnore]
        public string Name
        {
            get
            {
                return this.Product[nameof(this.Name)];
            }
            set
            {
                this.Product[nameof(this.Name)] = value;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Product Index 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[string index]
        {
            get { return this.Product[index]; }
            set { this.Product[index] = value; }
        }

        public Premium()
        {
            this.Product = new PremiumProduct();
            this.CreateDate = DateTime.Now;
        }
    }
}
