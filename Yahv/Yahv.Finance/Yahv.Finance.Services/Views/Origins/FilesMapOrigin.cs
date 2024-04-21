using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class FilesMapOrigin : UniqueView<FilesMap, PvFinanceReponsitory>
    {
        internal FilesMapOrigin() { }

        internal FilesMapOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FilesMap> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FilesMap>()
                   select new FilesMap()
                   {
                       ID = entity.ID,
                       FileID = entity.FileID,
                       Name = entity.Name,
                       Value = entity.Value,
                   };
        }
    }
}
