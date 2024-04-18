using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class StrategyContext
    {
        public BaseStrategy strategy { get; set; }

        public StrategyContext(string agentCompany, List<InsideOrderItem> Models)
        {
            switch (agentCompany)
            {
                case Needs.Wl.PlanningService.Services.AgentCompanies.kexunSeries:
                case Needs.Wl.PlanningService.Services.AgentCompanies.yuandaSeries:
                case Needs.Wl.PlanningService.Services.AgentCompanies.SZZDSeries:
                case Needs.Wl.PlanningService.Services.AgentCompanies.SZB1BSeries:
                case Needs.Wl.PlanningService.Services.AgentCompanies.SHBYSeries:
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
