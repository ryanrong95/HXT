using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 承运商通用视图
    /// </summary>
    public class CarriersTopView : UniqueView<Carrier, PvbCrmReponsitory>
    {
        public CarriersTopView()
        {

        }

        public CarriersTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Carrier> GetIQueryable()
        {
            return new Yahv.Services.Views.CarriersTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 包括匿名
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Carrier> IncludeAnonymous()
        {
            var anonymous = new List<Carrier>()
            {
                new Carrier()
                {
                     ID = AnonymousEnterprise.Current.ID,
                    Name = AnonymousEnterprise.Current.Name
                }
            };

            return (this.ToArray().Concat(anonymous)).OrderBy(item => item.Name);
        }
    }
}
