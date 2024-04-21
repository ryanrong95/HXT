using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 员工视图
    /// </summary>
    internal class StaffsOrigin : UniqueView<Staff, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal StaffsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal StaffsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Staff> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Staffs>()
                   select new Staff()
                   {
                       ID = entity.ID,
                       Status = (StaffStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       SelCode = entity.SelCode,
                       UpdateDate = entity.UpdateDate,
                       Name = entity.Name,
                       Code = entity.Code,
                       Gender = (Gender)entity.Gender,
                       AdminID = entity.AdminID,
                       LeagueID = entity.LeagueID,
                       DyjDepartmentCode = entity.DyjDepartmentCode,
                       AssessmentTime = entity.AssessmentTime,
                       AssessmentMethod = entity.AssessmentMethod,
                       DyjCompanyCode = entity.DyjCompanyCode,
                       WorkCity = entity.WorkCity,
                       DyjCode = entity.DyjCode,
                       PostionID = entity.PostionID,
                       RegionID = entity.RegionID,
                       SchedulingID = entity.SchedulingID,
                       DepartmentCode = entity.DepartmentCode,
                       PostionCode = entity.PostionCode,
                   };
        }
    }
}