using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views
{

    public class FilesDescriptionsView : vDepthView<Models.FileDescription, Entity.FileDescription, PvdCrmReponsitory>
    {
        public FilesDescriptionsView()
        {
        }

        public FilesDescriptionsView(IQueryable<Models.FileDescription> iQueryable) : base(iQueryable)
        {
        }

        protected FilesDescriptionsView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected FilesDescriptionsView(PvdCrmReponsitory reponsitory, IQueryable<Models.FileDescription> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        public FilesDescriptionsView Search(Expression<Func<Models.FileDescription, bool>> predicate)
        {
            var query = this.IQueryable.Where(predicate);
            return new FilesDescriptionsView(this.Reponsitory, query);
        }

        protected override IQueryable<Models.FileDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.FilesDescription>()
                   where entity.Status == (int)DataStatus.Normal
                   select new Models.FileDescription
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
                   };
        }

        protected override IEnumerable<Entity.FileDescription> OnMyPage(IQueryable<Models.FileDescription> iquery)
        {
            var pcfiles = iquery.ToArray();
            //join enterprises?
            var linq_pcfiles = pcfiles.Select(item => new Entity.FileDescription
            {
                ID = item.ID,
                EnterpriseID = item.EnterpriseID,
                SubID = item.SubID,
                CustomName = item.CustomName,
                Url = item.Url,
                Status = item.Status,
                Type = item.Type,
                CreatorID = item.CreatorID,
                Summary = item.Summary,
                CreateDate = item.CreateDate,
            });
            return linq_pcfiles;

        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="entity"></param>
        static public void Save(Entity.FileDescription entity)
        {
            using (var reponsitory = new FilesDescriptionsView().Reponsitory)
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.FilesDescription()
                {
                    ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                    EnterpriseID = entity.EnterpriseID,
                    SubID = entity.SubID,
                    CustomName = entity.CustomName,
                    Url = entity.Url,
                    Summary = entity.Summary,
                    Type = (int)entity.Type,
                    CreateDate = entity.CreateDate,
                    CreatorID = entity.CreatorID,
                    Status = (int)entity.Status,
                });
            }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="id">文件id</param>
        static public void Delete(string id)
        {
            using (var reponsitory = new FilesDescriptionsView().Reponsitory)
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.FilesDescription>(new
                {
                    Status = Underly.DataStatus.Closed
                }, item => item.ID == id);
            }
        }
    }
}
