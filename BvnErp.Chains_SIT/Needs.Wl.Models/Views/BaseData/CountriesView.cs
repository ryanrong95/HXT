using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 国家地区
    /// 默认不分页
    /// </summary>
    public class CountriesView : View<Models.Country, ScCustomsReponsitory>
    {
        public CountriesView()
        {
            this.AllowPaging = false;
        }

        internal CountriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Country> GetIQueryable()
        {
            return from country in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>()
                   orderby country.Code 
                   select new Models.Country
                   {
                       ID = country.ID,
                       Code = country.Code,
                       Name = country.Name,
                       EnglishName = country.EnglishName
                   };
        }

        public Country FindByCode(string code)
        {
            return this.GetIQueryable().Where(s => s.Code == code).FirstOrDefault();
        }
    }
}