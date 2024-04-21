using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 个人所得税预扣率
    /// </summary>
    public class PersonalRatesRoll : UniqueView<PersonalRate, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalRatesRoll()
        {
        }

        protected override IQueryable<PersonalRate> GetIQueryable()
        {
            return new PersonalRatesOrigin();
        }
    }
}