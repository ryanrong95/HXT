using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 个人所得税预扣率表 视图
    /// </summary>
    public class PersonalRatesOrigin : UniqueView<PersonalRate, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PersonalRatesOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PersonalRatesOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 个人所得税预扣率表 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        protected override IQueryable<PersonalRate> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.PersonalRates>()
                   select new PersonalRate()
                   {
                       ID = entity.ID,
                       Levels = entity.Levels,
                       BeginAmount = entity.BeginAmount,
                       EndAmount = entity.EndAmount,
                       Rate = entity.Rate,
                       Deduction = entity.Deduction,
                       Description = entity.Description,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       AdminID = entity.AdminID,
                   };
        }
    }
}