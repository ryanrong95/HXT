using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class nSuppliersOrigin : Yahv.Linq.UniqueView<nSupplier, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal nSuppliersOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nSuppliersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nSupplier> GetIQueryable()
        {
            var enterprises = new EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nSuppliers>()
                   join ep1 in enterprises on entity.EnterpriseID equals ep1.ID
                   join reals in enterprises on entity.RealID equals reals.ID into realenterprise
                   from ep2 in realenterprise.DefaultIfEmpty()
                   select new nSupplier()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       RealID = entity.RealID,
                       Enterprise = ep1,
                       RealEnterprise = ep2,
                       FromID = entity.FromID,
                       Conduct = (Business)entity.Conduct,
                       Grade = (SupplierGrade)entity.Grade,
                       CHNabbreviation = entity.CHNabbreviation,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (GeneralStatus)entity.Status,
                       Creator = entity.Creator
                   };

        }
    }
}
