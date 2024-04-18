using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Order:IUnique
    {

        public string ID { get; set; }

        public OrderType Type { get; set; }

        public string ClientID { get; set; }

        public string SiteuserID { get; set; }

        public string CompanyID { get; set; }

        public string ConsigneeID { get; set; }

        public string ConsignorID { get; set; }

        public int? PackageCount { get; set; }

        public decimal? Weight { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string Summary { get; set; }


        public void ChangeOrderStatus(string orderID, OrderStatus orderStatus)
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Update<Layers.Data.Sqls.PsOrder.Orders>(new
                {
                    Status = (int)orderStatus,
                }, item => item.ID == orderID);
            }
        }
    }
}
