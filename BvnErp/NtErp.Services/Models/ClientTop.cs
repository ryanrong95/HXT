using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 视图 Model
    /// </summary>
    [Needs.Underly.FactoryView(typeof(Views.MyClientsTopView<Layer.Data.Sqls.BvnErpReponsitory>))]
    public class ClientTop : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        //public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        //public DateTime UpdateDate { get; set; }
        public Needs.Erp.Generic.Status Status { get; set; }

        public ClientTop()
        {

        }

    }
}
