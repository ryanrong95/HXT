using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls.TraceRecords
{

    public class TraceCommentsRoll : Origins.TraceCommentsOrigin
    {
        public TraceCommentsRoll()
        {

        }

        protected override IQueryable<TraceComment> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }

    /// <summary>
    /// 我的点评记录
    /// </summary>
    public class MyTraceCommentsRoll : Origins.TraceCommentsOrigin
    {
        public MyTraceCommentsRoll()
        {

        }

        IErpAdmin admin;
        public MyTraceCommentsRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<TraceComment> GetIQueryable()
        {
            if (admin.IsSuper)
            {
                return base.GetIQueryable();
            }

            //return from record in new TraceRecordsRoll(this.Reponsitory)
            //       join comment in base.GetIQueryable() on record.ID equals comment.TraceRecordID
            //       where comment.AdminID == this.admin.ID
            //       select comment;

            return from comment in base.GetIQueryable()  // on record.ID equals comment.TraceRecordID
                   where comment.AdminID == this.admin.ID
                   select comment;

            //var tranceIDs = new TraceRecordsRoll().Where(x => x.OwnerID == Admin.ID).Select(x => x.ID).Distinct().ToArray();

            //return base.GetIQueryable()

            //    .Where(x => tranceIDs.Contains(x.TraceRecordID) && x.IsPointed == true);
        }

    }

}
