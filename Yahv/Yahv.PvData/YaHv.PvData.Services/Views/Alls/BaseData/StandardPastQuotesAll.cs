using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 标准历史价格
    /// </summary>
    public class StandardPastQuotesAll : UniqueView<Models.StandardPastQuote, PvDataReponsitory>
    {
        public StandardPastQuotesAll()
        {
        }

        internal StandardPastQuotesAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<StandardPastQuote> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.StandardPastQuotes>()
                   select new Models.StandardPastQuote
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       Currency = entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       Quantity = entity.Quantity,
                       CIQprice = entity.CIQprice,
                       CreateDate = entity.CreateDate,
                       Source = entity.Source
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="name">品牌名称</param>
        /// <returns></returns>
        public IQueryable<Models.StandardPastQuote> this[string partNumber, string manufacturer]
        {
            get
            {
                return this.Where(cpn => cpn.PartNumber.Trim() == partNumber.Trim() && cpn.Manufacturer.Trim() == manufacturer.Trim());
            }
        }
    }
}
