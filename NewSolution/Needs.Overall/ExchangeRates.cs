#define Config

using Needs.Overall.Exchanges;

namespace Needs.Overall
{
    /// <summary>
    /// 语言集合
    /// </summary>
    public sealed class ExchangeRates
    {
        public static ExchangeRates<Floating> Floatings
        {
            get
            {
                return ExchangeRates<Floating>.Current;
            }
        }

        public static ExchangeRates<Customs> Customs
        {
            get
            {
                return ExchangeRates<Customs>.Current;
            }
        }
    }
}
