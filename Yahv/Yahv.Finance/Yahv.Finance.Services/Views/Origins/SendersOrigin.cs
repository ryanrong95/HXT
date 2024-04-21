using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 接口发起人 原始视图
    /// </summary>
    public class SendersOrigin : UniqueView<Sender, PvFinanceReponsitory>
    {
        public SendersOrigin()
        {
        }

        public SendersOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Sender> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Senders>()
                   select new Sender()
                   {
                       Status = (GeneralStatus)entity.Status,
                       ID = entity.ID,
                       Name = entity.Name,
                       SecretKey = entity.SecretKey,
                   };
        }
    }
}