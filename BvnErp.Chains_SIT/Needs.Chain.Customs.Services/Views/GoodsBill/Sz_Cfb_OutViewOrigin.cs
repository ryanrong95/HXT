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
    public class Sz_Cfb_OutViewOrigin : UniqueView<Models.OutStoreViewModel, ScCustomsReponsitory>
    {
        public Sz_Cfb_OutViewOrigin()
        {
        }

        internal Sz_Cfb_OutViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OutStoreViewModel> GetIQueryable()
        {
            var result = from outInfo in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>()
                         join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>()
                         on outInfo.AdminID equals admin.ID into admins
                         from admin in admins.DefaultIfEmpty()
                         select new Models.OutStoreViewModel
                         {
                            OrderItemID = outInfo.OrderItemID,
                            OutQty = outInfo.Quantity,
                            OutStoreDate = outInfo.CreateDate,
                            OperatorID = outInfo.AdminID,
                            OperatorName = admin.RealName
                         };
            return result;
        }
    }
}
