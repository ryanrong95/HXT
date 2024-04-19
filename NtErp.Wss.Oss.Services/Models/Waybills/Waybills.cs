using Needs.Linq;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 运单
    /// </summary>
    public class Waybills : IEnumerable<Waybill>
    {
        List<Waybill> sources;
        Order father;

        internal Waybills(IEnumerable<Waybill> sources)
        {
            this.sources = sources.Select(item => new Waybill
            {
                ID = item.ID,
                Carrier = item.Carrier,
                Weight = item.Weight,

                Items = item.Items
            }).ToList();
        }

        internal Waybills(Order father, IEnumerable<Waybill> sources) : this(sources)
        {
            this.father = father;
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="index">ID</param>
        /// <returns></returns>
        public Waybill this[string index]
        {
            get
            {
                return this.sources.Single(item => item.ID == index);
            }
        }


        public int Count
        {
            get
            {
                return this.sources.Count;
            }
        }

        #region 持久化
        /// <summary>
        /// 添加附加价值
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Waybill entity)
        {
            this.sources.Add(new Waybill
            {
                ID = entity.ID,
                Carrier = entity.Carrier,
                Weight = entity.Weight
            });
        }

        public void Enter()
        {
            foreach (var item in this.sources)
            {
                item.Enter();
            }
        }
        #endregion 


        public IEnumerator<Waybill> GetEnumerator()
        {
            return this.sources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
