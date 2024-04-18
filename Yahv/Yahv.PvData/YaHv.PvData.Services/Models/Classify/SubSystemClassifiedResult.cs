using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 子系统归类结果
    /// </summary>
    public class SubSystemClassifiedResult : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 主ID(OrderID|预归类产品ID)
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        public ClassifyStep Step { get; set; }

        /// <summary>
        /// 归类信息ID
        /// </summary>
        public string CpnID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime? OrderedDate { get; set; }

        /// <summary>
        /// 合同发票
        /// </summary>
        public string PIs { get; set; }

        /// <summary>
        /// 回发的地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 客户自定义品名
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        internal SubSystemClassifiedResult()
        {
        }
    }
}
