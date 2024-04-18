using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class WsClientsTopView : QueryView<WsClient, Layer.Data.Sqls.PvbCrmReponsitory>
    {
        public WsClientsTopView()
        {

        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbCrm.WsClientsTopView>()
                   where entity.Status== (int)ApprovalStatus.Normal
                   select new WsClient
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       RegAddress = entity.RegAddress,
                       Corporation = entity.Corporation,
                       CustomsCode = entity.CustomsCode,
                       EnterCode = entity.EnterCode,
                       Uscc = entity.Uscc,
                       Grade = (ClientGrade)entity.Grade,
                       Vip = entity.Vip,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       AdminID = entity.AdminID,
                       AdminCode = entity.AdminCode,
                       Status = (ApprovalStatus)entity.Status,
                   };
        }
    }
}
