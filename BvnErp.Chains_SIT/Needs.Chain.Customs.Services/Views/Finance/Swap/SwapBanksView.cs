using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 银行的视图
    /// </summary>
    public class SwapBanksView : UniqueView<Models.SwapBank, ScCustomsReponsitory>
    {
        public SwapBanksView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public SwapBanksView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SwapBank> GetIQueryable()
        {
            var result = from bank in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapBanks>()
                         where bank.Status == (int)Enums.Status.Normal
                         select new Models.SwapBank
                         {
                             ID = bank.ID,
                             Code=bank.Code,
                             Name = bank.Name,
                             Status = (Enums.Status)bank.Status,
                             Summary = bank.Summary,
                             UpdateDate = bank.UpdateDate,
                             CreateDate = bank.CreateDate,
                         };
            return result;
        }
    }
}
