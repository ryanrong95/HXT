using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.PageModels
{
    /// <summary>
    /// 深圳库房出库通知
    /// </summary>
    public class SZWarehouseExitingListModel
    {
        public string ID { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 送货类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int PackagesNumber { get; set; }

        /// <summary>
        ///打印状态
        /// </summary>
        public string IsPrint { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string Admin { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
    }

    /// <summary>
    /// 深圳库房入库通知
    /// </summary>
    public class SZWarehouseEntryListModel : Needs.Wl.Warehouse.Services.Models.SZEntryNotice
    {
        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 客户公司名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}