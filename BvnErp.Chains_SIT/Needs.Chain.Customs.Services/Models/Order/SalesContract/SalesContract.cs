using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 内销合同
    /// </summary>
    public class SalesContract : IUnique
    {
        #region 属性
        /// <summary>
        /// 主订单ID-合同号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 合同日期
        /// </summary>
        public DateTime SalesDate { get; set; }

        /// <summary>
        /// 合同日期-转换
        /// </summary>
        public string SalesDateText
        {
            get
            {
                return this.SalesDate.ToString("yyyy年MM月dd日");
            }
        }

        /// <summary>
        /// 合同有效期,默认一个月
        /// </summary>
        public string ValidDate
        {
            get
            {
                return SalesDate.AddMonths(1).ToString("yyyy年MM月dd日");
            }
        }

        /// <summary>
        /// 买方信息
        /// </summary>
        public InvoiceBaseInfo Buyer { get; set; }

        /// <summary>
        /// 卖方信息
        /// </summary>
        public InvoiceBaseInfo Seller { get; set; }

        /// <summary>
        /// 合同内容-型号信息
        /// </summary>
        public List<ContractItem> ContractItems { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }


        #endregion

        /// <summary>
        /// 当跟单员审核代理报关委托书通过时发生
        /// </summary>
        public event OrderFileAuditedHanlder Approved;

        public SalesContract()
        {
            this.Approved += SalesContract_Approved;
        }

        private void SalesContract_Approved(object sender, OrderFileAuditedEventArgs e)
        {
            var order = e.AgentProxy.Order;
            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]审核通过了客户上传的销售合同");
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Approve(string fileID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, fileID);
            }

            //this.OnApproved();

            #region 审批通过调用代仓储
            var confirm = new FileApprove
            {
                OrderID = this.ID,
                Type = Enums.FileType.SalesContract
            };

            var apisetting = new ApiSettings.PvWsOrderApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.FileApprove;
            var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);

            if (message.code != 200)
            {
                throw new Exception(message.data);
            }
            #endregion

        }

    }



    /// <summary>
    /// 发票基础信息
    /// </summary>
    public class InvoiceBaseInfo
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; } = string.Empty;

        /// <summary>
        /// 银行名称--开户行
        /// </summary>
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; } = string.Empty;

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; } = string.Empty;

        /// <summary>
        /// 卖方公章URL
        /// </summary>
        public string SealUrl { get; set; }
    }

    /// <summary>
    /// 合同型号项
    /// </summary>
    public class ContractItem
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice
        {
            get
            {
                return (TotalPrice / Quantity).ToRound(4);
            }
        }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

    }
}
