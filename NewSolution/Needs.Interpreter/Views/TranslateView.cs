using Layer.Data.Sqls;
using Needs.Interpreter.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Views
{
    public class TranslateView : UniqueView<Translate, BvTesterReponsitory>
    {
        internal TranslateView()
        {

        }

        protected override IQueryable<Translate> GetIQueryable()
        {
            return from entiy in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvTester.TopObjects>()
                   select new Translate
                   {
                       ID = entiy.ID,
                       Name = entiy.Name,
                       Language = entiy.Language,
                       Type = entiy.Type,
                       Value = entiy.Value
                   };
        }
    }
}
