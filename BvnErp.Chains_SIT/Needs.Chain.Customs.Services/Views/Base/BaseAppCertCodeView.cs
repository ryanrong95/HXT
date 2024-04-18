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
    public class BaseAppCertCodeView : UniqueView<Models.BaseAppCertCode, ScCustomsReponsitory>
    {
        public BaseAppCertCodeView()
        {
        }

        internal BaseAppCertCodeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseAppCertCode> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseAppCertCode>()
                   select new Models.BaseAppCertCode
                   {
                       ID = cert.ID,
                       Code = cert.Code,
                       Name = cert.Name,
                   };
        }
    }
}