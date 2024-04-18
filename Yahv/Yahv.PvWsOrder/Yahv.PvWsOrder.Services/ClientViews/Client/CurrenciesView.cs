using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 币种
    /// </summary>
    public class CurrenciesView : UniqueView<BaseCurrency, ScCustomReponsitory>
    {
        public CurrenciesView()
        {
        }

        protected override IQueryable<BaseCurrency> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.BaseCurrencies>()
                   select new BaseCurrency
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       EnglishName = entity.EnglishName
                   };
        }

        public BaseCurrency FindByCode(string code)
        {
            return this.GetIQueryable().Where(s => s.Code == code).FirstOrDefault();
        }
    }

    /// <summary>
    /// 币种
    /// </summary>
    public class BaseCurrency : IUnique
    {
        private string id;

        public string ID
        {
            get
            {
                return this.id ?? this.Code;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }
    }
}
