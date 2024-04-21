using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Tools
{
    abstract public class Standards
    {
        internal Standards() { }

        abstract protected PayItemType InitPayItemType();

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="conduct"></param>
        /// <param name="catalog"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public PayTool this[string conduct, string catalog, string subject]
        {
            get
            {
                return PaymentTools.Data.SingleOrDefault(item => item.Conduct == conduct
                    && item.Catalog == catalog
                    && item.Name == subject
                    && item.Type == this.InitPayItemType()) ?? PayTool.Fact;
            }
        }
    }

    /// <summary>
    /// 应收
    /// </summary>
    public class ReceivablesTool : Standards, IEnumerable<PayTool>
    {
        internal ReceivablesTool() { }
        protected override PayItemType InitPayItemType()
        {
            return PayItemType.Receivables;
        }

        public IEnumerator<PayTool> GetEnumerator()
        {
            return PaymentTools.Data.Where(item => item.Type == PayItemType.Receivables).Select(item => new PayTool()
            {
                Conduct = item.Conduct,
                Catalog = item.Catalog,
                Subject = item.Subject,
                Name = item.Name,
                Type = PayItemType.Receivables
            }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 应付
    /// </summary>
    public class PayablesTool : Standards, IEnumerable<PayTool>
    {
        internal PayablesTool() { }
        protected override PayItemType InitPayItemType()
        {
            return PayItemType.Payables;
        }

        public IEnumerator<PayTool> GetEnumerator()
        {
            return PaymentTools.Data.Where(item => item.Type == PayItemType.Payables).Select(item => new PayTool()
            {
                Conduct = item.Conduct,
                Catalog = item.Catalog,
                Subject = item.Subject,
                Name = item.Name,
                Type = PayItemType.Payables
            }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
