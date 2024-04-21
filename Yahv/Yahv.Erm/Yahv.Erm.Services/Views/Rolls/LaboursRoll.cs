using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 劳资信息
    /// </summary>
    public class LaboursRoll : UniqueView<Labour, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LaboursRoll()
        {
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Labour> GetIQueryable()
        {
            var staffView = new Origins.StaffsOrigin(this.Reponsitory);
            var LaboursView = new Origins.LaboursOrigin(this.Reponsitory);
            return from labour in LaboursView
                   join staff in staffView on labour.ID equals staff.ID
                   where staff.Status != StaffStatus.Delete
                   select new Labour()
                   {
                       ID = labour.ID,
                       CreateDate = labour.CreateDate,
                       ContractPeriod = labour.ContractPeriod,
                       ContractType = labour.ContractType,
                       EnterpriseID = labour.EnterpriseID,
                       EntryCompany = labour.EntryCompany,
                       EntryDate = labour.EntryDate,
                       LeaveDate = labour.LeaveDate,
                       SigningTime = labour.SigningTime,
                       UpdateDate = labour.UpdateDate,
                       ProbationMonths = labour.ProbationMonths,
                       SocialSecurityAccount = labour.SocialSecurityAccount,
                   };
        }
    }
}
