using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 大赢家收款
    /// </summary>
    public class PayeeLeftDyjDto
    {
        public string ID { get; set; }
        public string 制单日期 { get; set; }
        public string 收款类型 { get; set; }
        public string 金库 { get; set; }
        public string 帐户 { get; set; }
        public string 结算方式 { get; set; }
        public string 分公司 { get; set; }
        public string 部门 { get; set; }
        public string 员工 { get; set; }
        public decimal 收款总金额 { get; set; }
        public decimal 外币总金额 { get; set; }
        public string 支票金额 { get; set; }
        public string 返款金额 { get; set; }
        public string 付款单位 { get; set; }
        public string 制单人 { get; set; }
        public string 摘要 { get; set; }
        public decimal 明细金额 { get; set; }
        public decimal 外币 { get; set; }
        public string 折扣 { get; set; }
        public string 明细返款金额 { get; set; }
        public string 备注 { get; set; }
    }
}
