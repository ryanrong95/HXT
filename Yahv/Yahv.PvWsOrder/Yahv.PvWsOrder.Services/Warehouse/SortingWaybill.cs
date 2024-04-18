using System;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Warehouse
{
    public class SortingWaybill
    {
        public SortingWaybill(){}

        /// <summary>
        /// 通知ID
        /// </summary>
        /// <remarks>运单的唯一码</remarks>
        public string WaybillID { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        /// <remarks>实际的运单号</remarks>
        public string Code { get; set; }

        /// <summary>
        /// 承运商名字
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <remarks>
        /// 通知时间
        /// </remarks>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        /// <remarks>目前中港贸易中的特殊字段</remarks>
        public string EnterCode { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        public CenterFileDescription[] Files { get; set; }

        /// <summary>
        /// 运单入库条件
        /// </summary>
        public WayCondition Condition { get; set; }

        /// <summary>
        /// 分拣通知
        /// </summary>
        public SortingNotice[] Notices
        {
            get;set;
        }

        public CgNoticeSource Source
        {
            get;set;
        }

        /// <summary>
        /// 运单类型
        /// </summary>
        public WaybillType WaybillType { get; set; }


        public Origin Place { get; set; }
    }

    public class SortingNotice
    {
        public SortingNotice() { }
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 库房ID
        /// </summary>
        public string WareHouseID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WayBillID { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public CgNoticeType Type { get; set; }
        /// <summary>
        /// 通知状态
        /// </summary>
        public NoticesStatus Status { get; set; }
        /// <summary>
        /// 分拣目标
        /// </summary>
        public NoticesTarget Target { get; set; }
        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// 进项
        /// </summary>
        public Input Input { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        public CenterProduct Product { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 实到数量
        /// </summary>
        public int TruetoQuantity { get; set; }
        /// <summary>
        /// 已到库房数量
        /// </summary>
        public int SortedQuantity { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public CgNoticeSource Source { get; set; }
        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }
        /// <summary>
        /// 货架编号
        /// </summary>
        public string ShelveID { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }
        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }
        /// <summary>
        /// 通知条件
        /// </summary>
        public NoticeCondition Condition { get; set; }
    }

    public class Input
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 全局唯一码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 原inputid，拆项是用，不拆项时为空
        /// </summary>
        public string OriginID { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string ItemID { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 第三方收款人
        /// </summary>
        public string ThirdID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string TrackerID { get; set; }
        /// <summary>
        /// 销售员ID
        /// </summary>
        public string SalerID { get; set; }
        /// <summary>
        /// 采购员
        /// </summary>
        public string PurchaserID { get; set; }
        /// <summary>
        /// 币值
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }
        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
