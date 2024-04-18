using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    public class ApplicationItemsAll : UniqueView<ApplicationItem, PvWsOrderReponsitory>
    {
        public ApplicationItemsAll()
        {

        }

        public ApplicationItemsAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApplicationItem> GetIQueryable()
        {
            var itemView = new ApplicationItemsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal);

            var applies = from entity in itemView
                          group entity by entity.OrderID into entities
                          select new
                          {
                              OrderID = entities.Key,
                              AppliedPrice = entities.Sum(t => t.Amount)
                          };

            var linq = from entity in itemView
                       join apply in applies on entity.OrderID equals apply.OrderID
                       select new ApplicationItem()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           OrderID = entity.OrderID,
                           Amount = entity.Amount,
                           Status = entity.Status,

                           AppliedPrice = apply.AppliedPrice,
                       };
            return linq;
        }

        public IQueryable<ApplicationItem> GetPaymentIQueryable()
        {
            var applys = new ApplicationsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal && item.Type == Enums.ApplicationType.Payment);
            var items = new ApplicationItemsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal);

            var itemView = from entity in items
                           join apply in applys on entity.ApplicationID equals apply.ID
                           select entity;

            var applies = from entity in itemView
                          group entity by entity.OrderID into entities
                          select new
                          {
                              OrderID = entities.Key,
                              AppliedPrice = entities.Sum(t => t.Amount)
                          };

            var linq = from entity in itemView
                       join apply in applies on entity.OrderID equals apply.OrderID
                       select new ApplicationItem()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           OrderID = entity.OrderID,
                           Amount = entity.Amount,
                           Status = entity.Status,

                           AppliedPrice = apply.AppliedPrice,
                       };
            return linq;
        }

        public IQueryable<ApplicationItem> GetReceiveIQueryable()
        {
            var applys = new ApplicationsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal && item.Type == Enums.ApplicationType.Receival);
            var items = new ApplicationItemsOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal);

            var itemView = from entity in items
                           join apply in applys on entity.ApplicationID equals apply.ID
                           select entity;

            var applies = from entity in itemView
                          group entity by entity.OrderID into entities
                          select new
                          {
                              OrderID = entities.Key,
                              AppliedPrice = entities.Sum(t => t.Amount)
                          };

            var linq = from entity in itemView
                       join apply in applies on entity.OrderID equals apply.OrderID
                       select new ApplicationItem()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           OrderID = entity.OrderID,
                           Amount = entity.Amount,
                           Status = entity.Status,

                           AppliedPrice = apply.AppliedPrice,
                       };
            return linq;
        }
    }
}
