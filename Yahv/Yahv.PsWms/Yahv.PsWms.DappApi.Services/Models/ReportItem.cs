using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class ReportItem : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报告ID
        /// </summary>
        public string ReportID { get; set; }

        /// <summary>
        /// 通知ID, 只做记录不要关联
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 通知ItemID, 只做记录不要关联
        /// </summary>
        public string NoticeItemID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 最小包装量, 通知数量，董建补充：如果无通知到货为0
        /// </summary>
        public int NoticeMpq { get; set; }

        /// <summary>
        /// 通知数量, 件数
        /// </summary>
        public int NoticePackageNumber { get; set; }

        /// <summary>
        /// 通知数量, 总数
        /// </summary>
        public int NoticeTotal { get; set; }

        /// <summary>
        /// 库存操作,最小包装量
        /// </summary>
        public int StorageMpq { get; set; }

        /// <summary>
        /// 库存操作, 件数
        /// </summary>
        public int StoragePackageNumber { get; set; }

        /// <summary>
        /// 库存操作, 总数
        /// </summary>
        public int StorageTotal { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 单据ItemID
        /// </summary>
        public string FormItemID { get; set; }

        /// <summary>
        /// 入库币种, Currency 枚举, 默认人民币
        /// </summary>
        public Currency? InCurrency { get; set; }

        /// <summary>
        /// 入库单价, 默认0
        /// </summary>
        public decimal? InUnitPrice { get; set; }

        /// <summary>
        /// 出库币种, Currency, 默认人民币
        /// </summary>
        public Currency? OutCurrency { get; set; }

        /// <summary>
        /// 出库单价, 默认0
        /// </summary>
        public decimal? OutUnitPrice { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 异常备注, 如果写入就代表有异常 否则为null
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 库位ID
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 库位Code
        /// </summary>
        public string ShelveCode { get; set; }

        #endregion

        #region 扩展属性

        public Product Product { get; set; }

        #endregion
    }
}
