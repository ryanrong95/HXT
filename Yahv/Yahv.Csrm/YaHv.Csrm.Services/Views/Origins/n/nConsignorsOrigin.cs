using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class nConsignorsOrigin : Yahv.Linq.UniqueView<nConsignor, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal nConsignorsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal nConsignorsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<nConsignor> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nConsignors>()
                   select new nConsignor()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       nSupplierID = entity.nSupplierID,
                       EnterpriseID = entity.EnterpriseID,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Contact = entity.Contact,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       //Creator =entity.Creator,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       Place = entity.Place,
                       IsDefault = entity.IsDefault
                   };

        }
    }
}
