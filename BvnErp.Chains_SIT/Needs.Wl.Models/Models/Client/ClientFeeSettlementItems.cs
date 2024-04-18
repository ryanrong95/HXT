using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.Models
{
    public class ClientFeeSettlementItems : IEnumerable<ClientFeeSettlement>
    {
        private readonly IList<ClientFeeSettlement> list;

        public ClientFeeSettlementItems()
        {

        }

        public ClientFeeSettlementItems(IEnumerable<ClientFeeSettlement> items)
        {
            this.list = items.ToList();
        }

        public ClientFeeSettlement this[Enums.FeeType type]
        {
            get
            {
                return this.list.Where(s => s.FeeType == type && s.Status == (int)Enums.Status.Normal).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取序列中的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count();
            }
        }

        public IEnumerator<ClientFeeSettlement> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
