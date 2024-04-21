using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 运单视图
    /// </summary>
    public class WaybillsTopView : QueryView<Waybill, PvbCrmReponsitory>
    {
        public WaybillsTopView()
        {

        }

        public WaybillsTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Waybill> GetIQueryable()
        {
            return new Yahv.Services.Views.WaybillsTopView<PvbCrmReponsitory>();
        }

        public Waybill this[string waybillID]
        {
            get { return this.SingleOrDefault(item => item.ID == waybillID); }
        }
    }
}
