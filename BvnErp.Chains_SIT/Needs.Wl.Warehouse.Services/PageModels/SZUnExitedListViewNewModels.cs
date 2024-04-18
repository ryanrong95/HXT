using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Warehouse.Services.PageModels
{
    public class SZUnExitedListViewNewModels : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// ExitNoticeID
        /// </summary>
        public string ExitNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// 件数
        /// </summary>
        public int PackNo { get; set; }

        /// <summary>
        /// 送货类型
        /// </summary>
        public Needs.Wl.Models.Enums.ExitType ExitType { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string AdminName { get; set; } = string.Empty;

        /// <summary>
        /// 出库状态
        /// </summary>
        public Needs.Wl.Models.Enums.ExitNoticeStatus ExitNoticeStatus { get; set; }

        /// <summary>
        /// 打印状态
        /// Enums.IsPrint
        /// </summary>
        public int? IsPrint { get; set; }

        /// <summary>
        /// ExitNotice 生成时间，即制单时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
