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
    public class MessageView : UniqueView<Models.IcgooMQ, ScCustomsReponsitory>
    {
        public MessageView()
        {
        }

        internal MessageView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooMQ> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooPostLog>()
                   select new Models.IcgooMQ
                   {
                       ID = para.ID,
                       PostData = para.PostData,
                       IsAnalyzed = para.IsAnalyzed,
                       Status = (Status)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       Summary = para.Summary,
                       CompanyType = (CompanyTypeEnums)para.CompanyType,
                       AdditionWeight = para.PlateQty,
                       IsForklift = para.IsForklift,
                   };
        }
    }
}
