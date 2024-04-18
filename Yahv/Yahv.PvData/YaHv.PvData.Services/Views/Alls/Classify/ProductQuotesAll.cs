using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 产品报价
    /// </summary>
    public class ProductQuotesAll : UniqueView<Models.ProductQuote, PvDataReponsitory>
    {
        public ProductQuotesAll()
        {
        }

        internal ProductQuotesAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductQuote> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductQuotes>()
                   select new Models.ProductQuote
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       Origin = entity.Origin,
                       Currency = entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       Quantity = entity.Quantity,
                       CIQprice = entity.CIQprice,
                       CreateDate = entity.CreateDate
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="months">月数</param>
        /// <returns></returns>
        public IQueryable<Models.ProductQuote> this[string partNumber, int months]
        {
            get
            {
                return this.Where(cpn => cpn.PartNumber == partNumber && cpn.CreateDate >= DateTime.Now.AddMonths(months));
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <param name="months">月数</param>
        /// <returns></returns>
        public IQueryable<Models.ProductQuote> this[string partNumber, string manufacturer, int months]
        {
            get
            {
                return this.Where(cpn => cpn.PartNumber == partNumber && cpn.Manufacturer == manufacturer && cpn.CreateDate >= DateTime.Now.AddMonths(months));
            }
        }
    }
}
