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
    /// <summary>
    /// 代仓储供应商
    /// </summary>
    public class WsSuppliersOrigins : Yahv.Linq.UniqueView<WsSupplier, PvbCrmReponsitory>
    {
        internal WsSuppliersOrigins()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsSuppliersOrigins(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new WsSupplier
                   {
                       ID = entity.ID,
                       Grade = (Yahv.Underly.SupplierGrade)entity.Grade,
                       WsSupplierStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
                       Summary = entity.Summary,
                       Enterprise = enterprises,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       Place = entity.Place
                   };
        }
    }

    /// <summary>
    /// 关系表中的代仓储供应商
    /// </summary>
    public class XdtWsSuppliersOrigins : Yahv.Linq.UniqueView<XdtWsSupplier, PvbCrmReponsitory>
    {

        internal XdtWsSuppliersOrigins()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal XdtWsSuppliersOrigins(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<XdtWsSupplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on entity.ID equals maps.SubID
                   join clientEnterprise in enterprisesView on maps.EnterpriseID equals clientEnterprise.ID
                   where maps.Type == (int)MapsType.WsSupplier && maps.Bussiness == (int)Business.WarehouseServicing

                   select new XdtWsSupplier
                   {
                       ID = entity.ID,
                       Grade = (Yahv.Underly.SupplierGrade)entity.Grade,
                       WsSupplierStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
                       Summary = entity.Summary,
                       Enterprise = enterprises,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       WsClient = clientEnterprise,
                       Place = entity.Place
                   };
        }
    }
}
