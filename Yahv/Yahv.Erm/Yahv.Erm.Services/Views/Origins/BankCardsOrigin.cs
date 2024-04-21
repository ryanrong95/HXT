using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 银行卡视图
    /// </summary>
    internal class BankCardsOrigin : UniqueView<BankCard, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal BankCardsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal BankCardsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<BankCard> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<BankCards>()
                   select new BankCard()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       BankAddress = entity.BankAddress,
                       SwiftCode = entity.SwiftCode,
                       Bank = entity.Bank,
                       Account = entity.Account,
                       //StaffID = entity.StaffID,
                   };
        }
    }
}