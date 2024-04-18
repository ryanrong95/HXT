using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.WebApi.Models
{
    /// <summary>
    /// 内单订单model
    /// </summary>
    public class PvWsOrderInsApiModel
    {
        /// <summary>
        ///主订单号
        /// </summary>
        public string VastOrderID { get; set; }
        
        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 调用代仓储接口时，为Null
        /// 调用库房接口时，进行赋值
        /// </summary>
        public string WayBillID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 香港库房ID
        /// </summary>
        public string HKWareHouseID { get; set; }

        /// <summary>
        /// 跟单员ID
        /// </summary>
        public string AdminID { get; set; }


        /// <summary>
        /// 报关公司
        /// </summary>
        public string DeclarationCompany { get; set; }

        /// <summary>
        /// 订单的香港接货方式
        /// </summary>
        public PvConsignee OrderConsignee { get; set; }

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        public PvConsignor OrderConsignor { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public List<string> PayExchangeSuppliers { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public List<PvOrderItems> Items { get; set; }

    }

    /// <summary>
    /// 订单项
    /// </summary>
    public class PvOrderItems
    {
        /// <summary>
        /// 芯达通订单项ID（pickKey生成）
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 产品唯一编码
        /// </summary>
        public string ProductUnicode { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 品名（也是报关品名）
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>

        public string Brand { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 单价
        /// </summary>

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>

        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>

        public decimal Quantity { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>

        public string Unit { get; set; }

        #region 归类信息

        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { get; set; }

        /// <summary>
        /// 检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>

        public string Elements { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>

        public string TaxName { get; set; }

        /// <summary>
        /// 税务编号
        /// </summary>

        public string TaxCode { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>

        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>

        public decimal? ValueAddedRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? ExciseTaxRate { get; set; }

        /// <summary>
        /// 法一单位
        /// </summary>

        public string FirstLegalUnit { get; set; }

        /// <summary>
        /// 法二单位
        /// </summary>

        public string SecondLegalUnit { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>

        public bool CCC { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool Embargo { get; set; }


        /// <summary>
        /// 香港管控
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? CIQprice { get; set; }

        /// <summary>
        /// 高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        /// <summary>
        /// 检疫
        /// </summary>
        public bool IsDisinfected { get; set; }

        /// <summary>
        /// 归类时间
        /// </summary>
        public DateTime? ClassifyTime { get; set; }

        /// <summary>
        /// 归类状态
        /// </summary>
        public ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 第二次归类人
        /// </summary>
        public string ClassifySecondAdminID { get; set; }
        #endregion

        #region 分拣信息

        /// <summary>
        /// 箱号
        /// </summary>
        public string PackNo { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        #endregion
    }

    /// <summary>
    /// 订单的香港接货方式
    /// </summary>
    public class PvConsignee
    {
        //此处根据Name和ClientID取得供应商ID
        public string ClientSupplierName { get; set; }

        /// <summary>
        /// 香港接货方式：1：供应商送货、2：自提
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 联系人, 交货方式为自提时填写
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码, 交货方式为自提时填写
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话, 交货方式为自提时填写（可空）
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 交货地址, 交货方式为自提时填写
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货时间，交货方式为自提时填写
        /// </summary>
        public DateTime? PickUpTime { get; set; }

        /// <summary>
        /// 物流单号，供应商送货时填写
        /// </summary>
        public string WayBillNo { get; set; }
    }

    /// <summary>
    /// 订单的深圳交货方式
    /// </summary>
    public class PvConsignor
    {
        /// <summary>
        /// 订单交货方式：1：自提、2：送货上门、3：快递
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 收件单位名称（送货上门/快递时填写）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人（送货上门/快递时），提货人（客户自提时）
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码（送货上门/快递时），提货人手机号码（客户自提时）
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话（送货上门/快递时），提货人电话（客户自提时），可空
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 收件地址（送货上门/快递时填写）
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货人证件类型：身份证、驾驶证（客户自提时填写）
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 提货人证件号码（客户自提时填写）
        /// </summary>
        public string IDNumber { get; set; }
    }
}