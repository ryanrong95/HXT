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
    public class BaseCorrelationReasonView : UniqueView<Models.BaseCorrelationReason, ScCustomsReponsitory>
    {
        public BaseCorrelationReasonView()
        {
        }

        internal BaseCorrelationReasonView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseCorrelationReason> GetIQueryable()
        {
            return from reason in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCorrelationReason>()
                   select new Models.BaseCorrelationReason
                   {
                       ID = reason.ID,
                       Code = reason.Code,
                       Name = reason.Name,
                   };
        }
    }
}