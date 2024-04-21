using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service
{
    static public class ApprovalRecords
    {
        #region 供应商
        /// <summary>
        //供应商注册审批记录
        /// </summary>
        /// <returns></returns>
        public static IQueryable<SupplierRegisteRecord> SupplierRegiste()
        {
            return new Views.Rolls.SupplierRegisteRecordsRoll();
        }
        /// <summary>
        /// 供应商关联关系审批记录
        /// </summary>
        /// <returns></returns>
        public static IQueryable<BusinessRelationsRecord> SupplierRelations()
        {
            return new Views.Rolls.SupplierRelationsRecordsRoll();
        }
        /// <summary>
        /// 供应商保护已审批记录
        /// </summary>
        /// <returns></returns>
        public static IQueryable<ProtectedRecord> SupplierProtecteds()
        {
            return new Views.Rolls.SupplierProtectedRecordsRoll().Where(item=> item.Status != Underly.ApplyStatus.Waiting);
        }
        /// <summary>
        /// 供应商保护申请记录
        /// </summary>
        /// <returns></returns>
        public static IQueryable<ProtectedRecord> MySupplierProtecteds(IErpAdmin admin, string supplierid)
        {
            return new Views.Rolls.SupplierProtectedRecordsRoll().Where(item => item.ApplyAdmin.ID == admin.ID && item.MainID == supplierid);
        }
        /// <summary>
        /// 供应商特色审批记录
        /// </summary>
        /// <returns></returns>
        public static IQueryable<SupplierSpecialRecord> SupplierSpecials()
        {
            return new Views.Rolls.SupplierSpecialRecordsRoll();
        }

        #endregion

    }
}
