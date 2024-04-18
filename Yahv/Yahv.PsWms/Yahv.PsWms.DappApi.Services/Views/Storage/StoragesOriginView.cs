using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    /// <summary>
    /// 库存视图
    /// </summary>
    public class StoragesOriginView : Linq.UniqueView<Storage, PsWmsRepository>
    {
        public StoragesOriginView()
        {
        }

        internal StoragesOriginView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal StoragesOriginView(PsWmsRepository reponsitory, IQueryable<Storage> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                       select new Storage
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           NoticeID = entity.NoticeID,
                           NoticeItemID = entity.NoticeItemID,
                           ProductID = entity.ProductID,
                           InputID = entity.InputID,
                           Type = (Enums.StorageType)entity.Type,
                           Islock = entity.Islock,
                           StocktakingType = (Enums.StocktakingType)entity.StocktakingType,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           SorterID = entity.SorterID,
                           FormID = entity.FormID,
                           FormItemID = entity.FormItemID,
                           Currency = (Underly.Currency)entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           ShelveID = entity.ShelveID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Exception = entity.Exception,
                           Summary = entity.Summary,
                           Unique = entity.Unique
                       };

            return view;
        }
    }
}
