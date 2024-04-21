using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 月账单视图
    /// </summary>
    internal class PayBillsOrigin : UniqueView<PayBill, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PayBillsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PayBillsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PayBill> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<PayBills>()
                   select new PayBill()
                   {
                       ID = entity.ID,
                       Status = (PayBillStatus)entity.Status,
                       DateIndex = entity.DateIndex,
                       StaffID = entity.StaffID,
                       ClosedData = entity.ClosedData,
                       CreaetDate = entity.CreaetDate,
                       UpdateDate = entity.UpdateDate,
                       EnterpriseID = entity.EnterpriseID,
                   };
        }
    }
}