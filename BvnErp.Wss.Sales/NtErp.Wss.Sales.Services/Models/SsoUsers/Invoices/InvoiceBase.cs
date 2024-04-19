using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model
{
    /// <summary>
    /// 开票信息
    /// </summary>
    /// http://www.wordstemplates.org/category/invoice-templates/
    [XmlInclude(typeof(InvoiceInfo))]
    [XmlInclude(typeof(China.Invoice))]
    public class InvoiceBase
    {
        public InvoiceBase()
        {
            this.Consignee = new Consignee();
        }

        protected Order father;

        /// <summary>
        /// 交货地（以此来判定 invoice 国际化）
        /// </summary>
        public District District
        {
            get
            {
                if (this.father == null)
                {
                    return District.CN;
                }
                return this.father.District;
            }
        }

        Consignee consignee;
        /// <summary>
        /// 提货人
        /// </summary>
        public Consignee Consignee
        {
            get
            {
                if (this.father != null && this.consignee == null)
                {
                    this.consignee = father.BillConsignee;
                }
                return this.consignee;
            }
            set
            {
                this.consignee = value;
            }
        }
    }
}
