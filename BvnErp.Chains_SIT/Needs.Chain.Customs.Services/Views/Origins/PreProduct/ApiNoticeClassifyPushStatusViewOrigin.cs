using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class ApiNoticeClassifyPushStatusViewOrigin : UniqueView<Models.ApiNoticeClassifyPushStatusModel, ScCustomsReponsitory>
    {
        internal ApiNoticeClassifyPushStatusViewOrigin()
        {
        }

        internal ApiNoticeClassifyPushStatusViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ApiNoticeClassifyPushStatusModel> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNoticeClassifyPushStatusView>()
                   select new Models.ApiNoticeClassifyPushStatusModel
                   {
                       PreProductCategoryID = entity.PreProductCategoryID,
                       PushStatus = (Enums.PushStatus)entity.PushStatus,
                   };
        }
    }
}
