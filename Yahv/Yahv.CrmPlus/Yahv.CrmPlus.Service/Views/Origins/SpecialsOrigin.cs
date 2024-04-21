using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class SpecialsOrigin : Linq.UniqueView<Special, PvdCrmReponsitory>
    {
        internal SpecialsOrigin()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal SpecialsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Special> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Specials>()
                   join enteprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                   on entity.EnterpriseID equals enteprise.ID

                   //创建人
                   join creat_admin in adminsView on entity.CreatorID equals creat_admin.ID into creatAdmin
                   from creator in creatAdmin.DefaultIfEmpty()

                   where entity.Status != (int)Underly.AuditStatus.Closed
                   select new Special
                   {
                       ID = entity.ID,
                       SupplierName = enteprise.Name,
                       EnterpriseID = entity.EnterpriseID, 
                       Type = (Underly.nBrandType)entity.Type,
                       Status = (Underly.AuditStatus)entity.Status,
                       Brand = entity.Brand,
                       PartNumber = entity.PartNumber,
                       Summary = entity.Summary,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       CreatorName = creator.RealName
                   };
        }
    }
}
