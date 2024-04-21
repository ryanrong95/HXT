using System;
using System.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services
{
    public class CurrencyHelper
    {
        /// <summary>
        /// 根据简称获取Currency
        /// </summary>
        /// <param name="shortName">简称（CNY）</param>
        /// <returns></returns>
        static public Currency GetCurrency(string shortName)
        {
            return Enum.GetValues(typeof(Currency)).Cast<Currency>()
                .FirstOrDefault(item => item.GetCurrency().ShortName == shortName);
        }
    }
}