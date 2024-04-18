using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class SiteusersOrigin : UniqueView<Models.Origin.Siteuser, PsOrderRepository>
    {
        #region 构造函数
        public SiteusersOrigin()
        {
        }

        public SiteusersOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Siteuser> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Siteusers>()
                       select new Models.Origin.Siteuser
                       {
                           ID = entity.ID,
                           Username = entity.Username,
                           Password = entity.Password,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           LoginDate = entity.LoginDate
                       };
            return view;
        }
    }
}
