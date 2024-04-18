using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Plats.Services.Models.Origins;

namespace Yahv.Plats.Services.Views.Origins
{
    /// <summary>
    /// 员工视图
    /// </summary>
    public class StaffsOrigin : UniqueView<Models.Origins.Staff, PvbErmReponsitory>
    {
        public StaffsOrigin()
        {

        }

        public StaffsOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Staff> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                   join p in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Personals>() on entity.ID equals p.ID
                   select new Staff()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       DepartmentCode = entity.DepartmentCode,
                       DyjCode = entity.DyjCode,
                       DyjCompanyCode = entity.DyjCompanyCode,
                       DyjDepartmentCode = entity.DyjDepartmentCode,
                       Email = p.Email,
                       Gender = entity.Gender,
                       LeagueID = entity.LeagueID,
                       Mobile = p.Mobile,
                       PostionCode = entity.PostionCode,
                       PostionID = entity.PostionID,
                       SelCode = entity.SelCode,
                   };
        }
    }
}