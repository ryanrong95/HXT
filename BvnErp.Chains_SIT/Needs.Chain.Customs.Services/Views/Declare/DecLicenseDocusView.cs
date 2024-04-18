using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecLicenseDocusView : UniqueView<Models.DecLicenseDocu, ScCustomsReponsitory>
    {
        public DecLicenseDocusView()
        {
        }
        internal DecLicenseDocusView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecLicenseDocu> GetIQueryable()
        {
            var DocuCode = new BaseDocuCodesView(this.Reponsitory);

            return from license in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>()
                   join decDocu in DocuCode on license.DocuCode equals decDocu.Code
                   select new Models.DecLicenseDocu
                   {
                       ID = license.ID,
                       DeclarationID = license.DeclarationID,
                       DocuCode = license.DocuCode,
                       CertCode = license.CertCode,
                       DocuCodeCertify = decDocu,
                       FileUrl = license.FileURL
                   };
        }
    }
}
