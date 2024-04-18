using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class OrdersOrigin : UniqueView<Models.Origin.Order, PsOrderRepository>
    {
        #region 构造函数
        public OrdersOrigin()
        {
        }

        public OrdersOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Order> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Orders>()
                       select new Models.Origin.Order
                       {
                           ID = entity.ID,
                           Type = (OrderType)entity.Type,
                           ClientID = entity.ClientID,
                           SiteuserID = entity.SiteuserID,
                           CompanyID = entity.CompanyID,
                           ConsigneeID = entity.ConsigneeID,
                           ConsignorID = entity.ConsignorID,
                           PackageCount = entity.PackageCount,
                           Weight = entity.Weight,
                           Status = (OrderStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                       };
            return view;
        }
    }
}
