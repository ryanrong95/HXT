using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Waybills
    {
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
        /// 客户名称
        /// </summary>
        /// <remarks>
        /// 客户企业名称
        /// </remarks>
        public string ClientName { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 运单类型
        /// </summary>
        public WaybillType WaybillType { get; set; }

        /// <summary>
        /// 承运商编号
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 承运商名字
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }


        public string Place { get; set; }

        internal string Condition { get; set; }


        public string ConsignorID { get; set; }
        public string ConsigneeID { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public WayParter Consignee { get; set; }

        /// <summary>
        /// 交货人
        /// </summary>
        public WayParter Consignor { get; set; }

        /// <summary>
        /// 提货人信息
        /// </summary>
        public WayLoading WayLoading { get; set; }

        public WayChcd WayChcd { get; set; }
        public WayCharge WayCharge { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? TotalParts { get; set; }

        /// <summary>
        /// 总重量（冗余运单）
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// 总体积
        /// </summary>
        public decimal? TotalVolume { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyageNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        public GeneralStatus Status { get; set; }

        internal CenterFileDescription[] DataFiles { get; set; }

        public string Packaging { get; set; }

        public string TransferID { get; set; }
        public string FatherID { get; set; }
    }
}
