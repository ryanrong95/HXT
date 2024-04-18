using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.DyjModels
{
    public class FeeApplyModel
    {
        public string key { get; set; }

        public FeeApplyDataModel data { get; set; }

        //public FeeApplyListModel[] list { get; set; }
    }

    public class FeeApplyDataModel
    {
        /// <summary>
        /// 币种 1为人民币，2位美元
        /// </summary>
        public int CurrencyID { get; set; }
        /// <summary>
        /// 指定审核人
        /// </summary>
        public string CheckID { get; set; }
        /// <summary>
        /// 指定审核人名称
        /// </summary>
        public string CheckName { get; set; }
        /// <summary>
        /// 金额（人民币）
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 金额外币（外币）
        /// </summary>
        public decimal FAmount { get; set; }
        /// <summary>
        /// 付款公司
        /// </summary>
        public string PayCompany { get; set; }
        /// <summary>
        /// 备注信息 
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 收款单位ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 收款单位名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 收款单位联系人
        /// </summary>
        public string ClientLinkName { get; set; }
        /// <summary>
        /// 收款银行
        /// </summary>
        public string ClientBank { get; set; }
        /// <summary>
        /// 收款银行账号
        /// </summary>
        public string ClientBankNum { get; set; }
        /// <summary>
        /// 收款银行地址
        /// </summary>
        public string ClientBankAddress { get; set; }     
        /// <summary>
        /// 付汇单位ID
        /// </summary>
        public string Provider { get; set; }      
        /// <summary>
        /// 付汇单位名称
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// 收款单位联系人
        /// </summary>
        public string ProviderLinkName { get; set; }
        /// <summary>
        /// 收款银行
        /// </summary>
        public string ProviderBank { get; set; }
        /// <summary>
        /// 收款银行账号
        /// </summary>
        public string ProviderBankNum { get; set; }
        /// <summary>
        /// 收款银行地址
        /// </summary>
        public string ProviderBankAddress { get; set; }
    }

    public class FeeApplyListModel
    {
        /// <summary>
        /// 单据号，新增的时候为0
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 付款类型ID
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 付款类型名称
        /// </summary>
        public string PayTypeName { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CorpID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CorpName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 业务员ID
        /// </summary>
        public string EmployeeID { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 摘要（不能为空）
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal FAmount { get; set; }
    }
}
