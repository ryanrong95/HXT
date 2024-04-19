
using NtErp.Wss.Sales.Services.Models;
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
    /// Invoice （海外）overseas
    /// </summary>
    public class InvoiceInfo : InvoiceBase
    {
        public InvoiceInfo()
        {

        }

        [XmlIgnore]
        public string OrderID { get { return this.father.ID; } }

        [XmlIgnore]
        public string UserID { get { return this.father.UserID; } }

        /// <summary>
        /// 约定币种
        /// Currency Type
        /// </summary>
        [XmlIgnore]
        public Currency Currency { get { return this.father.Currency; } }

        /// <summary>
        /// 受益人银行
        /// </summary>
        [XmlIgnore]
        public Distributor Beneficiary
        {
            get
            {
                return this.father.Beneficiary;
            }
        }
    }
}
