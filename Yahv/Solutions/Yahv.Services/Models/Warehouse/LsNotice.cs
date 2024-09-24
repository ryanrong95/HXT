using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 租赁通知
    /// </summary>
    public class LsNotice : IUnique
    {
        #region 属性

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///  租赁等级
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 租赁数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 开始时间 
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 收款人ID，这里指平台所属公司：华芯通等（内部公司）
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 状态:待支付（租赁订单专有，租赁通知没有）,待分配,已分配,已到期,已关闭
        /// </summary>
        public LsOrderStatus Status { get; set; }

        /// <summary>
        /// 接受到的库位ID的集合(若已分配返回出去)
        /// </summary>
        public string[] ShelveIDs { get; set; }

        //可用的库位ID 的集合
        public object[] UsableShelves
        {
            get;
            set;
        }

        /// <summary>
        /// 续租时启用，获取以前的租赁通知
        /// </summary>
        public LsNotice PreLsNotice { get; set; }

        #endregion

        #region 扩展属性
        // <summary>
        /// Status的枚举描述
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }
        #endregion

    }

    public class LsNoticeSubmit
    {
        public LsNotice[] List { get; set; }
    }
}
