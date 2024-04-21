using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class FilesDescriptionOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.FilesDescription, PvdCrmReponsitory>
    {
        internal FilesDescriptionOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal FilesDescriptionOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FilesDescription> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.FilesDescription>()
                       //保护人
                   join admin in adminsView on entity.CreatorID equals admin.ID into _Admin
                   from creatorAdmin in _Admin.DefaultIfEmpty()
                   select new FilesDescription
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       SubID = entity.SubID,
                       CustomName = entity.CustomName,
                       Url = Models.FileHost.Web + entity.Url,
                       Status = (DataStatus)entity.Status,
                       Type = (CrmFileType)entity.Type,
                       CreatorID = entity.CreatorID,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       CreatorName = creatorAdmin.RealName
                   };
        }
    }
}
