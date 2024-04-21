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
    public class FilesDescriptionOrigin : UniqueView<FilesDescription, PvFinanceReponsitory>
    {
        internal FilesDescriptionOrigin() { }

        internal FilesDescriptionOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FilesDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FilesDescription>()
                   select new FilesDescription()
                   {
                       ID = entity.ID,
                       Type = (Enums.FileDescType)entity.Type,
                       Url = entity.Url,
                       CustomName = entity.CustomName,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Status = (Underly.GeneralStatus)entity.Status,
                   };
        }
    }
}
