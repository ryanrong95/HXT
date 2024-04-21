using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class ConsignorsOrigin : Yahv.Linq.UniqueView<Consignor, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ConsignorsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ConsignorsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Consignor> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Consignors>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Consignor()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       EnterpriseID = entity.EnterpriseID,
                       DyjCode = entity.DyjCode,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprise,
                       CreatorID = admin.ID,
                       Admin = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                   };

        }
    }
    /// <summary>
    /// 代仓储的提货地址
    /// </summary>
    public class WsConsignorsOrigin : Yahv.Linq.UniqueView<WsConsignor, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal WsConsignorsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsConsignorsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsConsignor> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Consignors>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   join map in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on entity.ID equals map.SubID
                   join clientEnterprise in enterpriseView on map.EnterpriseID equals clientEnterprise.ID
                   where map.Type == (int)MapsType.Consignor
                  && map.Bussiness == (int)Business.WarehouseServicing
                   select new WsConsignor()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       EnterpriseID = entity.EnterpriseID,
                       DyjCode = entity.DyjCode,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprise,
                       CreatorID= admin.ID,
                       Admin = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       IsDefault = map.IsDefault,
                       WsClient = clientEnterprise
                   };

        }
    }
}
