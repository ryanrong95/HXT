using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Generic
{
    public class ErpWebsite
    {
        IGenericAdmin admin;

        public ErpWebsite(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        public Services.Generic.Views.ClientsTopView MyClients
        {
            get
            {
                return new Services.Generic.Views.ClientsTopView(this.admin);
            }
        }


        //全部客户 （ClientsTopAlls）
        //全部客户 + unbind  binding
        public Services.Generic.Views.ClientsTopAlls ClientAll
        {
            get
            {
                return new Services.Generic.Views.ClientsTopAlls();
            }
        }
        public Oss.Services.Views.MyOrdersView MyOrders
        {
            get
            {
                return new Oss.Services.Views.MyOrdersView(this.admin);
            }
        }
    }
}



