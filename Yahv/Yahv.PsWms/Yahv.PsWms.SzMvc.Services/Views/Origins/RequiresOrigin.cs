using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class RequiresOrigin : UniqueView<Models.Origin.Require, PsOrderRepository>
    {
        #region 构造函数
        public RequiresOrigin()
        {
        }

        public RequiresOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Require> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Requires>()
                       select new Models.Origin.Require
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderTransportID = entity.OrderTransportID,
                           Name = entity.Name,
                           Content = entity.Content,
                           CreateDate = entity.CreateDate,
                       };
            return view;
        }
    }
}
