using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceResult:IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 发票类型名称（江苏增值税（卷式普通发票））
        /// </summary>
        public string invoiceTypeName { get; set; }
        /// <summary>
        /// 发票类型代码
        /// </summary>
        public string invoiceTypeCode { get; set; }
        /// <summary>
        /// 查询日期
        /// </summary>
        public DateTime? checkDate { get; set; }
        /// <summary>
        /// 查验次数
        /// </summary>
        public int? checkNum { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        public string invoiceDataCode { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        public string invoiceNumber { get; set; }      
        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? billingTime { get; set; }
        /// <summary>
        /// (购买方)名称
        /// </summary>
        public string purchaserName { get; set; }
        /// <summary>
        /// (购买方)纳税人识别号
        /// </summary>
        public string taxpayerNumber { get; set; }
        /// <summary>
        /// 机器编号
        /// </summary>
        public string taxDiskCode { get; set; }
        /// <summary>
        /// (购买方)地址.电话
        /// </summary>
        public string taxpayerAddressOrId { get; set; }
        /// <summary>
        /// (购买方)开户行及账号
        /// </summary>
        public string taxpayerBankAccount { get; set; }
        /// <summary>
        /// (销售方)名称
        /// </summary>
        public string salesName { get; set; }
        /// <summary>
        /// (销售方)纳税人识别号
        /// </summary>
        public string salesTaxpayerNum { get; set; }
        /// <summary>
        /// (销售方)地址.电话
        /// </summary>
        public string salesTaxpayerAddress { get; set; }
        /// <summary>
        /// (销售方)开户行及账号
        /// </summary>
        public string salesTaxpayerBankAccount { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? totalAmount { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal? totalTaxNum { get; set; }
        /// <summary>
        /// 价税合计
        /// </summary>
        public decimal? totalTaxSum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string invoiceRemarks { get; set; }
        /// <summary>
        /// 收货员（卷式发票新增字段，其他票可以不用）
        /// </summary>
        public string goodsClerk { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        public string checkCode { get; set; }
        /// <summary>
        /// 是否作废 0：正常，1：作废
        /// </summary>
        public string voidMark { get; set; }
        /// <summary>
        /// 是否为清单票  Y：是，N：否//可以根据该字段展示清单票和正常票
        /// </summary>
        public string isBillMark { get; set; }
        /// <summary>
        /// 收费标志字段（06：可抵扣通行费 07：不可抵扣通行费，08：成品油）
        /// </summary>
        public string tollSign { get; set; }
        /// <summary>
        /// 收费标志名称
        /// </summary>
        public string tollSignName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        

        InvoiceDetailDatas invoiceDetailData;
        public InvoiceDetailDatas InvoiceDetailData
        {
            get
            {
                if (invoiceDetailData == null)
                {
                    using (var view = new Views.InvoiceResultItemsView())
                    {
                        var query = view.Where(item => item.InvoiceResultID == this.ID);
                        this.InvoiceDetailData = new InvoiceDetailDatas(query);
                    }
                }
                return this.invoiceDetailData;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.invoiceDetailData = new InvoiceDetailDatas(value, new Action<InvoiceDetailData>(delegate (InvoiceDetailData item)
                {
                    item.InvoiceResultID = this.ID;
                }));
            }
        }
    }
}
