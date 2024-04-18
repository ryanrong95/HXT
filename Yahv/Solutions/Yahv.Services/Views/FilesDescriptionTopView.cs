using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 文件通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class FilesDescriptionTopView<TReponsitory> : UniqueView<FileDescription, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public FilesDescriptionTopView()
        {

        }

        public FilesDescriptionTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FileDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FilesDescriptionTopView>()
                   select new FileDescription()
                   {
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       AdminID = entity.AdminID,
                       Type = (FileType)entity.Type,
                       Name = entity.Name,
                       Status = (ApprovalStatus)entity.Status,
                       EnterpriseID = entity.EnterpriseID,
                       FileFormat = entity.FileFormat,
                       Url = entity.Url,
                       Summary = entity.Summary,
                   };
        }
    }
}
