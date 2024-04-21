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
using Yahv.Underly.Erps;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views
{

    public class PcFilesView : vDepthView<Models.PcFile, Entity.PcFile, PvdCrmReponsitory>
    {
        public PcFilesView()
        {
        }

        public PcFilesView(IQueryable<Models.PcFile> iQueryable) : base(iQueryable)
        {
        }

        protected PcFilesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected PcFilesView(PvdCrmReponsitory reponsitory, IQueryable<Models.PcFile> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        public PcFilesView Search(Expression<Func<Models.PcFile, bool>> predicate)
        {
            var query = this.IQueryable.Where(predicate);
            return new PcFilesView(this.Reponsitory, query);
        }

        protected override IQueryable<Models.PcFile> GetIQueryable()
        {
            return from files in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.PcFiles>()
                   select new Models.PcFile
                   {
                       ID = files.ID,
                       MainID = files.MainID,
                       CustomName = files.CustomName,
                       Type = (Underly.CrmFileType)files.Type,
                       CreateDate = files.CreateDate,
                       CreatorID = files.AdminID,
                       SiteUserID = files.SiteuserID,
                       Url = Models.FileHost.Web + files.Url
                   };
        }

        protected override IEnumerable<Entity.PcFile> OnMyPage(IQueryable<Models.PcFile> iquery)
        {
            var pcfiles = iquery.ToArray();
            var linq_pcfiles = pcfiles.Select(item => new Entity.PcFile
            {
                ID = item.ID,
                MainID = item.MainID,
                CustomName = item.CustomName,
                Type = item.Type,
                CreateDate = item.CreateDate,
                CreatorID = item.CreatorID,
                SiteUserID = item.SiteUserID,
                Url = item.Url
            });
            return linq_pcfiles;

        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="entity"></param>
        static public void Save(Entity.PcFile entity)
        {
            using (var reponsitory = new PcFilesView().Reponsitory)
            {
                string id = PKeySigner.Pick(PKeyType.PcFile);
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.PcFiles()
                {
                    ID = id,
                    Type = (int)entity.Type,
                    MainID = entity.MainID,
                    CustomName = entity.CustomName,
                    Url = entity.Url,
                    AdminID = entity.CreatorID,
                    CreateDate = DateTime.Now,
                    SiteuserID = entity.SiteUserID
                });
            }
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="id">文件id</param>
        static public void Delete(string id)
        {
            using (var reponsitory = new PcFilesView().Reponsitory)
            {
                reponsitory.Delete<Layers.Data.Sqls.PvdCrm.PcFiles>(item => item.ID == id);
            }
        }
    }
}
