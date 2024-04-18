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
    /// 暂存View
    /// </summary>
    public class TemporaryView : UniqueView<Models.Temporary, ScCustomsReponsitory>
    {
        public TemporaryView()
        {
        }

        internal TemporaryView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Temporary> GetIQueryable()
        {
            var adminView = new Views.AdminsTopView(this.Reponsitory);

            return from temporary in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Temporarys>()
                   join admin in adminView on temporary.AdminID equals admin.ID
                   where temporary.Status == (int)Enums.Status.Normal
                   select new Models.Temporary
                   {
                       ID = temporary.ID,
                       Admin = admin,
                       EntryNumber = temporary.EntryNumber,
                       CompanyName = temporary.CompanyName,
                       ShelveNumber = temporary.ShelveNumber,
                       WaybillCode = temporary.WaybillCode,
                       EntryDate = temporary.EntryDate,
                       PackNo = temporary.PackNo,
                       WrapType = (Enums.BaseWrapType)temporary.WrapType,
                       TemporaryStatus = (Enums.TemporaryStatus)temporary.TemporaryStatus,
                       Status = (Enums.Status)temporary.Status,
                       CreateDate = temporary.CreateDate,
                       UpdateDate = temporary.UpdateDate,
                       Summary = temporary.Summary,
                   };
        }
    }
}
