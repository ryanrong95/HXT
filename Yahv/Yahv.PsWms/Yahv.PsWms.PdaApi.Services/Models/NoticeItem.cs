using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    /// <summary>
    /// 通知项
    /// </summary>
    public class NoticeItem : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 通知来源 NoticeSource: Keeper	库房 1 库管, Tracker	跟单 2 客服 
        /// </summary>
        public Enums.NoticeSource Source { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 自定义编码
        /// </summary>
        public string CustomCode { get; set; }

        /// <summary>
        /// 盘点类型
        /// </summary>
        public Enums.StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 最小包装量, 实际录入的如果是按个点数默认1
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 件数 实际录入的, 如果是按个点数默认, Total
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 总数, 计算的或是实际录入的
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Yahv.Underly.Currency Currency { get; set; }

        /// <summary>
        /// 金额, 默认0
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 所属单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 所属单据ID
        /// </summary>
        public string FormItemID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 分拣人
        /// </summary>
        public string SorterID { get; set; }

        /// <summary>
        /// 捡货人
        /// </summary>
        public string PickerID { get; set; }

        #endregion
    }
}
