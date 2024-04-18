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
    public class Sz_Cfb_InViewOrigin : UniqueView<Models.InStoreViewModel, ScCustomsReponsitory>
    {
        public Sz_Cfb_InViewOrigin()
        {
        }

        internal Sz_Cfb_InViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InStoreViewModel> GetIQueryable()
        {
            var result = from outInfo in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>()
                         join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>()
                         on outInfo.AdminID equals admin.ID into admins
                         from admin in admins.DefaultIfEmpty()
                         select new Models.InStoreViewModel
                         {
                            OrderItemID = outInfo.OrderItemID,                            
                            OperatorID = outInfo.AdminID,
                            OperatorName = admin.RealName,
                            LotNumber = outInfo.LotNumber,
                            InStoreDate = outInfo.CreateDate,
                            InputID = outInfo.InputID
                         };
            return result;
        }
    }
}
