
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvdCrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class BusinessRelationsOrigin : Yahv.Linq.UniqueView<BusinessRelation, PvdCrmReponsitory>
    {

        internal BusinessRelationsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal BusinessRelationsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<BusinessRelation> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>()
                   join e1 in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on entity.MainID equals e1.ID
                   join e2 in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on entity.SubID equals e2.ID

                   //创建人
                   join creat_admin in adminsView on entity.CreatorID equals creat_admin.ID into creatAdmin
                   from creator in creatAdmin.DefaultIfEmpty()

                   select new BusinessRelation
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       MainName = e1.Name,
                       SubName = e2.Name,
                       SubID = entity.SubID,
                       Status = (AuditStatus)entity.Status,
                       BusinessRelationType = (BusinessRelationType)entity.Type,
                       CreateDate=entity.CreateDate,
                       CreatorID=entity.CreatorID,
                       CreatorName= creator.RealName

                   };
        }
    }

}
