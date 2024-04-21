using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class ConsigneesOrigin : Yahv.Linq.UniqueView<Consignee, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ConsigneesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ConsigneesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Consignee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Consignees>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Consignee()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       EnterpriseID = entity.EnterpriseID,
                       DyjCode = entity.DyjCode,
                       District = (District)entity.District,
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
                       Creator = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       PlateCode = entity.PlateCode
                   };

        }
    }
    /// <summary>
    /// 代仓储业务的到货地址
    /// </summary>
    public class WsConsigneesOrigin : Yahv.Linq.UniqueView<WsConsignee, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal WsConsigneesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsConsigneesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsConsignee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Consignees>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID

                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   join map in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on entity.ID equals map.SubID
                   where map.Bussiness == (int)Business.WarehouseServicing
                   select new WsConsignee()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       EnterpriseID = entity.EnterpriseID,
                       DyjCode = entity.DyjCode,
                       District = (District)entity.District,
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
                       Creator = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       IsDefault = map.IsDefault,
                       Place=entity.Place
                   };

        }
    }

    /// <summary>
    /// 不同业务下客户与到货地址的关系数据
    /// </summary>
    public class ServiceConsigneesOrigin : Yahv.Linq.UniqueView<TradingConsignee, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ServiceConsigneesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ServiceConsigneesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingConsignee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Bussiness == (int)Business.Trading && item.Type == (int)MapsType.Consignee);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Consignees>()

                       join map in mapsView on entity.ID equals map.SubID

                       join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID

                       join admin in adminsView on map.CtreatorID equals admin.ID into _admin
                       from admin in _admin.DefaultIfEmpty()


                       select new TradingConsignee()
                       {
                           ID = entity.ID,
                           PlateCode = entity.PlateCode,//库房门牌编码
                           Title = entity.Title,
                           EnterpriseID = entity.EnterpriseID,
                           DyjCode = entity.DyjCode,
                           District = (District)entity.District,
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
                           Creator = admin,
                           Province = entity.Province,
                           City = entity.City,
                           Land = entity.Land,
                           IsDefault = map.IsDefault
                       };
            return linq;

        }
    }
}
