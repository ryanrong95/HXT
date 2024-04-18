using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 送货人View
    /// </summary>
    public class ConsigneeView : UniqueView<Models.Consignee, ScCustomsReponsitory>
    {
        public ConsigneeView()
        {
        }

        internal ConsigneeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Consignee> GetIQueryable()
        {
            return from deliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Consignees>()
                   select new Models.Consignee
                   {
                       ID = deliver.ID,
                       Name = deliver.Name,
                       Mobile = deliver.Mobile,
                       IDType = (Enums.IDType)deliver.IDType,
                       IDNumber = deliver.IDNumber,
                       CreateDate = deliver.CreateDate,
                   };
        }
    }
}
