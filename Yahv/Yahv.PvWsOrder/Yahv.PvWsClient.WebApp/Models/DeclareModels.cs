using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Yahv.PvWsClient.WebApp.Models
{

    /// <summary>
    /// 请求响应数据
    /// </summary>
    public class ResponseData
    {
        public string success { get; set; }

        public string message { get; set; }

        public string url { get; set; }
    }

    /// <summary>
    /// 可用额度
    /// </summary>
    public class CreditAvailable
    {
        public decimal GoodsCredit { get; set; }

        public decimal TaxCredit { get; set; }

        public decimal AgentCredit { get; set; }

        public decimal OtherCredit { get; set; }

        public CreditAvailable()
        {
            this.GoodsCredit = this.TaxCredit = this.AgentCredit = this.OtherCredit = 0;
        }
    }

    /// <summary>
    /// 新增订单
    /// </summary>
    public class DeclareAddViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 是否已归类
        /// </summary>
        public bool IsClssified { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] HKFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public string WrapType { get; set; }

        /// <summary>
        /// 香港接货方式：供应商送货、自提
        /// </summary>
        public string HKDeliveryType { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string WayBillNo { get; set; }

        /// <summary>
        /// 供应商（香港交货方式）
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 供应商提货地址(id)
        /// </summary>
        public string SupplierAddress { get; set; }

        public string SupplierAddressOptions { get; set; }

        /// <summary>
        /// 供应商提货地址
        /// </summary>
        public string SupplierAddressName { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? PickupTime { get; set; }

        //提货时间
        public string PickupTimeStr { get; set; }

        /// <summary>
        /// 国内交货方式
        /// </summary>
        public string SZDeliveryType { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string ClientPicker { get; set; }

        /// <summary>
        /// 提货人手机号码
        /// </summary>
        public string ClientPickerMobile { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>

        public string IDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称(ID)
        /// </summary>
        public string ClientConsignee { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称
        /// </summary>
        public string ClientConsigneeName { get; set; }

        /// <summary>
        /// 国内交货方式收货地址
        /// </summary>
        public string ClientConsigneeAddress { get; set; }

        /// <summary>
        /// 国内交货方式收货地址联系人
        /// </summary>
        public string ClientContact { get; set; }

        /// <summary>
        /// 国内交货方式收货地址联系人手机号码
        /// </summary>
        public string ClientContactMobile { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string[] PayExchangeSupplier { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public bool IsFullVehicle { get; set; }

        /// <summary>
        /// 是否垫款
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public string PackNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsSubmit { get; set; }

        /// <summary>
        /// 是否预付款
        /// </summary>
        public bool IsPrePaid { get; set; }
    }
}