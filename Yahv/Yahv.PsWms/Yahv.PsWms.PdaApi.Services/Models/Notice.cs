using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    /// <summary>
    /// 通知
    /// </summary>
    public class Notice : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 所属客户
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 内部公司，所属公司
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 通知类型: Inbound 入库 1, Outbound 出库 2, InAndOut 即入即出 3,
        /// </summary>
        public Enums.NoticeType NoticeType { get; set; }

        /// <summary>
        /// 来自的订单ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 交货人信息ID
        /// </summary>
        public string ConsignorID { get; set; }

        /// <summary>
        /// 收货人信息ID
        /// </summary>
        public string ConsigneeID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 通知状态
        /// </summary>
        public Enums.NoticeStatus Status { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 跟单
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 异常备注
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 信息备注
        /// </summary>
        public string Summary { get; set; }
        #endregion
    }
}
