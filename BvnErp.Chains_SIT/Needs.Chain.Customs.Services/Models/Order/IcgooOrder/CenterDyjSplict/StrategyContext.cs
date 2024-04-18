using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StrategyContext
    {
        public BaseStrategy strategy { get; set; }

        public StrategyContext(string agentCompany, List<InsideOrderItem> Models)
        {
            switch (agentCompany)
            {
                case InnerSaleAgentCompanies.kexunSeries:
                case InnerSaleAgentCompanies.yuandaSeries:
                    this.strategy = new BoxStrategy(Models);
                    break;

                default:
                    this.strategy = new ItemStrategy(Models);
                    break;
            }
        }

        public List<OrderModel> SplitOrder()
        {
            return this.strategy.SplitOrder();
        }
    }
}
