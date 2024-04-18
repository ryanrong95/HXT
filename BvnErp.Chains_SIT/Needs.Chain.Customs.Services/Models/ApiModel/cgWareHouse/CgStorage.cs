using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 库存
    /// </summary>
    public class CgStorage
    {
        #region 属性
        /// <summary>
        /// 唯一标识 四位年+2位月+2位日+6位流水
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 库存类型:流水库、库存库、运营库、报废库、检测库、暂存库、异常库、退货库
        /// </summary>
        public CgStoragesType Type { get; set; }
        /// <summary>
        /// 库房编号
        /// </summary>
        public string WareHouseID { get; set; }
        /// <summary>
        /// 分拣编号
        /// </summary>
        public string SortingID { get; set; }
        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 通知编号
        /// </summary>
        public string NoticeID { get; set; }
        /// <summary>
        /// 原产地
        /// </summary>
        public Origin Origin { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 状态：200、正常；400、删除；500、停用
        /// </summary>
        public StoragesStatus Status { get; set; }
        /// <summary>
        /// 库位编号
        /// </summary>
        public string ShelveID { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }
        /// <summary>
        /// 摘要描述
        /// </summary>
        public string Summary { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 库存流水
        /// </summary>
        //public Form[] Forms { get; set; }

        /// <summary>
        /// 中心产品信息
        /// </summary>
        public CenterProduct Product { get; set; }

        /// <summary>
        /// 进项
        /// </summary>
        public Input Input { get; set; }

        #endregion
    }
}
