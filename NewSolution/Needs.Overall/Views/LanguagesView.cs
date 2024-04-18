using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Overall.Models;
using System.Linq;

namespace Needs.Overall.Views
{
    /// <summary>
    /// 语言视图
    /// </summary>
    class LanguagesView : QueryView<Language, BvOverallsReponsitory>
    {
        public LanguagesView()
        {
        }

        protected override IQueryable<Language> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.Languages>()
                   select new Language
                   {
                       ShortName = entity.ShortName,
                       DisplayName = entity.DisplayName,
                       EnglishName = entity.EnglishName,
                       DataName = entity.DataName
                   };
        }
    }
}
