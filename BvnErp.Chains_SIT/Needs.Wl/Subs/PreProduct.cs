using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public PreProduct PreProduct
        {
            get
            {
                return new PreProduct(this);
            }
        }
    }

    public class PreProduct
    {
        IGenericAdmin Admin;

        public PreProduct(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public Ccs.Services.Views.IcgooRequestParaView RequestPara
        {
            get
            {
                return new Ccs.Services.Views.IcgooRequestParaView();
            }
        }

        public Ccs.Services.Views.ClassifyResultView ClassifyResult
        {
            get
            {
                return new Ccs.Services.Views.ClassifyResultView();
            }
        }

        public Ccs.Services.Views.PendingClassifyView PendingClassify
        {
            get
            {
                return new Ccs.Services.Views.PendingClassifyView();
            }
        }

        public Ccs.Services.Views.SMSContactView SMSContact
        {
            get
            {
                return new Ccs.Services.Views.SMSContactView();
            }
        }
    }
}
