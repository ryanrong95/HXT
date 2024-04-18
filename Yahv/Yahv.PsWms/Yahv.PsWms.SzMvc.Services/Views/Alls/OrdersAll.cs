using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Alls
{

    public class OrdersAll : UniqueView<Models.Origin.Order, PsOrderRepository>
    {
        #region 构造函数
        public OrdersAll()
        {
        }

        public OrdersAll(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Order> GetIQueryable()
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory);
            var clients = new Origins.ClientsOrigin(this.Reponsitory);

            var view = from entity in orders
                       select new Models.Origin.Order
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           SiteuserID = entity.SiteuserID,
                           CompanyID = entity.CompanyID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           PackageCount = entity.PackageCount,
                           Weight = entity.Weight,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                       };
            return view;
        }
    }
}
