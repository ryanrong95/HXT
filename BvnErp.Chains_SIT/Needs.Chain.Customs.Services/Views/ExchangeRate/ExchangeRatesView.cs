using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 全部汇率的视图
    /// </summary>
    public abstract class ExchangeRatesView<T> : UniqueView<T, ScCustomsReponsitory> where T : Models.ExchangeRate, new()
    {
        private string Currency;

        public ExchangeRatesView()
        {
        }

        internal ExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        public ExchangeRatesView(string currency)
        {
            this.Currency = currency;
        }

        public T ToRate()
        {
            return this.GetIQueryable().Where(item => item.Code == this.Currency).FirstOrDefault();
        }

        protected override IQueryable<T> GetIQueryable()
        {
            var currencyView = new BaseCurrenciesView(this.Reponsitory);
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>()
                   join currency in currencyView on entity.Code equals currency.ID
                   select new T
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = currency.Name,
                       Type = (Enums.ExchangeRateType)entity.Type,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }
    }

    /// <summary>
    /// 海关汇率的视图
    /// </summary>
    public class CustomExchangeRatesView : ExchangeRatesView<Models.CustomExchangeRate>
    {
        public CustomExchangeRatesView()
        {
        }

        internal CustomExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        public CustomExchangeRatesView(string currency) : base(currency)
        {

        }

        protected override IQueryable<Models.CustomExchangeRate> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Type == Enums.ExchangeRateType.Custom
                   select new Models.CustomExchangeRate
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }
    }

    /// <summary>
    /// 实时汇率的视图
    /// </summary>
    public class RealTimeExchangeRatesView : ExchangeRatesView<Models.RealTimeExchangeRate>
    {
        public RealTimeExchangeRatesView()
        {

        }

        internal RealTimeExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        public RealTimeExchangeRatesView(string currency) : base(currency)
        {

        }

        protected override IQueryable<Models.RealTimeExchangeRate> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Type == Enums.ExchangeRateType.RealTime
                   select new Models.RealTimeExchangeRate
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }

        public IQueryable<Models.RealTimeExchangeRate> GetIQueryable(string currency)
        {
            return this.GetIQueryable().Where(t => t.Code == currency);
        }
    }


    /// <summary>
    /// 九点半汇率的视图
    /// </summary>
    public class NineRealTimeExchangeRatesView : ExchangeRatesView<Models.RealTimeExchangeRate>
    {
        public NineRealTimeExchangeRatesView()
        {

        }

        internal NineRealTimeExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        public NineRealTimeExchangeRatesView(string currency) : base(currency)
        {

        }

        protected override IQueryable<Models.RealTimeExchangeRate> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Type == Enums.ExchangeRateType.NineRealTime
                   select new Models.RealTimeExchangeRate
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       Rate = entity.Rate,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }

        public IQueryable<Models.RealTimeExchangeRate> GetIQueryable(string currency)
        {
            return this.GetIQueryable().Where(t => t.Code == currency);
        }
    }



    /// <summary>
    /// 九点半汇率的视图
    /// </summary>
    public class ZGExchangeRatesView : ExchangeRatesView<Models.RealTimeExchangeRate>
    {
        public ZGExchangeRatesView()
        {

        }

        internal ZGExchangeRatesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        public ZGExchangeRatesView(string currency) : base(currency)
        {

        }

        protected override IQueryable<Models.RealTimeExchangeRate> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   //where entity.Type == Enums.ExchangeRateType.RealTime
                   select new Models.RealTimeExchangeRate
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       Rate = entity.Rate,
                       Type = entity.Type,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }

        public IQueryable<Models.RealTimeExchangeRate> GetIQueryable(string currency)
        {
            return this.GetIQueryable().Where(t => t.Code == currency);
        }
    }

    ///// <summary>
    ///// 币种的汇率的视图
    ///// </summary>
    //public class CurrencyExchangeRatesView : ExchangeRatesView
    //{
    //    Enums.ExchangeRateType RateType;
    //    string Code;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="type">汇率类型</param>
    //    /// <param name="currencyCode">币种</param>
    //    public CurrencyExchangeRatesView(Enums.ExchangeRateType type, string currencyCode)
    //    {
    //        RateType = type;
    //        Code = currencyCode;
    //    }

    //    protected override IQueryable<Models.ExchangeRate> GetIQueryable()
    //    {
    //        return from entity in base.GetIQueryable()
    //               where entity.Type == this.RateType && entity.Code == this.Code
    //               select entity;
    //    }
    //}
}