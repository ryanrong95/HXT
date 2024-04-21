using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 接口发起人视图
    /// </summary>
    public class SendersRoll : UniqueView<Sender, PvFinanceReponsitory>
    {
        public SendersRoll()
        {

        }

        public SendersRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Sender> GetIQueryable()
        {
            return new SendersOrigin(this.Reponsitory).Where(item => item.Status == GeneralStatus.Normal);
        }
    }
}