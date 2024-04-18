using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticeItems_Show_View : UniqueView<NoticeItem_Out_Show, PsWmsRepository>
    {
        #region 构造函数
        public NoticeItems_Show_View()
        {
        }

        public NoticeItems_Show_View(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public NoticeItems_Show_View(PsWmsRepository reponsitory, IQueryable<NoticeItem_Out_Show> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        #endregion

        protected override IQueryable<NoticeItem_Out_Show> GetIQueryable()
        {
            var items = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>();
            var products = new ProductsView(this.Reponsitory);
            var storages = new StoragesOriginView(this.Reponsitory);
            var shelveView = new ShelvesView(this.Reponsitory);

            var view = from entity in items
                       join product in products on entity.ProductID equals product.ID
                       join storage in storages on entity.StorageID equals storage.ID
                       join shelve in shelveView on storage.ShelveID equals shelve.ID into shelves
                       from shelve in shelves.DefaultIfEmpty()
                       select new NoticeItem_Out_Show
                       {
                           ID = entity.ID,
                           NoticeID = entity.NoticeID,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           FormID = entity.FormID,
                           FormItemID = entity.FormItemID,
                           Exception = entity.Exception,
                           StorageID = entity.StorageID,
                           Shelve = shelve,
                           Unique = storage.Unique,

                           Partnumber = product.Partnumber,
                           Brand = product.Brand,
                           Package = product.Package,
                           DateCode = product.DateCode,
                       };

            return view;
        }

    }

    public class NoticeItem_Out_Show : IUnique
    {
        public string ID { get; set; }
        public string NoticeID { get; set; }
        public string FormID { get; set; }
        public string FormItemID { get; set; }
        public string Package { get; set; }
        public string Brand { get; set; }
        public string Partnumber { get; set; }
        public string DateCode { get; set; }
        public int Mpq { get; set; }
        public int PackageNumber { get; set; }
        public int Total { get; set; }
        public string Exception { get; set; }

        /// <summary>
        /// 库存id
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public Shelve Shelve { get; set; }

        public string ShelveCode
        {
            get
            {
                return Shelve?.Code;
            }
        }

        public string Unique { get; set; }
    }
}
