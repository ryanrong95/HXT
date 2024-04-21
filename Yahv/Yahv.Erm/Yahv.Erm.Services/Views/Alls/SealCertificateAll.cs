using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Alls
{
    public class SealCertificateAll : UniqueView<SealCertificate, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SealCertificateAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public SealCertificateAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<SealCertificate> GetIQueryable()
        {
            var originView = new SealCertificateOrigin(this.Reponsitory);
            var staffView = new StaffsOrigin(this.Reponsitory);
            var adminView = new AdminsOrigin(this.Reponsitory);

            return from entity in originView
                   join admin in adminView on entity.AdminID equals admin.ID
                   join staff in staffView on entity.StaffID equals staff.ID into staffs
                   from staff in staffs.DefaultIfEmpty()
                   where entity.Status == Underly.GeneralStatus.Normal
                   select new SealCertificate()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Name = entity.Name,
                       Type = (SealCertificateType)entity.Type,
                       ProcessingDate = entity.ProcessingDate,
                       DueDate = entity.DueDate,
                       AdminID = entity.AdminID,
                       StaffID = entity.StaffID,

                       Admin = admin,
                       Staff = staff,
                   };
        }
    }
}
