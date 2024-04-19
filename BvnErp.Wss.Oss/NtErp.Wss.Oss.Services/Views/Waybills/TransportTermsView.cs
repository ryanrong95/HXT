using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 运输条款视图
    /// </summary>
    public class TransportTermsView : UniqueView<Models.TransportTerm, CvOssReponsitory>
    {
        internal TransportTermsView()
        {

        }
        internal TransportTermsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TransportTerm> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.TransportTerms>()
                   select new Models.TransportTerm
                   {
                       ID = entity.ID,
                       Carrier = entity.Carrier,
                       FreightMode = (FreightMode)entity.FreightMode,
                       PriceClause = (PriceClause)entity.PriceClause,
                       TransportMode = (TransportMode)entity.TransportMode,
                       Address = entity.Address
                   };
        }
    }
}
