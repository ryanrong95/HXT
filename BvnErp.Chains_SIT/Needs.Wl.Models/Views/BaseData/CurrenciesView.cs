using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class CurrenciesView : View<Models.Currency, ScCustomsReponsitory>
    {
        /// <summary>
        /// 默认不分页查询
        /// </summary>
        public CurrenciesView()
        {
            this.AllowPaging = false;
        }

        internal CurrenciesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Currency> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCurrencies>()
                   select new Models.Currency
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       EnglishName = entity.EnglishName
                   };
        }

        public Models.Currency FindByCode(string code)
        {
            return this.GetIQueryable().Where(s => s.Code == code).FirstOrDefault();
        }
    }
}