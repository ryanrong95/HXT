using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 实付视图
    /// </summary>
    public class PaymentsView : QueryView<Payment, PvbCrmReponsitory>
    {
        public PaymentsView()
        {

        }

        public PaymentsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Payment> GetIQueryable()
        {
            return new Yahv.Services.Views.PaymentsTopView<PvbCrmReponsitory>();
        }
    }
}
