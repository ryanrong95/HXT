using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 轮班员工视图
    /// </summary>
    internal class ShiftStaffsOrigin : UniqueView<ShiftStaff, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal ShiftStaffsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal ShiftStaffsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ShiftStaff> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<ShiftStaffs>()
                   select new ShiftStaff()
                   {
                       ID = entity.ID,
                       ShiftSchedule = entity.ShiftSchedule,
                       ShiftRules = entity.ShiftRules,
                       NextSchedulingID = entity.NextSchedulingID,
                       CreatorID = entity.CreatorID,
                       ModifyID = entity.ModifyID,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                   };
        }
    }
}