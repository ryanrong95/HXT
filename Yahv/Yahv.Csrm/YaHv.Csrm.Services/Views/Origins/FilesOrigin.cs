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
    public class FilesOrigin : Yahv.Linq.UniqueView<Models.Origins.FileDescription, PvbCrmReponsitory>
    {
        internal FilesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal FilesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FileDescription> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FilesDescription>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Models.Origins.FileDescription
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Name = entity.Name,
                       Type = (FileType)entity.Type,
                       Url = entity.Url,
                       CreateDate = entity.CreateDate,
                       FileFormat = entity.FileFormat,
                       Summary = entity.Summary,
                       Status = (ApprovalStatus)entity.Status,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       Enterprise = enterprise,
                       CompanyID = entity.PaysID
                   };
        }
    }
}
