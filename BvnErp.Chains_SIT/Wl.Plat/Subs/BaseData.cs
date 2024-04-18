using Needs.Wl.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat
{
    public partial class UserPlat
    {
        /// <summary>
        /// 币种
        /// </summary>
        public static CurrenciesView Currencies
        {
            get { return new CurrenciesView(); }
        }

        /// <summary>
        /// 国家/地区
        /// </summary>
        public static CountriesView Countries
        {
            get { return new CountriesView(); }
        }

        /// <summary>
        /// 单位
        /// </summary>
        public static UnitsView Units
        {
            get { return new UnitsView(); }
        }

        /// <summary>
        /// 包装
        /// </summary>
        public static PackTypesView PackTypes
        {
            get { return new PackTypesView(); }
        }

        /// <summary>
        /// 检验检疫地区
        /// </summary>
        public static CustomsQuarantinesView CustomsQuarantines
        {
            get { return new CustomsQuarantinesView(); }
        }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public static CustomExchangeRatesView CustomExchangeRates
        {
            get { return new CustomExchangeRatesView(); }
        }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public static RealTimeExchangeRatesView RealTimeExchangeRates
        {
            get { return new RealTimeExchangeRatesView(); }
        }
    }
}