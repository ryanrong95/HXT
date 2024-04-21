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
    /// 企业通用视图
    /// </summary>
    public class EnterprisesTopView : UniqueView<Enterprise, PvbCrmReponsitory>
    {
        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>();
        }

        public Enterprise this[string id]
        {
            get { return this.SingleOrDefault(item => item.ID == id); }
        }
    }
}
