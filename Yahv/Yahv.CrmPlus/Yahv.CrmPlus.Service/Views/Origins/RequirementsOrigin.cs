using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 客户特殊要求
    /// </summary>
    public class RequirementsOrigin : Linq.UniqueView<Requirement, PvdCrmReponsitory>
    {
        internal RequirementsOrigin()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal RequirementsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Requirement> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            //var fileView = new Views.Origins.FilesDescriptionOrigin(this.Reponsitory).Where(x=>x.Type==CrmFileType.Requirements);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Requirements>()
                   //join file in fileView on entity.EnterpriseID equals file.EnterpriseID
                   join creat_admin in adminsView on entity.CreatorID equals creat_admin.ID into creatAdmin
                   from creator in creatAdmin.DefaultIfEmpty()
                   select new Requirement
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       SpecialType = (SpecialType)entity.SpecialType,
                       Content = entity.Content,
                       Status = (DataStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Admin = creator,
                       //FileName = file.CustomName,
                       //Url = file.Url
                   };
        }
    }
}
