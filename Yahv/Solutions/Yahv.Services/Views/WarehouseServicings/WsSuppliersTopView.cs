using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 已被wsnSuppliersTopView代替
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    [Obsolete]
    public class WsSuppliersTopView<TReponsitory> : UniqueView<WsSupplier, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsSuppliersTopView()
        {

        }
        public WsSuppliersTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Underly.Business.WarehouseServicing && map.Type == (int)Underly.MapsType.WsSupplier
                   select map;
        }


        IQueryable<WsSupplier> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsSuppliersTopView>()
                   select new WsSupplier
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       Corporation = entity.Corporation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Grade = (Underly.SupplierGrade)entity.Grade,
                       Status = (Underly.GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                   };
        }
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            return from entity in this.getIQueryable()
                   join map in this.GetMapIQueryable() on entity.ID equals map.SubID
                   select new WsSupplier
                   {
                       ID = entity.ID,
                       ClientID = map.EnterpriseID,//华芯通客户,关系表中获取
                       Name = entity.Name,
                       ChineseName = entity.ChineseName,
                       EnglishName = entity.EnglishName,
                       Corporation = entity.Corporation,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Grade = entity.Grade,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                   };

        }
    }
}
