using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 月结账单
    /// </summary>
    public class InvoiceFilesTopView : QueryView<InvoiceFile, PvbCrmReponsitory>
    {
        public InvoiceFilesTopView()
        {

        }

        public InvoiceFilesTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<InvoiceFile> GetIQueryable()
        {
            return  new Yahv.Services.Views.InvoiceFilesTopView<PvbCrmReponsitory>().Where(x=>x.FileType== 17 || x.FileType==18);
        }
    }
}

