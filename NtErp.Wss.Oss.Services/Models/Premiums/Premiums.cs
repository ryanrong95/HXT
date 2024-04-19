using Needs.Utils.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 附加价值
    /// </summary>
    public class Premiums : BaseItems<Premium>
    {
        internal Premiums()
        {
        }

        public Premiums(IEnumerable<Premium> enums) : base(enums)
        {
        }

        internal Premiums(IEnumerable<Premium> enums, ItemStart<Premium> action) : base(enums, action)
        {

        }

        internal Premiums(IQueryable<Premium> enums, ItemStart<Premium> action) : base(enums, action)
        {

        }

        /// <summary>
        /// 添加附加价值
        /// </summary>
        /// <param name="entity"></param>
        override public void Add(Premium entity)
        {
            base.Add(entity);
        }

        /// <summary>
        /// 总额
        /// </summary>
        public decimal Total
        {
            get
            {
                return this.Sum(t => t.Total).Twoh();
            }
        }
    }
}
