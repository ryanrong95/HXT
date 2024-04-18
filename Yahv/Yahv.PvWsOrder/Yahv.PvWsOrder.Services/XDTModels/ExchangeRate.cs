using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 币种汇率
    /// </summary>
    public abstract class ExchangeRate :IUnique
    {
        private string id;

        public new string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Type.GetHashCode()).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public ExchangeRate(ExchangeRateType type) 
        {
            this.Type = type;
        }

        /// <summary>
        /// 币种代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 汇率类型：海关汇率、实时汇率
        /// </summary>
        public ExchangeRateType Type { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

    }
}
