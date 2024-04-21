using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.Payments.Tools
{
    /// <summary>
    /// 支付项类型
    /// </summary>
    public enum PayItemType
    {
        /// <summary>
        /// 应收
        /// </summary>
        [Description("应收账款")]
        Receivables = 0,
        /// <summary>
        /// 应付
        /// </summary>
        [Description("应付账款")]
        Payables = 1
    }

    public class PayQuote
    {
        public Currency? Currency { get; set; }
        public decimal? Price { get; set; }


        public static implicit operator PayQuotes(PayQuote value)
        {
            return new PayQuotes(value);
        }
    }


    class MyClass
    {
        public MyClass()
        {
            //var kk = PaymentTools.Carloads["科目"].Quotes;
            //PayQuote quote = PaymentTools.Receivables["", "", ""].Quotes("大小"  or 1, 22);

            //PaymentManager.Erp("asdfasdf")["asdf", "asdf"]["asdf"].Receivable["asdf", "asdf"].Record(kk.Currency, kk.Price);

            //PaymentManager.Erp("asdfasdf")["asdf", "asdf"]["asdf"].Credit["", ""].Cost();

            //PaymentManager.Erp("asdfasdf").Credit.For().Cost();



        }
    }

    /// <summary>
    /// 支付集合
    /// </summary>
    /// <remarks>
    /// 非单体的结构写法
    /// </remarks>
    public class PayQuotes : IEnumerable<PayQuote>
    {

        IEnumerable<PayQuote> data;

        public Currency Currency
        {
            get
            {
                return this.First().Currency ?? Currency.Unknown;
            }
            internal set
            {
                this.First().Currency = value;
            }
        }
        public decimal? Price
        {
            get
            {
                return this.First()?.Price;
            }
            internal set
            {
                if (value != null) this.First().Price = (decimal)value;
            }
        }

        public int Count { get { return this.data.Count(); } }

        public PayQuotes() : this(new PayQuote[1])
        {
            var ss = this.data;
        }

        public PayQuotes(IEnumerable<PayQuote> data)
        {
            this.data = data;
        }

        public PayQuotes(params PayQuote[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// 根据币种获取支付信息
        /// </summary>
        /// <param name="index">币种</param>
        /// <returns>支付信息</returns>
        public PayQuote this[Currency index]
        {
            get { return this.data.SingleOrDefault(item => item.Currency == index); }
        }

        public IEnumerator<PayQuote> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static implicit operator PayQuote(PayQuotes value)
        {
            return new PayQuote
            {
                Currency = value.Currency,
                Price = value?.Price
            };
        }
    }

    /// <summary>
    /// 工具项
    /// </summary>
    public class PayTool
    {
        /// <summary>
        /// 据实
        /// </summary>
        static public PayTool Fact = new PayTool()
        {
            Conduct = nameof(Fact),
            Subject = nameof(Fact),
            Catalog = nameof(Fact),
            Quotes = null
        };

        public PayItemType Type { get; set; }

        public string Conduct { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }

        public PayQuotes Quotes { get; set; }

        public override bool Equals(object obj)
        {
            var value = obj as PayTool;
            if (this.Conduct == nameof(Fact)
                && this.Catalog == nameof(Fact)
                && this.Subject == nameof(Fact)
                && value.Conduct == nameof(Fact)
                && value.Catalog == nameof(Fact)
                && value.Subject == nameof(Fact))
            {
                return true;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


}
