using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PalletsOrigin : UniqueView<Models.PalletNumber, ScCustomsReponsitory>
    {
        public PalletsOrigin()
        {
        }

        internal PalletsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PalletNumber> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Pallets>()
                   select new Models.PalletNumber
                   {
                       ID = para.ID,
                       Stock = para.Stock,
                       Pallet = para.Pallet,
                       NoticeTime = para.NoticeTime,
                       Status = (Status)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       Summary = para.Summary,
                   };
        }
    }
}
