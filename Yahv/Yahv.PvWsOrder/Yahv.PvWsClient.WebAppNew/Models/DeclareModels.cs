using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    /// <summary>
    /// 新增报关订单
    /// </summary>
    public class DeclareAddViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 是否是转报关订单
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] HKFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 装箱单
        /// </summary>
        public FileModel[] PackingFiles { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public bool CharteredBus { get; set; }

        /// <summary>
        /// 香港仓库地址
        /// </summary>
        public string WareHouseAddress { get; set; }

        /// <summary>
        /// 香港仓库联系人
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 香港仓库联系人电话
        /// </summary>
        public string WareHouseTel{ get; set; }

        /// <summary>
        /// 香港是否垫付运费
        /// </summary>
        public bool HKFreight { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string HKExpressNumber { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string HKExpressName { get; set; }

        /// <summary>
        /// 自提地址
        /// </summary>
        public string SZPickAddress { get; set; }

        /// <summary>
        /// 香港接货方式：供应商送货、自提
        /// </summary>
        public string HKDeliveryType { get; set; }

        /// <summary>
        /// 供应商提货地址(id)
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? PickupTime { get; set; }

        //提货时间
        public string PickupTimeStr { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string ClientPicker { get; set; }

        /// <summary>
        /// 提货人手机号码
        /// </summary>
        public string ClientPickerMobile { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? SZPickupTime { get; set; }


        //提货时间
        public string SZPickupTimeStr { get; set; }

        /// <summary>
        /// 国内交货方式
        /// </summary>
        public string SZDeliveryType { get; set; }

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
        /// 国内交货方式收货地址联系人手机号码
        /// </summary>
        public string ClientContactMobile { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string[] PayExchangeSupplier { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }


        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsSubmit { get; set; }

        public bool IsEdit { get; set; }

        /// <summary>
        /// 是否预归类
        /// </summary>
        public bool IsClssified { get; set; }

    }

    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string ProductUnicode { get; set; }

        /// <summary>
        /// 归类产品ID
        /// </summary>
        public string PreproductID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal StockNum { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public int Unit { get; set; }

        /// <summary>
        /// 单价名称
        /// </summary>
        public  string UnitName { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        public string InputID { get; set; }

        public string OutputID { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// 币种 ShortName
        /// </summary>
        public string CurrencyShortName { get; set; }

        /// <summary>
        /// 币种 Int
        /// </summary>
        public string CurrencyInt { get; set; }
    }

    /// <summary>
    /// 特殊货物要求数据模型
    /// </summary>
    public class SpecialGoodsModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 要求名称ID
        /// </summary>
        public string NameValue { get; set; }

        /// <summary>
        /// 要求名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int ? Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 具体要求
        /// </summary>
        public string Requirement { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileURL { get; set; }

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileFullURL { get; set; }
    }

    public class LinkToDeclareListModel
    {
        public string dec_status { get; set; }
    }
}