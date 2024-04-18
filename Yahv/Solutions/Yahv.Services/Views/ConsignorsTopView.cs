using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 供应商发货地址通用视图
    /// </summary>
    /// <remarks>
    /// 需要引用PvbCrm下的视图： ConsignorsTopView,MapsBEnterTopView
    /// </remarks>
    public class ConsignorsTopView<TReponsitory> : UniqueView<Models.Consignor, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsignorsTopView()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsignorsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected Business? business;
        public ConsignorsTopView(Business business)
        {
            this.business = business;
        }
        public ConsignorsTopView(Business business, TReponsitory reponsitory) : base(reponsitory)
        {
            this.business = business;
        }

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            if (business.HasValue)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                       where map.Bussiness == (int)business && map.Type == (int)MapsType.Consignor
                       select map;
            }
            return null;
        }


        IQueryable<Consignor> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ConsignorsTopView>()
                   join enterprise in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.EnterprisesTopView>() on entity.EnterpriseID equals enterprise.ID
                   select new Consignor
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Title = entity.Title,
                       DyjCode = entity.DyjCode,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Status = (ApprovalStatus)entity.Status,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Enterprise = new Enterprise
                       {
                           ID = enterprise.ID,
                           Name = enterprise.Name,
                           RegAddress = enterprise.RegAddress,
                           Uscc = enterprise.Uscc,
                           Corporation = enterprise.Corporation,
                           Place = enterprise.Place,
                       }
                   };
        }
        protected override IQueryable<Consignor> GetIQueryable()
        {
            var mapsView = this.GetMapIQueryable();
            if (mapsView == null)
            {
                return this.getIQueryable();
            }
            else
            {
                return from entity in this.getIQueryable()
                       join m in mapsView on entity.ID equals m.SubID
                       select new Consignor
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Title = entity.Title,
                           DyjCode = entity.DyjCode,
                           Address = entity.Address,
                           Postzip = entity.Postzip,
                           Status = (ApprovalStatus)entity.Status,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Enterprise = entity.Enterprise,
                           ClientID = m.EnterpriseID,//关系表中获取
                           IsDefault = m.IsDefault
                       };
            }

        }

    }
}


