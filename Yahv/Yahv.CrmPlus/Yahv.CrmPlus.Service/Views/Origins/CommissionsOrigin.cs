using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class CommissionsOrigin : Yahv.Linq.UniqueView<Commission, PvdCrmReponsitory>
    {
        internal CommissionsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CommissionsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Commission> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Commissions>()

                   select new Commission
                   {
                       ID = entity.ID,
                       SupplierID = entity.SupplierID,
                       CompanyID = entity.CompanyID,
                       Type = (CommissionType)entity.Type,
                       Methord = (CommissionMethod)entity.Methord,
                       Radio = entity.Ratio,
                       Msp = entity.Msp,
                       Currency = (Currency)entity.Currency,
                       CreatorID = entity.CreatorID,
                       Status = (DataStatus)entity.Status,
                       Months = entity.Months,
                       Days = entity.Days
                   };
        }
    }
}
