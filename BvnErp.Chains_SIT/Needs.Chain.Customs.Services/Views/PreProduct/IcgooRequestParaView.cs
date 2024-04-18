using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class IcgooRequestParaView : UniqueView<Models.IcgooRequestPara, ScCustomsReponsitory>
    {
        public IcgooRequestParaView()
        {
        }

        internal IcgooRequestParaView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooRequestPara> GetIQueryable()
        {
            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooRequestPara>()
                   where para.IsUse == true
                   select new Models.IcgooRequestPara
                   {
                       ID = para.id.ToString(),
                       Supplier = para.Supplier,
                       days = para.days.Value,
                       RequestUrl = para.RequestUrl,
                       PageSize = para.PageSize.Value,
                       ClientID = para.ClientID
                   };
        }
    }
}
