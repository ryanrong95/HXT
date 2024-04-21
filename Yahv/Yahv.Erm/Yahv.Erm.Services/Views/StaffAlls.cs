using Yahv.Linq;
using Yahv.Erm.Services.Models.Origins;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Views.Origins;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 员工
    /// </summary>
    public class StaffAlls : UniqueView<Staff, PvbErmReponsitory>
    {
        public StaffAlls()
        {
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="reponsitory"></param>
        internal StaffAlls(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        void kkk()
        {
#if true

#elif false

#endif
        }

#if ForWy
        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Staff> GetIQueryable()
        {
            var admins = new AdminsOrigin(this.Reponsitory);
            var labours = new LaboursOrigin(this.Reponsitory);
            var Postions = new PostionsOrigin(this.Reponsitory);
            var Leagues = new LeaguesOrigin(this.Reponsitory);
            var Personals = new PersonalsOrigin(this.Reponsitory);
            var banks = new BankCardsOrigin(this.Reponsitory);

            return from staff in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                   join admin in admins on staff.ID equals admin.StaffID into adminviews
                   from admin in adminviews.DefaultIfEmpty()
                   join labour in labours on staff.ID equals labour.ID into labourviews
                   from labour in labourviews.DefaultIfEmpty()
                   join postion in Postions on staff.PostionID equals postion.ID into postionviews
                   from postion in postionviews.DefaultIfEmpty()
                   join league in Leagues on staff.WorkCity equals league.ID into leagueviews
                   from league in leagueviews.DefaultIfEmpty()
                   join personal in Personals on staff.ID equals personal.ID into personalviews
                   from personal in personalviews.DefaultIfEmpty()
                   join bank in banks on staff.ID equals bank.ID into bankViews
                   from bank in bankViews.DefaultIfEmpty()
                   join company in Reponsitory.ReadTable<CompaniesTopView>() on labour.EnterpriseID equals company.ID into joinCompany
                   from company in joinCompany.DefaultIfEmpty()
                   where staff.Status != (int)StaffStatus.Delete
                   select new Staff
                   {
                       ID = staff.ID,
                       Name = staff.Name,
                       SelCode = staff.SelCode,
                       AssessmentMethod = staff.AssessmentMethod,
                       Code = staff.Code,
                       AssessmentTime = staff.AssessmentTime,
                       DyjCompanyCode = staff.DyjCompanyCode,
                       DyjDepartmentCode = staff.DyjDepartmentCode,
                       Gender = (Gender)staff.Gender,
                       LeagueID = staff.LeagueID,
                       DyjCode = staff.DyjCode,
                       PostionID = staff.PostionID,
                       UpdateDate = staff.UpdateDate,
                       WorkCity = staff.WorkCity,
                       CreateDate = staff.CreateDate,
                       Status = (StaffStatus)staff.Status,
                       LeaveDate = labour.LeaveDate,
                       EntryDate = labour.EntryDate,
                       PostionName = postion.Name,
                       CityName = league.Name,
                       IDCard = personal.IDCard,
                       EnterpriseID = labour.EnterpriseID,
                       EnterpriseCompany = company.Name,
                       BankAccount = bank.Account,
                       Password = admin.Password,
                       RoleID = admin.RoleID,
                       UserName = admin.UserName,
                       Email = personal.Email,
                       Mobile = personal.Mobile,
                   };
        }
#else
        protected override IQueryable<Staff> GetIQueryable()
        {
            var personalView = new PersonalsOrigin(this.Reponsitory);
            var adminView = new AdminsOrigin(this.Reponsitory);
            var labourView = new LaboursOrigin(this.Reponsitory);
            var positionView = new PostionsOrigin(this.Reponsitory);
            var cityView = new LeaguesOrigin(this.Reponsitory).Where(item => item.Category == Underly.Category.StaffInCity);
            var banks = new BankCardsOrigin(this.Reponsitory);

            return from staff in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                   join personal in personalView on staff.ID equals personal.ID
                   join admin in adminView on staff.ID equals admin.StaffID into admins
                   from admin in admins.DefaultIfEmpty()
                   join labour in labourView on staff.ID equals labour.ID into labours
                   from labour in labours.DefaultIfEmpty()
                   join position in positionView on staff.PostionID equals position.ID into positions
                   from position in positions.DefaultIfEmpty()
                   join city in cityView on staff.WorkCity equals city.ID into citys
                   from city in citys.DefaultIfEmpty()
                   join bank in banks on staff.ID equals bank.ID into bankViews
                   from bank in bankViews.DefaultIfEmpty()
                   where staff.Status != (int)StaffStatus.Delete
                   select new Staff
                   {
                       ID = staff.ID,
                       Name = staff.Name,
                       Code = staff.Code,
                       SelCode = staff.SelCode,
                       Gender = (Gender)staff.Gender,
                       DyjCompanyCode = staff.DyjCompanyCode,
                       DyjDepartmentCode = staff.DyjDepartmentCode,
                       DyjCode = staff.DyjCode,
                       WorkCity = staff.WorkCity,
                       LeagueID = staff.LeagueID,
                       PostionID = staff.PostionID,
                       DepartmentCode = staff.DepartmentCode,
                       PostionCode = staff.PostionCode,
                       AssessmentMethod = staff.AssessmentMethod,
                       AssessmentTime = staff.AssessmentTime,
                       AdminID = staff.AdminID,
                       UpdateDate = staff.UpdateDate,
                       CreateDate = staff.CreateDate,
                       Status = (StaffStatus)staff.Status,
                       RegionID = staff.RegionID,
                       SchedulingID = staff.SchedulingID,

                       Personal = personal,
                       Admin = admin,
                       Labour = labour,
                       Postion = position,
                       City = city,

                       //冗余数据（兼顾以前版本）
                       EnterpriseID = labour.EnterpriseID,
                       EnterpriseCompany = labour.EntryCompany,
                       LeaveDate = labour.LeaveDate,
                       EntryDate = labour.EntryDate,
                       PostionName = position.Name,
                       CityName = city.Name,
                       IDCard = personal.IDCard,
                       BankAccount = bank.Account,
                       BankName = bank.Bank,
                       Password = admin.Password,
                       RoleID = admin.RoleID,
                       UserName = admin.UserName,
                       Email = personal.Email,
                       Mobile = personal.Mobile,
                   };
        }

#endif



        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void Delete(string[] ids)
        {
            this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
            {
                Status = Status.Delete,
            }, item => ids.Contains(item.ID));
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="id">唯一码</param>
        /// <returns>Partner</returns>
        public override Staff this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }
    }

    /// <summary>
    /// 员工文件视图
    /// </summary>
    public class StaffFileAlls : UniqueView<Yahv.Services.Models.CenterFileDescription, PvCenterReponsitory>
    {
        string StaffID = string.Empty;

        public StaffFileAlls(string staffID)
        {
            this.StaffID = staffID;
        }

        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            var files = new Yahv.Services.Views.CenterFilesTopView()
                .Where(t => t.StaffID == StaffID && t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal);
            return files;
        }
    }

    /// <summary>
    /// 申请文件视图
    /// </summary>
    public class ApplicationFileAlls : UniqueView<Yahv.Services.Models.CenterFileDescription, PvCenterReponsitory>
    {
        string ApplicationID = string.Empty;

        public ApplicationFileAlls(string applicationID)
        {
            this.ApplicationID = applicationID;
        }

        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            var files = new Yahv.Services.Views.CenterFilesTopView()
                .Where(t => t.ErmApplicationID == ApplicationID && t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal);
            return files;
        }
    }
}
