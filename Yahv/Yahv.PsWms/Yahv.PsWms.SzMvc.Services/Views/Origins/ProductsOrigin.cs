using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class ProductsOrigin : UniqueView<Models.Origin.Product, PsOrderRepository>
    {
        #region 构造函数
        public ProductsOrigin()
        {
        }

        public ProductsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Product> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Products>()
                       select new Models.Origin.Product
                       {
                           ID = entity.ID,
                           Partnumber = entity.Partnumber,
                           Brand = entity.Brand,
                           Package = entity.Package,
                           DateCode = entity.DateCode,
                           Mpq = entity.Mpq,
                           Moq = entity.Moq,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };
            return view;
        }
    }
}
