using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class AdminsInfoTopView<TReponsitory> : UniqueView<AdminInfo, TReponsitory> // PvbErmReponsitory
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsInfoTopView()
        {
            //System.Data.DataSet
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsInfoTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AdminInfo> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.AdminsInfoTopView>()
                   select new AdminInfo
                   {
                       ID = entity.ID,
                       Status = (AdminStatus)entity.Status,
                       RealName = entity.RealName,
                       StaffID = entity.StaffID,
                       SelCode = entity.SelCode,
                       UserName = entity.UserName,
                       LastLoginDate = entity.LastLoginDate,
                       RoleID = entity.RoleID,
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
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                   };
        }
    }
}
