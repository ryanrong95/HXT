using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.InRuntimes
{
    public sealed class ReadonlyInfo<T> : IEnumerable<T>
    {
        List<T> items;

        public ReadonlyInfo(params T[] arry)
        {
            this.items = new List<T>(arry);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// 币种信息
    /// </summary>
    public sealed class Displayer
    {
        /// <summary>
        /// 显示币种
        /// </summary>
        public ReadonlyInfo<Currency> Quotations { get; private set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public ReadonlyInfo<Currency> Transactions { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="settling">指定 交易币种</param>
        public Displayer(District district)
        {
            switch (district)
            {
                case District.CN:
                    {
                        this.Transactions = new ReadonlyInfo<Currency>(Currency.CNY);
                        this.Quotations = new ReadonlyInfo<Currency>(Currency.CNY, Currency.USD);
                    }
                    break;
                case District.HK:
                    {
                        this.Transactions = new ReadonlyInfo<Currency>(Currency.CNY, Currency.HKD, Currency.USD);
                        this.Quotations = new ReadonlyInfo<Currency>(Currency.CNY, Currency.HKD, Currency.USD);
                    }
                    break;

                case District.IN:
                case District.US:
                case District.Global:
                case District.Unknown:
                default:
                    {
                        this.Transactions = new ReadonlyInfo<Currency>(Currency.USD);
                        this.Quotations = new ReadonlyInfo<Currency>(Currency.USD);
                    }
                    break;

            }
        }

    }
}
