using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class WxTokenView : UniqueView<Models.WxToken, ScCustomsReponsitory>
    {
        public WxTokenView()
        {

        }

        internal WxTokenView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<WxToken> GetIQueryable()
        {
            return from wxToken in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.WxTokens>()
                   select new Models.WxToken
                   {
                       ID = wxToken.ID,
                       Type = wxToken.Type,
                       Value = wxToken.Value,
                       CreateTime = wxToken.CreateTime,
                   };
        }
    }
}
