using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class PcFilesOrigin : UniqueView<Models.Origin.PcFile, PsOrderRepository>
    {
        #region 构造函数
        public PcFilesOrigin()
        {
        }

        public PcFilesOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.PcFile> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>()
                       select new Models.Origin.PcFile
                       {
                           ID = entity.ID,
                           MainID = entity.MainID,
                           Type = (Enums.PsOrderFileType)entity.Type,
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
