using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecRequestCertsView : UniqueView<Models.DecRequestCert, ScCustomsReponsitory>
    {
        public DecRequestCertsView()
        {
        }
        internal DecRequestCertsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecRequestCert> GetIQueryable()
        {
            return from requestcert in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecRequestCerts>()
                   select new Models.DecRequestCert
                   {
                       ID = requestcert.ID,
                       DeclarationID = requestcert.DeclarationID,
                       AppCertCode = requestcert.AppCertCode,
                       ApplOri = requestcert.ApplOri,
                       ApplCopyQuan = requestcert.ApplCopyQuan
                   };
        }
    }
}
