using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 海关汇率的视图
    /// </summary>
    public class CustomExchangeRatesView : ExchangeRatesView<Models.CustomExchangeRate>
    {
        public CustomExchangeRatesView():base(Enums.ExchangeRateType.Custom)
        {

        }
    }
}
