using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 银行卡
    /// </summary>
    public class BankCardsRoll : UniqueView<BankCard, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BankCardsRoll()
        {
        }

        protected override IQueryable<BankCard> GetIQueryable()
        {
            var staffView = new Origins.StaffsOrigin(this.Reponsitory);
            var BankCardsView= new Origins.BankCardsOrigin(this.Reponsitory);
            return from bankCard in BankCardsView join 
                   staff in staffView on bankCard.ID equals staff.ID
                   where staff.Status != StaffStatus.Delete
                   select new BankCard()
                   {
                       ID = bankCard.ID,
                       CreateDate = bankCard.CreateDate,
                       BankAddress = bankCard.BankAddress,
                       SwiftCode = bankCard.SwiftCode,
                       Bank = bankCard.Bank,
                       Account = bankCard.Account,
                   };
        }
    }
}