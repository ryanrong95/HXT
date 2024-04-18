using Needs.Linq;
using Needs.Underly;
using Needs.Utils.Converters;
using NtErp.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 汇率
    /// </summary>
    public class ExchangeRate
    {
        internal string Type { get; set; }
        public District District { get; set; }
        public Currency From { get; set; }
        public Currency To { get; set; }
        virtual public decimal Value { get; set; }

        public ExchangeRate()
        {
            this.Type = this.GetType().Name;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOverallsReponsitory())
            {
                Expression<Func<Layer.Data.Sqls.BvOveralls.ExchangeRates, bool>> predicate = item => item.Type == this.Type
                    && item.District == (int)this.District
                    && item.From == (int)this.From
                    && item.To == (int)this.To;

                var entity = reponsitory.GetTable<Layer.Data.Sqls.BvOveralls.ExchangeRates>().SingleOrDefault(predicate);
                if (entity == null)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), predicate);

                    //entity.Value = this.Value;
                    //reponsitory.Submit();
                }
            }
        }
    }
}
