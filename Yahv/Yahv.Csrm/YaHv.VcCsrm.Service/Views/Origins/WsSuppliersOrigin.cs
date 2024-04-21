using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class WsSuppliersOrigin : Yahv.Linq.UniqueView<WsSupplier, PvcCrmReponsitory>
    {
        internal WsSuppliersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsSuppliersOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.WsSuppliers>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   
                   select new WsSupplier
                   {
                       ID = entity.ID,
                       Grade = (Yahv.Underly.SupplierGrade)entity.Grade,
                       WsSupplierStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
                       Summary = entity.Summary,
                       CreatorID = entity.AdminID,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       Origin = entity.Origin,
                       ShipID = entity.ShipID
                   };
        }
    }
}
