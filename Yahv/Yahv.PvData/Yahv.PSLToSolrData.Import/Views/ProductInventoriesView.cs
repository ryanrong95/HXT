using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PSLToSolrData.Import.Models;

namespace Yahv.PSLToSolrData.Import.Views
{
    /// <summary>
    /// 产品库存视图
    /// </summary>
    public class ProductInventoriesView : UniqueView<Models.ProductInventory, Linqs.PSLReponsitory>
    {
        internal ProductInventoriesView()
        {
        }

        internal ProductInventoriesView(Linqs.PSLReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProductInventory> GetIQueryable()
        {
            return from pi in Reponsitory.ReadTable<Linqs.ProductInventories>()
                   join pd in Reponsitory.ReadTable<Linqs.ProductDeliveries>() on pi.ID equals pd.ID
                   join pq in Reponsitory.ReadTable<Linqs.ProductQuotes>() on pi.ID equals pq.InventoryID into pqs
                   select new Models.ProductInventory()
                   {
                       ID = pi.ID,
                       ProductID = pi.ProductID,
                       Supplier = pi.Supplier,
                       Storage = pi.Storage,
                       DateCode = pi.DateCode,
                       Packaging = pi.Packaging,
                       Sign = pi.Sign,
                       CreateDate = pi.CreateDate,
                       ModifyDate = pi.ModifyDate,
                       Quantity = pi.Quantity,
                       Status = pi.Status,

                       //产品交货条件
                       ProductDelivery = new ProductDelivery()
                       {
                           ID= pd.ID,
                           Moq = pd.Moq,
                           Jump = pd.Jump,
                           Mpa = pd.Mpa,
                           MpaCurrency = pd.MpaCurrency,
                           Delivery = pd.Delivery,
                           CreateDate = pd.CreateDate,
                           ModifyDate = pd.ModifyDate
                       },

                       //产品报价
                       ProductQuotes = pqs.Select(item => new Models.ProductQuote()
                       {
                           ID = item.ID,
                           InventoryID = item.InventoryID,
                           Moq = item.Moq,
                           Currency = item.Currency,
                           UnitPrice = item.UnitPrice,
                           CreateDate = item.CreateDate,
                           IsCurrent = item.IsCurrent,
                           Type = item.Type
                       }).ToArray()
                   };
        }
    }
}
