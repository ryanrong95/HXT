using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    public partial class ClientTop : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        internal ClientTop()
        {

        }



        public NtErp.Wss.Oss.Services.Models.ClientTop ToOss()
        {
            return new NtErp.Wss.Oss.Services.Models.ClientTop
            {
                ID = this.ID,
                UserName = this.UserName,
                Email = this.Email,
                Mobile = this.Mobile,
            };
        }

    }
}
