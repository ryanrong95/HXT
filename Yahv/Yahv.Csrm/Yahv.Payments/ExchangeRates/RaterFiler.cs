using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 汇率过滤器
    /// </summary>
    public class RaterFiler : IEnumerable<Rater>
    {

        IEnumerable<Rater> data;

        ExchangeType type;
        ExchangeDistrict district;
        /// <summary>
        /// 内部构造器
        /// </summary>
        /// <param name="type"></param>
        internal RaterFiler(ExchangeType type)
        {
            this.type = type;
            this.district = ExchangeRates.DefaultDistrict;
        }

        /// <summary>
        /// 索引汇率
        /// </summary>
        /// <param name="from">来源币种</param>
        /// <param name="to">目标币种</param>
        /// <returns></returns>
        public decimal this[Currency from, Currency to]
        {
            get
            {
                if (from == to)
                {
                    return 1m;
                }

                //正向获取
                var rater = this.SingleOrDefault(item => item.From == from && item.To == to);

                if (rater == null)
                {
                    //反转获取
                    var reverser = this.SingleOrDefault(item => item.From == to && item.To == from);

                    if (reverser == null)
                    {
                        var from2cny = this.SingleOrDefault(item => item.From == from && item.To == Currency.CNY);
                        var to2cny = this.SingleOrDefault(item => item.From == to && item.To == Currency.CNY);

                        if (from2cny == null || to2cny == null)
                        {
                            throw new NotSupportedException($@"系统未设置  {from}|{to} 或  {to}|{from} 的 {this.type.GetDescription()}，同时也找不到{from}|{Currency.CNY}  或 {to}|{Currency.CNY}的 {this.type.GetDescription()}");
                        }

                        //利用人民币做中间汇率，做币种转换
                        return from2cny.Value / to2cny.Value;

                    }

                    //反转计算
                    return 1m / reverser.Value;
                }

                //正常返回
                return rater?.Value ?? 1m;
            }
        }


        /// <summary>
        /// 获取全部数据
        /// </summary>
        public IEnumerable<Rater> Alls
        {
            get
            {
                List<Rater> list = new List<Rater>();
                foreach (var rater in this)
                {
                    list.Add(rater);
                    if (!list.Any(item => item.From == rater.To && item.To == rater.From))
                    {
                        list.Add(new Rater
                        {
                            From = rater.To,
                            To = rater.From,
                            District = rater.District,
                            Type = rater.Type,
                            Value = this[rater.To, rater.From]
                        });
                    }
                }
                return list;
            }
        }


        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<Rater> GetEnumerator()
        {
            return ExchangeRates.Current.Where(item => item.Type == this.type
                && item.District == this.district).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
