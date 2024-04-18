using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class FlowAccount : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 流水账类型（信用、信用花费、现金、银行(电汇)、信用总账）
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 币种（暂存人民币）
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 汇率（对人民币汇率）
        /// </summary>
        public decimal? ERate1 { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行、现金收付款手续的流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// Admin
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 创建人日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 结转期号
        /// </summary>
        public int? ChangeIndex { get; set; }

        /// <summary>
        /// 发生期号 发生期号 （yyyyMM）
        /// </summary>
        public int? OriginIndex { get; set; }

        /// <summary>
        /// 还款日期
        /// </summary>
        public DateTime? OriginalDate { get; set; }

        /// <summary>
        /// 结转日期
        /// </summary>
        public DateTime? ChangeDate { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 银行卡账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int DateIndex { get; set; }
        #endregion
    }
}
