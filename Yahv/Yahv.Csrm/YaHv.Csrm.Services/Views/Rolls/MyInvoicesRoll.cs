using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 当前登录人的发票
    /// </summary>
    public class MyTradingInvoicesRoll : Origins.TradingInvoiceOrigin
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingInvoicesRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingInvoice> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                return base.GetIQueryable().GroupBy(item => item.ID).Select(g => g.First());
            }
            else
            {
                return base.GetIQueryable().Where(item => item.CreatorID == this.admin.ID);
            }
        }
        /// <summary>
        /// 匹配企业
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        public IQueryable<TradingInvoice> Match(string enterpriseid)
        {
            return this.Where(item => item.EnterpriseID == enterpriseid);
        }

    }

}
