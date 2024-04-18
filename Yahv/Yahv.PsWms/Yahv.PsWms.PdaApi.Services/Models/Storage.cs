using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    /// <summary>
    /// 库存
    /// </summary>
    public class Storage : IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 通知ItemID
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
        /// 库存类型: Store 库存库 1, Flow 流水库 2, Park 暂存库 3, Ordering 在途库 4, Scrap 报废库 100	
        /// </summary>
        public Enums.StorageType Type { get; set; }

        /// <summary>
        /// 锁止库存
        /// </summary>
        public bool Islock { get; set; }

        /// <summary>
        /// 盘点类型: Single 按个 1, MinPackage 最小包装 2
        /// </summary>
        public Enums.StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 入库分拣人ID
        /// </summary>
        public string SorterID { get; set; }

        /// <summary>
        /// 入库单ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 入库单ItemID
        /// </summary>
        public string FormItemID { get; set; }

        /// <summary>
        /// 入库币种
        /// </summary>
        public Underly.Currency Currency { get; set; }

        /// <summary>
        /// 入库单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 存储库位
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 异常备注
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 信息备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 库存的唯一标识
        /// </summary>
        public string Unique { get; set; }
    }
}
