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
    public class SwapLimitCountriesView : UniqueView<Models.SwapLimitCountry, ScCustomsReponsitory>
    {
        public SwapLimitCountriesView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public SwapLimitCountriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SwapLimitCountry> GetIQueryable()
        {
            var swapBanksView = new Views.SwapBanksView(this.Reponsitory);

            var result = from limit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapLimitCountries>()
                         join swapBank in swapBanksView on limit.BankID equals swapBank.ID
                         where limit.Status == (int)Enums.Status.Normal
                         select new Models.SwapLimitCountry
                         {
                             ID = limit.ID,
                             BankName=swapBank.Name,
                             BankID=limit.BankID,
                             Code = limit.Code,
                             Name = limit.Name,
                             Status = (Enums.Status)limit.Status,
                             Summary = limit.Summary,
                             UpdateDate = limit.UpdateDate,
                             CreateDate = limit.CreateDate,
                         };
            return result;
        }
    }
}
