using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    /// <summary>
    ///快递方式View
    /// </summary>
    public class ExpressTypeView : UniqueView<Models.ExpressType, ScCustomsReponsitory>
    {
        public ExpressTypeView()
        {
        }

        internal ExpressTypeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExpressType> GetIQueryable()
        {
            var expressCompanyView = new ExpressCompanyView(this.Reponsitory);

            return from expressType in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressTypes>()
                   join expressCompany in expressCompanyView on expressType.ExpressCompanyID equals expressCompany.ID
                   select new Models.ExpressType
                   {
                       ID = expressType.ID,
                       ExpressCompany = expressCompany,
                       ExpressCompanyID= expressType.ExpressCompanyID,
                       TypeName = expressType.TypeName,
                       TypeValue = expressType.TypeValue,
                       Status= (Enums.Status)expressType.Status,
                   };
        }
    }
}
