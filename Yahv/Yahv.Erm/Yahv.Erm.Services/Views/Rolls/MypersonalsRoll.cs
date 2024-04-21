using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工个人信息
    /// </summary>
    public class MypersonalsRoll : UniqueView<Personal, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MypersonalsRoll()
        {
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Personal> GetIQueryable()
        {
            var PersonalsView = new Origins.PersonalsOrigin(this.Reponsitory);
            return from personal in PersonalsView
                   select new Personal()
                   {
                       ID = personal.ID,
                       Weight = personal.Weight,
                       Height = personal.Height,
                       IsMarry = personal.IsMarry,
                       PassAddress = personal.PassAddress,
                       GraduatInstitutions = personal.GraduatInstitutions,
                       Volk = personal.Volk,
                       Major = personal.Major,
                       Education = personal.Education,
                       IDCard = personal.IDCard,
                       PoliticalOutlook = personal.PoliticalOutlook,
                       HomeAddress = personal.HomeAddress,
                       NativePlace = personal.NativePlace,
                       Image = personal.Image,
                       Blood = personal.Blood,
                   };
        }
    }
}