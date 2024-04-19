using NtErp.Wss.Oss.Services;
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
    /// 当事人视图
    /// </summary>
    public class PartiesView : PartyBaseView, Needs.Overall.IFkoView<Models.Party>
    {
        internal PartiesView()
        {

        }
        internal PartiesView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<Models.Party> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }
}
