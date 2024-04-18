using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Waybill
    {
        #region 属性
        public string ID { get; set; }

        public string FatherID { get; set; }

        /// <summary>
        /// 运单编码
        /// </summary>
        public string Code { get; set; }

        ///// <summary>
        ///// 运单类型
        ///// </summary>
        public WaybillType Type { get; set; }

        /// <summary>
        /// 子运单号(以逗号分隔)
        /// </summary>
        public string Subcodes { get; set; }

        /// <summary>
        /// 承运商ID
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 交货人ID
        /// </summary>
        public string ConsignorID { get; set; }

        /// <summary>
        /// 收货人ID
        /// </summary>
        public string ConsigneeID { get; set; }

        /// <summary>
        /// 运费支付人
        /// </summary>
        public WaybillPayer FreightPayer { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? TotalParts { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal? TotalCount { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// 总体积
        /// </summary>
        public decimal? TotalVolume { get; set; }

        /// <summary>
        /// 客户承运商账户
        /// </summary>
        public string CarrierAccount { get; set; }

        /// <summary>
        /// 航次号 (只在国际运单中起作用)
        /// </summary>
        public string VoyageNumber { get; set; }

        /// <summary>
        /// 货运条件：开箱验货，
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        ///// <summary>
        ///// 状态
        ///// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 建议时间
        /// </summary>
        public DateTime AppointTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 转运（连接的就是我们的WaybillID）编码
        /// </summary>
        public string TransferID { get; set; }

        /// <summary>
        /// 是否完成清关手续（只在香港快递有用）
        /// </summary>
        public bool? IsClearance { get; set; }

        public string Packaging { get; set; }

        public string Supplier { get; set; }

        public string Summary { get; set; }

        public int? ExcuteStatus { get; set; }

        public int? CuttingOrderStatus { get; set; }

        public int? InitExType { get; set; }
        public int? InitExPayType { get; set; }

        /// <summary>
        /// 确认收货状态：100 无，200已确认
        /// </summary>
        public int? ConfirmReceiptStatus { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 业务来源
        /// </summary>
        public CgNoticeSource Source { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public CgNoticeType NoticeType { get; set; }

        /// <summary>
        /// 临时入仓单号
        /// </summary>
        public string TempEnterCode { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 交货人
        /// </summary>
        public WayParter Consignor { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public WayParter Consignee { get; set; }

        /// <summary>
        /// 中港报关
        /// </summary>
        public WayChcd WayChcd { get; set; }

        /// <summary>
        /// 提货信息
        /// </summary>
        public WayLoading WayLoading { get; set; }

        public WayCharge WayCharge { get; set; }

        /// <summary>
        /// 承运商名称
        /// </summary>
        public string CarrierName { get; set; }
        

        #endregion

        public Waybill()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }
    }
}
