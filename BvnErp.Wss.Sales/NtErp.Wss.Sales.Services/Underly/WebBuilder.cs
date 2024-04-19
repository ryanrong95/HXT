using NtErp.Wss.Sales.Services.Underly.InRuntimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 建立者
    /// </summary>
    /// <example>
    /// 目前只考虑了在web环境下的建立者
    /// </example>
    public class WebBuilder : BaseBuilder, IVisitor
    {
        public District Delivery
        {
            get
            {
                string name = this[nameof(Delivery)];
                if (string.IsNullOrWhiteSpace(name))
                {
                    return District.Unknown;
                }

                District outs;
                if (Enum.TryParse(name, out outs))
                {
                    return outs;
                }
                else
                {
                    return District.Unknown;
                }
            }
        }

        public Displayer Displayer { get; private set; }

        public Currency Quotation
        {
            get
            {
                string name = this[nameof(this.Quotation)];
                Currency outs;
                if (Enum.TryParse(name, out outs))
                {
                    return outs;
                }

                return Currency.USD;
            }
        }

        public Currency Transaction
        {
            get
            {
                string name = this[nameof(this.Transaction)];
                Currency outs;
                if (Enum.TryParse(name, out outs))
                {
                    return outs;
                }

                return Currency.USD;
            }
        }

        public Translators.Language Language
        {
            get
            {
                string name = this[nameof(Language)];
                if (string.IsNullOrWhiteSpace(name))
                {
                    string hal = HttpContext.Current.Request.Headers["Accept-language"];
                    if (string.IsNullOrWhiteSpace(hal))
                    {
                        return PartAdapter.Current.Languages.Default;
                    }

                    return hal.Split(',').Select(item => item.Split(';')[0].Trim())
                          .Select(item => PartAdapter.Current.Languages[item])
                          .FirstOrDefault() ?? PartAdapter.Current.Languages.Default;
                }
                else
                {
                    return PartAdapter.Current.Languages[name] ?? PartAdapter.Current.Languages.Default;
                }
            }
        }

        public WebBuilder(string sessionID) : base(sessionID)
        {
            this.Displayer = new Displayer(this.Delivery);
        }

        /// <summary>
        ///  内部索引 方便使用
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>cookies值</returns>
        string this[string index]
        {
            get
            {
                return Utils.Http.Cookies.Cross[index];
            }
        }
    }
}
