using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 核销记录 视图
    /// </summary>
    public class PayeeRightsView : QueryView<PayeeRightQuery, PvFinanceReponsitory>
    {
        public PayeeRightsView()
        {
        }

        public PayeeRightsView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeRightQuery> GetIQueryable()
        {
            var lefts = new PayeeLeftsOrigin(this.Reponsitory);
            var rights = new PayeeRightsOrigin(this.Reponsitory);
            var catalogs = new AccountCatalogsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);
            var senders = new SendersOrigin(this.Reponsitory);

            return from entity in rights
                   join left in lefts on entity.PayeeLeftID equals left.ID
                   join catalog in catalogs on entity.AccountCatalogID equals catalog.ID
                   join admin in admins on entity.CreatorID equals admin.ID
                   join sender in senders on entity.SenderID equals sender.ID
                   select new PayeeRightQuery()
                   {
                       Currency = entity.Currency,
                       CreateDate = entity.CreateDate,
                       RightPrice = entity.Price,
                       LeftPrice = left.Price,
                       ID = entity.ID,
                       PayeeLeftID = entity.PayeeLeftID,
                       SenderName = sender.Name,
                       AccountCatalogName = catalog.Name,
                       CreatorName = admin.RealName,
                       PayerName = left.PayerName,
                   };
        }
    }
}