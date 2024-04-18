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
    /// 快递信息View
    /// </summary>
    public class ExpressageView : UniqueView<Models.Expressage, ScCustomsReponsitory>
    {
        public ExpressageView()
        {
        }

        internal ExpressageView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Expressage> GetIQueryable()
        {
            var expressCompanyView = new ExpressCompanyView(this.Reponsitory);
            var expressTypeView = new ExpressTypeView(this.Reponsitory);

            return from expressage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Expressages>()
                   join expressCompany in expressCompanyView on expressage.ExpressCompanyID equals expressCompany.ID
                   join expressType in expressTypeView on expressage.ExpressTypeID equals expressType.ID
                   where expressage.Status == (int)Enums.Status.Normal
                   select new Models.Expressage
                   {
                       ID = expressage.ID,
                       Contact = expressage.Contact,
                       Mobile = expressage.Mobile,
                       Address = expressage.Address,
                       ExpressCompany = expressCompany,
                       ExpressType = expressType,
                       PayType = (Enums.PayType)expressage.PayType,
                       WaybillCode = expressage.WaybillCode,
                       QueryMark= expressCompany.QueryMark,
                       //HtmlTemplate = expressage.HtmlTemplate,
                       Status = (Enums.Status)expressCompany.Status,
                       CreateDate = expressCompany.CreateDate,
                       UpdateDate = expressCompany.UpdateDate,
                   };
        }
    }
}
