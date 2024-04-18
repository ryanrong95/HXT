using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 收货地址通用视图
    /// </summary>
    /// <remarks>
    /// 需要引用PvbCrm下的视图： ConsigneesTopView,MapsBEnterTopView
    /// </remarks>
    public class ConsigneesTopView<TReponsitory> : UniqueView<Consignee, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsigneesTopView()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsigneesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected Business? business;
        public ConsigneesTopView(Business business)
        {
            this.business = business;
        }
        public ConsigneesTopView(Business business, TReponsitory reponsitory) : base(reponsitory)
        {
            this.business = business;
        }

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            if (business.HasValue)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                       where map.Bussiness == (int)business && map.Type == (int)MapsType.Consignee
                       select map;
            }
            return null;
        }


        IQueryable<Consignee> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ConsigneesTopView>()
                   join enterprise in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.EnterprisesTopView>() on entity.EnterpriseID equals enterprise.ID
                   select new Consignee
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Title = entity.Title,
                       DyjCode = entity.DyjCode,
                       District = (District)entity.District,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Status = (ApprovalStatus)entity.Status,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Place = entity.Place,//代仓储以Place为准，其他业务暂时参考District
                       Enterprise = new Enterprise
                       {
                           ID = enterprise.ID,
                           Name = enterprise.Name,
                           RegAddress = enterprise.RegAddress,
                           Uscc = enterprise.Uscc,
                           Corporation = enterprise.Corporation,
                           Place = enterprise.Place
                           //District = enterprise.District,
                       }
                   };
        }
        protected override IQueryable<Consignee> GetIQueryable()
        {
            var mapsView = this.GetMapIQueryable();
            if (mapsView == null)
            {
                var a = this.getIQueryable();
                return a;
            }
            else
            {
                var b = from entity in this.getIQueryable()
                        join m in mapsView on entity.ID equals m.SubID
                        select new Consignee
                        {
                            ID = entity.ID,
                            EnterpriseID = entity.EnterpriseID,
                            Title = entity.Title,
                            DyjCode = entity.DyjCode,
                            District = (District)entity.District,
                            Address = entity.Address,
                            Postzip = entity.Postzip,
                            Status = (ApprovalStatus)entity.Status,
                            Name = entity.Name,
                            Tel = entity.Tel,
                            Mobile = entity.Mobile,
                            Email = entity.Email,
                            IsDefault = m.IsDefault,
                            Enterprise = entity.Enterprise,
                            Place = entity.Place//代仓储以Place为准，其他业务暂时参考District

                        };
                return b;
            }

        }

    }
}
