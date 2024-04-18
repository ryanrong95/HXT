using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PreProductPostLogView : UniqueView<Models.PostBack, ScCustomsReponsitory>
    {
        public PreProductPostLogView()
        {
        }

        internal PreProductPostLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PostBack> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductPostLog>()
                   select new Models.PostBack
                   {
                       ID = para.ID,
                       id = para.PreProductCategoryID,
                       status = para.PostStatus.Value,
                       msg = para.Msg,
                       RecordStatus = (Status)para.Status.Value,
                       CreateDate = para.CreateDate.Value,
                   };
        }
    }
}
