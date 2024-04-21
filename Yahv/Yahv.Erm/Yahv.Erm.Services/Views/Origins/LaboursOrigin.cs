using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 劳资关系源视图
    /// </summary>
    internal class LaboursOrigin : UniqueView<Labour, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal LaboursOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal LaboursOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Labour> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Labours>()
                   select new Labour()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       ContractPeriod = entity.ContractPeriod,
                       ContractType = entity.ContractType,
                       EnterpriseID = entity.EnterpriseID,
                       EntryCompany = entity.EntryCompany,
                       EntryDate = entity.EntryDate,
                       LeaveDate = entity.LeaveDate,
                       SigningTime = entity.SigningTime,
                       UpdateDate = entity.UpdateDate,
                       ProbationMonths = entity.ProbationMonths,
                       SocialSecurityAccount = entity.SocialSecurityAccount,
                   };
        }
    }
}
