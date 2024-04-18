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
    /// 快递公司视图
    /// </summary>
    public class ExpressCompanyView : UniqueView<Models.ExpressCompany, ScCustomsReponsitory>
    {
        public ExpressCompanyView()
        {
        }

        internal ExpressCompanyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExpressCompany> GetIQueryable()
        {
            var carrierView = new CarriersView(this.Reponsitory).Where(t => t.Status == Enums.Status.Normal)
                .Where(t => t.CarrierType == Enums.CarrierType.DomesticExpress);

            return from expressCompany in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressCompanys>()
                   join carrier in carrierView on expressCompany.ID equals carrier.ID
                   select new Models.ExpressCompany
                   {
                       ID = carrier.ID,
                       Contact = carrier.Contact,
                       CarrierType = carrier.CarrierType,
                       Name = carrier.Name,
                       Code = carrier.Code,
                       Status = carrier.Status,
                       Summary = carrier.Summary,
                       CreateDate = carrier.CreateDate,
                       QueryMark=carrier.QueryMark,
                       CustomerName = expressCompany.CustomerName,
                       CustomerPwd = expressCompany.CustomerPwd,
                       MonthCode = expressCompany.MonthCode,
                   };
        }
    }
}
