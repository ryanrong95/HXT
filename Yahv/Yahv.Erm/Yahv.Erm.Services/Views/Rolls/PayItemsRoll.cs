using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 工资录入
    /// </summary>
    public class PayItemsRoll : UniqueView<PayItem, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PayItemsRoll()
        {
        }

        protected override IQueryable<PayItem> GetIQueryable()
        {
            return new Origins.PayItemsOrigin();
        }
    }
}