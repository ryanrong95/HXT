using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 客户
    /// </summary>
    public class ClientsAll<TReponsitory> : UniqueView<Client, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ClientsAll()
        {

        }
        public ClientsAll(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return null;
        }


        IQueryable<Client> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<ClientsTopView>()
                   select new Client
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Nature = (ClientType)entity.Nature,//客户性质
                       AreaType = (AreaType)entity.AreaType,//客户地区类型
                       Grade = (ClientGrade)entity.Grade,
                       Status = (ApprovalStatus)entity.Status,
                       TaxperNumber = entity.TaxperNumber,
                       District = entity.District,//废弃
                       Vip = (VIPLevel)entity.Vip,
                       DyjCode = entity.DyjCode
                   };
        }
        protected override IQueryable<Client> GetIQueryable()
        {
            var mapView = this.GetMapIQueryable();

            if (mapView == null)
            {
                return this.getIQueryable();
            }
            else
            {
                return from entity in this.getIQueryable()
                       join map in mapView on entity.ID equals map.EnterpriseID
                       select entity;
            }

        }
    }
}
