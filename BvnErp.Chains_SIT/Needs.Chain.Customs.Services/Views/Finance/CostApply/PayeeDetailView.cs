using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayeeDetailView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PayeeDetailView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PayeeDetailView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public PayeeDetailViewModel GetResult(string costApplyPayeeID)
        {
            var costApplyPayees = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplyPayees>();

            var result = from costApplyPayee in costApplyPayees
                         where costApplyPayee.ID == costApplyPayeeID
                         select new PayeeDetailViewModel
                         {
                             CostApplyPayeeID = costApplyPayee.ID,
                             PayeeName = costApplyPayee.PayeeName,
                             PayeeAccount = costApplyPayee.PayeeAccount,
                             PayeeBank = costApplyPayee.PayeeBank,
                         };

            return result.FirstOrDefault();
        }
    }

    public class PayeeDetailViewModel
    {
        public string CostApplyPayeeID { get; set; } = string.Empty;

        public string PayeeName { get; set; } = string.Empty;

        public string PayeeAccount { get; set; } = string.Empty;

        public string PayeeBank { get; set; } = string.Empty;
    }
}
