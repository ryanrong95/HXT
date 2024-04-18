using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseCusReceiptCodesView : UniqueView<Models.BaseCusReceiptCode, ScCustomsReponsitory>
    {
        public BaseCusReceiptCodesView()
        {
        }

        internal BaseCusReceiptCodesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseCusReceiptCode> GetIQueryable()
        {
            return from cusReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCusReceiptCode>()
                   select new Models.BaseCusReceiptCode
                   {
                       ID = cusReceipt.ID,
                       Code = cusReceipt.Code,
                       Name = cusReceipt.Name,
                   };
        }
    }
}
