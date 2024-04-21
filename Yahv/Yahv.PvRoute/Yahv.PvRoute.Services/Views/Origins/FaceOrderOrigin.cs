using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Enums;
using Yahv.PvRoute.Services.Models;

namespace Yahv.PvRoute.Services.Views.Origins
{
    public class FaceOrderOrigin : UniqueView<FaceOrder, PvRouteReponsitory>
    {
        public FaceOrderOrigin()
        {

        }

        public FaceOrderOrigin(PvRouteReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FaceOrder> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.FaceOrders>()
                   select new Models.FaceOrder
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Source = (PrintSource)entity.Source,
                       CreatorID = entity.CreatorID,
                       Html = entity.Html,
                       SendJson = entity.SendJson,
                       ReceiveJson = entity.ReceiveJson,
                       SessionID = entity.SessionID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (PrintState)entity.Status,
                   };
        }
    }
}
