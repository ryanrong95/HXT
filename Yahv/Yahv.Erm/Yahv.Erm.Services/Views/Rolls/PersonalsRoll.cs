using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工个人信息
    /// </summary>
    public class PersonalsRoll : UniqueView<Personal, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalsRoll()
        {
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Personal> GetIQueryable()
        {
            var staffView = new Origins.StaffsOrigin(this.Reponsitory);
            var PersonalsView = new Origins.PersonalsOrigin(this.Reponsitory);
            return from entity in PersonalsView
                   join staff in staffView on entity.ID equals staff.ID
                   where staff.Status != StaffStatus.Delete
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
}