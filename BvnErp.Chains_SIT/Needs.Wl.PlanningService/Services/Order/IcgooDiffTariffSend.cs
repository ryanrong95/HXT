using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class IcgooDiffTariffSend
    {
        public List<string> OrderIDs;
        public string MQName { get; set; }
        public IcgooDiffTariffSend(List<string> OrderIDs,string mqName)
        {
            this.OrderIDs = OrderIDs;
            this.MQName = mqName;
        }

        public void Send()
        {
            string Orders = "";
            foreach(var item in OrderIDs)
            {
                Orders += item + ",";
            }
            //去除最后一个逗号
            Orders = Orders.Substring(0, Orders.Length - 1);          

            var publisher = new RabbitMQ(MQName, MQName);
            publisher.Publish(Orders);
        }
    }
}
