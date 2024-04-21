using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 印章证照视图
    /// </summary>
    internal class SealCertificateOrigin : UniqueView<SealCertificate, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal SealCertificateOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal SealCertificateOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<SealCertificate> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.SealCertificates>()
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
                       Status = (GeneralStatus)entity.Status,
                   };
        }
    }
}
