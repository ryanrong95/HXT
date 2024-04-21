using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Wms.Services.Views
{
    public class ClientsView : QueryView<WsClient, PvWmsRepository>
    {
        protected override IQueryable<WsClient> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WsClientsTopView>()
                   select new WsClient
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Corporation = entity.Corporation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Status = (ApprovalStatus)entity.Status,
                       EnterCode = entity.EnterCode,
                       AdminID = entity.AdminID,
                       CreateDate = entity.CreateDate,
                       AdminCode = entity.AdminCode,
                       CustomsCode = entity.CustomsCode,
                       Grade = (ClientGrade)entity.Grade,
                       UpdateDate = entity.UpdateDate,
                       Vip = entity.Vip,
                   };
        }
    }
}
