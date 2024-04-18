using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 代仓储客户
    /// </summary>
    public class Storage : Yahv.Services.Views.StorageTop//Yahv.Services.Models.Storage
    {
        #region 扩展属性

        /// <summary>
        /// 客户编号
        /// </summary>
        public string EnterCode { get; set; }

        public string OriginDec
        {
            get
            {
                if (string.IsNullOrEmpty(this.Origin) || this.Origin == "***")
                {
                    return Yahv.Underly.Origin.Unknown.GetDescription();
                }
                else
                {
                    return ((Origin)Enum.Parse(typeof(Origin), this.Origin)).GetDescription();
                }
            }
        }

        public string CurrencyDec
        {
            get
            {
                if (this.Currency == null)
                {
                    return "--";
                }
                else
                {
                    return this.Currency.GetDescription();
                }
            }
        }

        public string UnitPriceDec
        {
            get
            {
                return this.UnitPrice == null ? "--" : this.UnitPrice.ToString();
            }
        }

        public string TotalPriceDec
        {
            get
            {
               // return this.UnitPrice == null ? "--" : (this.UnitPrice * this.Quantity).ToString();
                return this.UnitPrice == null ? "--" : Math.Round((double)(this.UnitPrice * this.Quantity), 3).ToString();
            }
        }

        #endregion
    }
}
