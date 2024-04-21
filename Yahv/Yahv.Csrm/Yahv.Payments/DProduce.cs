using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Linq;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 存储过程逻辑层
    /// </summary>
    internal class DProduce
    {
        /// <summary>
        /// 是否逾期
        /// </summary>
        /// <param name="payer">付款公司</param>
        /// <param name="payee">收款公司</param>
        /// <param name="business">业务</param>
        /// <param name="date">日期</param>
        /// <param name="type">视图分类</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        static public IEnumerable<dp_OverdueResult> Overdue(string payer, string payee, string business, DateTime date, OverdueType type = OverdueType.All, Currency currency = Currency.CNY)
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                return reponsitory.Current.dp_Overdue(payer, payee, business, date, type.ToString(), (int)currency).ToList();
            }
        }

        /// <summary>
        /// 是否逾期
        /// </summary>
        /// <param name="payer">付款公司</param>
        /// <param name="payee">收款公司</param>
        /// <param name="business">业务</param>
        /// <param name="date">日期</param>
        /// <param name="type">视图分类</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        static public IEnumerable<dp_OverdueResult> Overdue(string payer, string payee, string business, DateTime date, PvbCrmReponsitory reponsitory, OverdueType type = OverdueType.All, Currency currency = Currency.CNY)
        {
            return reponsitory.Current.dp_Overdue(payer, payee, business, date, type.ToString(), (int)currency).ToList();            
        }
    }
}
