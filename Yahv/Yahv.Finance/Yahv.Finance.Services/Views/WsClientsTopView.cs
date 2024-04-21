using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Services.Views;

namespace Yahv.Finance.Services.Views
{
    /// <summary>
    /// 代仓储客户
    /// </summary>
    public class WsClientsTopView : UniqueView<WsClient, PvFinanceReponsitory>
    {
        public WsClientsTopView()
        {
        }

        public WsClientsTopView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            return new WsClientsTopView<PvFinanceReponsitory>(this.Reponsitory);
        }
    }
}