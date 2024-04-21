using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Tools
{
    /// <summary>
    /// 标签费
    /// </summary>
    public class LabelFee
    {
        internal LabelFee() { }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="count">标签个数</param>
        /// <returns></returns>
        public PayTool this[int count]
        {
            get
            {
                if (count <= 10)
                {
                    return PaymentTools.Data.SingleOrDefault(item => item.Conduct == "代仓储"
                                                                       && item.Catalog == "杂费"
                                                                       && item.Subject == "处理标签费少量"
                                                                       && item.Type == PayItemType.Receivables);

                }
                else
                {
                    //最低100
                    var miniPrice = PaymentTools.Data.SingleOrDefault(item => item.Conduct == "代仓储"
                                                                              && item.Catalog == "杂费"
                                                                              && item.Subject == "处理标签费少量"
                                                                              && item.Type == PayItemType.Receivables);

                    //批量按5元一个
                    var price = PaymentTools.Data.SingleOrDefault(item => item.Conduct == "代仓储"
                                                                         && item.Catalog == "杂费"
                                                                         && item.Subject == "处理标签费批量"
                                                                         && item.Type == PayItemType.Receivables);

                    price.Quotes.Price = price.Quotes.Price * count;

                    //少于100 按照100 计算
                    if (price.Quotes.Price < miniPrice.Quotes.Price)
                    {
                        return miniPrice;
                    }

                    return price;
                }
            }
        }
    }
}
