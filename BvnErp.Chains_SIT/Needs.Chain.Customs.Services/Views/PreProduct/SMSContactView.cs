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
    public class SMSContactView : UniqueView<Models.SMSContact, ScCustomsReponsitory>
    {
        public SMSContactView()
        {
        }

        internal SMSContactView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SMSContact> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SMSContact>()
                   select new Models.SMSContact
                   {
                       ID = para.ID,
                       Name = para.Name,
                       Phone = para.Phone,
                       Status = (Status)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       Summary = para.Summary,
                   };
        }
    }
}
