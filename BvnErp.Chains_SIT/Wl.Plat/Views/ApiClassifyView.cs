using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class ApiClassifyView : View<Needs.Wl.Client.Services.PageModels.ApiClassifyProduct, ScCustomsReponsitory>
    {
        protected override IQueryable<Needs.Wl.Client.Services.PageModels.ApiClassifyProduct> GetIQueryable()
        {
            var query = from pro in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategories>()
                        join traffic in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>() on pro.HSCode equals traffic.HSCode
                        join unit1 in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>() on traffic.Unit1 equals unit1.Code into unit1s
                        from _unit1 in unit1s.DefaultIfEmpty()
                        join unit2 in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>() on traffic.Unit2 equals unit2.Code into unit2s
                        from _unit2 in unit2s.DefaultIfEmpty()
                        select new Needs.Wl.Client.Services.PageModels.ApiClassifyProduct
                        {
                            ID = pro.ID,
                            HSCode = pro.HSCode,
                            Name = pro.Name,
                            Model = pro.Model,
                            Unit1 = _unit1 == null ? "" : _unit1.Name,
                            Unit2 = _unit2 == null ? "" : _unit2.Name,
                            RegulatoryCode = traffic.RegulatoryCode,
                            CIQCode = traffic.CIQCode,
                            MFN = traffic.MFN,
                            General = traffic.General,
                            AddedValue = traffic.AddedValue,
                            Consume = traffic.Consume,
                            Elements = traffic.Elements
                        };
            return query.Take(5);
        }
    }
}