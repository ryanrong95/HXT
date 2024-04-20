using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;


namespace NtErp.Wss.Services.Models
{
    public class ClientTop : Needs.Linq.IUnique
    {
        public string ID { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public string Mobile { set; get; }
        public DateTime CreateDate { get; set; }
        public UserStatus Status { get; set; }
    }
}
