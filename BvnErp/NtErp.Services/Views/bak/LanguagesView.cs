using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Services.Models;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 语言视图
    /// </summary>
    public class LanguagesView : QueryView<Language, BvOverallsReponsitory>
    {
        public LanguagesView()
        {
        }

        protected override IQueryable<Language> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.Languages>()
                   select new Language
                   {
                       ID = entity.ID,
                       ShortName = entity.ShortName,
                       DisplayName = entity.DisplayName,
                       EnglishName = entity.EnglishName,
                       DataName = entity.DataName
                   };
        }
    }
}
