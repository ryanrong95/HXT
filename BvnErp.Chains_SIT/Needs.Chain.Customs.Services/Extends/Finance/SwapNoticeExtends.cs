using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品扩展方法
    /// </summary>
    public static class SwapNoticeExtends
    {
        /// <summary>
        /// 换汇通知
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.SwapNotices ToLinq(this Models.SwapNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.SwapNotices
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                BankName = entity.BankName,
                Currency = entity.Currency,
                TotalAmount = entity.TotalAmount,
                ExchangeRate = entity.ExchangeRate,
                TotalAmountCNY = entity.TotalAmountCNY,
                Status = (int)entity.SwapStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                OutFinanceAccountID = entity.OutAccount?.ID,
                InFinanceAccountID = entity.InAccount?.ID,
                MidFinanceAccountID = entity.MidAccount?.ID,
                Poundage = entity.Poundage,
            };
        }
    }
}
