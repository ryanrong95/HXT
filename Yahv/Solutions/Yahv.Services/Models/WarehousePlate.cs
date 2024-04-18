using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 子库房视图
    /// </summary>
    public class WarehousePlate : IUnique
    {
        #region 属性
        public string ID { set; get; }

        /// <summary>
        /// 门牌（子库房名称）
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 子库房地址
        /// </summary>

        public string Address { set; get; }

        /// <summary>
        /// 子库房Code
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZip { set; get; }
        /// <summary>
        /// 子库房状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 主库房ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 主库房名称
        /// </summary>
        public string WarehouseName { set; get; }
        /// <summary>
        /// 主库房Code
        /// </summary>
        public string WsCode { set; get; }
        /// <summary>
        /// 主库房地址
        /// </summary>
        public string WarehouseAddress { set; get; }
        /// <summary>
        /// 大库房视图
        /// </summary>
        public ApprovalStatus WareHouseStatus { set; get; }
        #endregion
    }
}
