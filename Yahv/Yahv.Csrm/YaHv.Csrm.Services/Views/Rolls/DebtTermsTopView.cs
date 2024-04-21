using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 账期条款
    /// </summary>
    public class DebtTermsTopView : QueryView<DebtTerm, PvbCrmReponsitory>
    {
        protected override IQueryable<DebtTerm> GetIQueryable()
        {
            return new Yahv.Services.Views.DebtTermsTopView<PvbCrmReponsitory>();
        }

        public DebtTerm this[string payer, string payee, string catalog]
        {
            get
            {
                return
                    this.SingleOrDefault(
                        item => item.Payee == payee && item.Payer == payer && item.Catalog == catalog);
            }
        }

        public DebtTerm this[string payer, string payee]
        {
            get
            {
                return
                    this.SingleOrDefault(
                        item => item.Payee == payee && item.Payer == payer);
            }
        }
    }
}
