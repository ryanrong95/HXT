using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 视图 Model
    /// </summary>

    [Needs.Underly.FactoryView(typeof(Views.ClientsTopView))]
    public partial class ClientTop : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        //[Obsolete("准备废弃")]
        //public Accounts Accounts
        //{
        //    get
        //    {
        //        return new Accounts(this);
        //    }
        //}

        public ClientTop()
        {

        }

    }
}
