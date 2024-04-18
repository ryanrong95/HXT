using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    /// <summary>
    /// 仓储对账列表查询 Model
    /// </summary>
    public class GetStorageVoucherListSearchModel
    {
        /// <summary>
        /// page
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// rows
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string CutDateIndex { get; set; }
    }

    /// <summary>
    /// 仓储对账列表返回结果 Model
    /// </summary>
    public class GetStorageVoucherListReturnModel
    {
        /// <summary>
        /// 期号
        /// </summary>
        public string CutDateIndex { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}