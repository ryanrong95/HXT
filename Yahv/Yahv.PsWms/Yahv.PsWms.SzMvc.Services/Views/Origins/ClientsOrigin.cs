using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class ClientsOrigin : UniqueView<Models.Origin.Client, PsOrderRepository>
    {
        #region 构造函数
        public ClientsOrigin()
        {
        }

        public ClientsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Client> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Clients>()
                       select new Models.Origin.Client
                       {
                           ID = entity.ID,
                           SiteuserID = entity.SiteuserID,
                           Name = entity.Name,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };
            return view;
        }
    }
}
