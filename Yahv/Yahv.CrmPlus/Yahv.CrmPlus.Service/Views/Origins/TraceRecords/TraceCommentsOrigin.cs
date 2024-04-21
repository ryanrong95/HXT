using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class TraceCommentsOrigin : Yahv.Linq.UniqueView<TraceComment, PvdCrmReponsitory>
    {

        internal TraceCommentsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TraceCommentsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TraceComment> GetIQueryable()
        {
            var enterpriseView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var traceRecordView = new TraceRecordsOrigin(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.TraceComments>()
                   join traceRecord in traceRecordView on entity.TraceRecordID equals traceRecord.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new TraceComment
                   {
                       ID = entity.ID,
                       TraceRecordID = entity.TraceRecordID,
                       TraceRecord = traceRecord,
                       Admin = admin,
                       AdminID = admin.ID,
                       Comments = entity.Comments,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       IsPointed=entity.IsPointed

                   };
        }
    }
}
