using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class InvoiceNotice : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public bool IsPersonal { get; set; }

        //发票类型
        //   public Enums.InvoiceFromType FromType { get; set; }

        //发票类型
        public InvoiceType Type { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }
        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { set; get; }
        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { set; get; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }
        public string Carrier { get; set; }
        public string WayBillCode { get; set; }
        public Enums.InvoiceEnum Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string AdminID { get; set; }
        public string Summary { get; set; }
        /// <summary>
        /// 承运商name
        /// </summary>
        public string CarrierName { get; set; }
        #endregion
    }

}
