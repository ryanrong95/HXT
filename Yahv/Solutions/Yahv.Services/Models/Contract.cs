using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class Contract
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 客户企业信息
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 内部公司ID
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 合同协议开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 合同协议结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal AgencyRate { get; set; }

        /// <summary>
        /// 最低代理费
        /// </summary>
        public decimal MinAgencyFee { get; set; }
        /// <summary>
        /// 换汇方式：预换汇，90天内换汇
        /// </summary>
        public ExchangeMode ExchangeMode { get; set; }

        /// <summary>
        /// 开票类型:服务费发票，全额发票
        /// </summary>
        public BillingType InvoiceType { get; set; }

        /// <summary>
        /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
      
        
        #endregion
    }
}
