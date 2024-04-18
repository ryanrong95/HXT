using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 文件视图
    /// </summary>
    public class PcFilesView : UniqueView<PcFile, PsWmsRepository>
    {
        public PcFilesView()
        {
        }

        internal PcFilesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal PcFilesView(PsWmsRepository reponsitory, IQueryable<PcFile> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<PcFile> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.PcFiles>()
                       select new PcFile
                       {
                           ID = entity.ID,
                           MainID = entity.MainID,
                           Type = (Enums.FileType)entity.Type,
                           CustomName = entity.CustomName,
                           Url = entity.Url,
                           CreateDate = entity.CreateDate,
                           AdminID = entity.AdminID,
                           SiteuserID = entity.SiteuserID,
                       };

            return view;
        }
    }
}
