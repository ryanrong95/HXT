using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 员工个人信息视图
    /// </summary>
    internal class PersonalsOrigin : UniqueView<Personal, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PersonalsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PersonalsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Personal> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Personals>()
                   select new Personal()
                   {
                       ID = entity.ID,
                       Weight = entity.Weight,
                       Height = entity.Height,
                       IsMarry = entity.IsMarry,
                       PassAddress = entity.PassAddress,
                       GraduatInstitutions = entity.GraduatInstitutions,
                       Volk = entity.Volk,
                       Major = entity.Major,
                       Education = entity.Education,
                       IDCard = entity.IDCard,
                       PoliticalOutlook = entity.PoliticalOutlook,
                       HomeAddress = entity.HomeAddress,
                       NativePlace = entity.NativePlace,
                       Image = entity.Image,
                       Blood = entity.Blood,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       BirthDate = entity.BirthDate,
                       BeginWorkDate = entity.BeginWorkDate,
                       GraduationDate = entity.GraduationDate,
                       Healthy = entity.Healthy,
                       EmergencyContact = entity.EmergencyContact,
                       EmergencyMobile = entity.EmergencyMobile,
                       LanguageLevel = entity.LanguageLevel,
                       ComputerLevel = entity.ComputerLevel,
                       PositionName = entity.PositionName,
                       Treatment = entity.Treatment,
                       SelfEvaluation = entity.SelfEvaluation,
                       Summary = entity.Summary,
                   };
        }
    }

    /// <summary>
    /// 员工的工作经历
    /// </summary>
    public class PersonalWorkExperiencesOrigin : UniqueView<PersonalWorkExperience, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalWorkExperiencesOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public PersonalWorkExperiencesOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PersonalWorkExperience> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<PersonalWorkExperiences>()
                   select new PersonalWorkExperience()
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       StartTime = entity.StartTime,
                       EndTime = entity.EndTime,
                       Company = entity.Company,
                       Position = entity.Position,
                       Salary = entity.Salary,
                       LeaveReason = entity.LeaveReason,
                       Phone = entity.Phone,
                   };
        }
    }

    /// <summary>
    /// 员工的家庭成员
    /// </summary>
    public class PersonalFamilyMembersOrigin : UniqueView<PersonalFamilyMember, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalFamilyMembersOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public PersonalFamilyMembersOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PersonalFamilyMember> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<PersonalFamilyMembers>()
                   select new PersonalFamilyMember()
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       Name = entity.Name,
                       Relation = entity.Relation,
                       Age = entity.Age,
                       Company = entity.Company,
                       Position = entity.Position,
                       Phone = entity.Phone ,
                   };
        }
    }
}