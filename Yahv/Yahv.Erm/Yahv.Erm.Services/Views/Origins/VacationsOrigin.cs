using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 员工假期视图
    /// </summary>
    public class VacationsOrigin : UniqueView<Vacation, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public VacationsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public VacationsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Vacation> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Vacations>()
                   select new Vacation()
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       Type = (VacationType)entity.Type,
                       StartTime = entity.StartTime,
                       EndTime = entity.EndTime,
                       Lefts = entity.Lefts,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Total = entity.Total,
                   };
        }
    }
}