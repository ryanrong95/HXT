using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    public class OrderItemOrigin
    {
        public void Insert(OrderItemOriginModel[] models)
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                repository.Insert(models.Select(item => new Layers.Data.Sqls.PsOrder.OrderItems
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    ProductID = item.ProductID,
                    Supplier = item.Supplier,
                    Origin = item.Origin,
                    CustomCode = item.CustomCode,
                    StocktakingType = (int)item.StocktakingType,
                    Mpq = item.Mpq,
                    PackageNumber = item.PackageNumber,
                    Total = item.Total,
                    Currency = item.Currency,
                    UnitPrice = item.UnitPrice,
                    StorageID = item.StorageID,
                    CreateDate = item.CreateDate,
                    ModifyDate = item.ModifyDate,
                    Status = (int)item.Status,
                    BakPartnumber = item.BakPartnumber,
                    BakBrand = item.BakBrand,
                    BakPackage = item.BakPackage,
                    BakDateCode = item.BakDateCode,
                    NoticeID = item.NoticeID,
                    NoticeItemID = item.NoticeItemID,
                    InputID = item.InputID,
                }));
            }
        }

        public void Update(OrderItemOriginModel model)
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                repository.Update<Layers.Data.Sqls.PsOrder.OrderItems>(new
                {
                    ID = model.ID,
                    OrderID = model.OrderID,
                    ProductID = model.ProductID,
                    Supplier = model.Supplier,
                    Origin = model.Origin,
                    CustomCode = model.CustomCode,
                    StocktakingType = (int)model.StocktakingType,
                    Mpq = model.Mpq,
                    PackageNumber = model.PackageNumber,
                    Total = model.Total,
                    Currency = model.Currency,
                    UnitPrice = model.UnitPrice,
                    StorageID = model.StorageID,
                    CreateDate = model.CreateDate,
                    ModifyDate = model.ModifyDate,
                    Status = (int)model.Status,
                    BakPartnumber = model.BakPartnumber,
                    BakBrand = model.BakBrand,
                    BakPackage = model.BakPackage,
                    BakDateCode = model.BakDateCode,
                    NoticeID = model.NoticeID,
                    NoticeItemID = model.NoticeItemID,
                    InputID = model.InputID,
                }, item => item.ID == model.ID);

                repository.Update<Layers.Data.Sqls.PsOrder.Products>(new
                {
                    Partnumber = model.BakPartnumber,
                    Brand = model.BakBrand,
                    Package = model.BakPackage,
                    DateCode = model.BakDateCode,
                    Mpq = model.Mpq,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == model.ProductID);
            }
        }

        public void Delete(string[] orderItemIDs)
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                repository.Update<Layers.Data.Sqls.PsOrder.OrderItems>(new
                {
                    Status = (int)GeneralStatus.Closed,
                }, item => orderItemIDs.Contains(item.ID));
            }
        }

        public OrderItemOriginModel[] Query(string orderID)
        {
            OrderItemOriginModel[] orderItemOriginModels;

            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                orderItemOriginModels = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>()
                    .Where(t => t.OrderID == orderID && t.Status == (int)GeneralStatus.Normal)
                    .Select(item => new OrderItemOriginModel
                    {
                        ID = item.ID,
                        OrderID = item.OrderID,
                        ProductID = item.ProductID,
                        Supplier = item.Supplier,
                        Origin = item.Origin,
                        CustomCode = item.CustomCode,
                        StocktakingType = (StocktakingType)item.StocktakingType,
                        Mpq = item.Mpq,
                        PackageNumber = item.PackageNumber,
                        Total = item.Total,
                        Currency = item.Currency,
                        UnitPrice = item.UnitPrice,
                        StorageID = item.StorageID,
                        CreateDate = item.CreateDate,
                        ModifyDate = item.ModifyDate,
                        Status = (GeneralStatus)item.Status,
                        BakPartnumber = item.BakPartnumber,
                        BakBrand = item.BakBrand,
                        BakPackage = item.BakPackage,
                        BakDateCode = item.BakDateCode,
                        NoticeID = item.NoticeID,
                        NoticeItemID = item.NoticeItemID,
                        InputID = item.InputID,
                    }).ToArray();
            }

            return orderItemOriginModels;
        }

        public void InsertProducts(ProductModel[] models)
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                repository.Insert(models.Select(item => new Layers.Data.Sqls.PsOrder.Products
                {
                    ID = item.ID,
                    Partnumber = item.Partnumber,
                    Brand = item.Brand,
                    Package = item.Package,
                    DateCode = item.DateCode,
                    Mpq = item.Mpq,
                    Moq = item.Moq,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                }).ToArray());
            }
        }
    }

    public class OrderItemOriginModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BakPartnumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BakBrand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BakPackage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BakDateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NoticeItemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InputID { get; set; }
    }

    public class ProductModel
    {
        public string ID { get; set; }

        public string Partnumber { get; set; }

        public string Brand { get; set; }

        public string Package { get; set; }

        public string DateCode { get; set; }

        public int? Mpq { get; set; }

        public int? Moq { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }
    }
}
