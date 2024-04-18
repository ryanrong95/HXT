using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 北京总部管控记录的视图（Admin过滤）
    /// </summary>
    public class MyHQControlRecordsView : HQControlRecordsView
    {
        IGenericAdmin Admin;

        public MyHQControlRecordsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderControlRecord> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from record in base.GetIQueryable()
                   where record.Admin.ID == this.Admin.ID &&
                        record.Status == Ccs.Services.Enums.Status.Normal

                   select record;
        }
    }
}
