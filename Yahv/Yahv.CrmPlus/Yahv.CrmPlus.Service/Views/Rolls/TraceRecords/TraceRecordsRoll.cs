using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls.TraceRecords
{
    public class TraceRecordsRoll : Origins.TraceRecordsOrigin
    {
        public TraceRecordsRoll()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TraceRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<TraceRecord> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }



}
