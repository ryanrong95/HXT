using System;

namespace NtErp.Wss.Services.Generic.Models
{
    public partial class ClientTop : NtErp.Services.Models.ClientTop, Needs.Linq.IUnique
    {
        //public string ID { set; get; }
        //public string UserName { set; get; }
        //public string Email { set; get; }
        //public string Mobile { set; get; }

        public ClientTop()
        {

        }
    }
}
