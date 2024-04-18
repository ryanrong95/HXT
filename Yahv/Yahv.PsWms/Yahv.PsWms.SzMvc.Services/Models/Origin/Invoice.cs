using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 客户发票信息
    /// </summary>
    public class Invoice : IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string Name { get; set; }

        public string TaxNumber { get; set; }

        public string RegAddress { get; set; }

        public string Tel { get; set; }

        public string BankName { get; set; }

        public string BankAccount { get; set; }

        public Underly.InvoiceDeliveryType DeliveryType { get; set; }

        public string RevAddress { get; set; }

        public string Contact { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string DeliveryTypeDec
        {
            get
            {
                return this.DeliveryType.GetDescription();
            }
        }
    }
}
