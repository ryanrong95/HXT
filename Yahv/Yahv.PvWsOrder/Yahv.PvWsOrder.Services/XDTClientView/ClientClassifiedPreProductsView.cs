using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels.Client;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 客户的预归类产品
    /// 包含归类信息
    /// </summary>
    public class ClientClassifiedPreProductsView : UniqueView<ClassifiedPreProduct, ScCustomReponsitory>
    {
        IUser User;

        public ClientClassifiedPreProductsView(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<ClassifiedPreProduct> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProducts>()
                   join category in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProductCategories>() on product.ID equals category.PreProductID
                   where product.ClientID == this.User.XDTClientID && product.Status == (int)GeneralStatus.Normal
                   orderby product.CreateDate descending
                   select new ClassifiedPreProduct
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       ProductName = category.ProductName,
                       ProductUnionCode = product.ProductUnionCode,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       BatchNo = product.BatchNo,
                       Price = product.Price,
                       Currency = product.Currency,
                       Supplier = product.Supplier,
                       Qty = product.Qty,
                       DueDate = product.DueDate,
                       Status = (int)product.Status,
                       CreateDate = product.CreateDate,
                       UpdateDate = product.UpdateDate,
                       ClassifyStatusInt = category.ClassifyStatus,
                   };
        }

        /// <summary>
        /// 根据传入参数获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<ClassifiedPreProduct> GetExpressionData(LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    linq = linq.Where(expression as Expression<Func<ClassifiedPreProduct, bool>>);
                }
            }
            return linq;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<ClassifiedPreProduct> GetPageList(LambdaExpression[] expressions, int PageSize, int PageIndex)
        {
            //根据查询条件过滤
            var data = this.GetExpressionData(expressions);
            int total = data.Count();
            var products = data.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            var classifies = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PreProductCategories>().Where(item => products.Select(a => a.ID).Contains(item.PreProductID)).ToArray();

            var results = from product in products
                          join classify in classifies on product.ID equals classify.PreProductID into classifProducts
                          from classifProduct in classifProducts.DefaultIfEmpty()
                          select new ClassifiedPreProduct
                          {
                              ID = product.ID,
                              ClientID = product.ClientID,
                              ProductUnionCode = product.ProductUnionCode,
                              Model = product.Model,
                              Manufacturer = product.Manufacturer,
                              BatchNo = product.BatchNo,
                              Price = product.Price,
                              Currency = product.Currency,
                              Supplier = product.Supplier,
                              Qty = product.Qty,
                              DueDate = product.DueDate,
                              ProductName = classifProduct == null ? "" : classifProduct.ProductName,
                              HSCode = classifProduct == null ? "" : classifProduct.HSCode,
                              TariffRate = classifProduct == null ? null : classifProduct.TariffRate,
                              AddedValueRate = classifProduct == null ? null : classifProduct.AddedValueRate,
                              TaxCode = classifProduct == null ? "" : classifProduct.TaxCode,
                              TaxName = classifProduct == null ? "" : classifProduct.TaxName,
                              Type = classifProduct == null ? null : (ItemCategoryType?)classifProduct.Type,
                              ClassifyStatus = classifProduct == null ? ClassifyStatus.Unclassified : (ClassifyStatus?)classifProduct.ClassifyStatus,
                              InspectionFee = classifProduct == null ? null : classifProduct.InspectionFee,
                              Unit1 = classifProduct == null ? "" : classifProduct.Unit1,
                              Unit2 = classifProduct == null ? "" : classifProduct.Unit2,
                              CIQCode = classifProduct == null ? "" : classifProduct.CIQCode,
                              Elements = classifProduct == null ? "" : classifProduct.Elements,
                              Summary = classifProduct == null ? "" : classifProduct.Summary,

                              Status = (int)product.Status,
                              CreateDate = product.CreateDate,
                              UpdateDate = product.UpdateDate
                          };

            return new PageList<ClassifiedPreProduct>(PageIndex, PageSize, results, total);
        }
    }

    public class ClassifiedPreProduct : PreProduct
    {
        //public string ProductName { get; set; }

        public string HSCode { get; set; }

        public decimal? TariffRate { get; set; }

        public decimal? AddedValueRate { get; set; }

        public string TaxCode { get; set; }

        public string TaxName { get; set; }

        public ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        public string Unit1 { get; set; }

        public string Unit2 { get; set; }

        public string CIQCode { get; set; }

        public string Elements { get; set; }

        public ClassifyStatus? ClassifyStatus { get; set; }

        public int? ClassifyStatusInt { get; set; }

        public string ClassifyFirstOperator { get; set; }

        public string ClassifySecondOperator { get; set; }
    }



    /// <summary>
    /// 归类状态
    /// </summary>
    public enum ClassifyStatus
    {
        /// <summary>
        /// 未归类
        /// </summary>
        [Description("未归类")]
        Unclassified,

        /// <summary>
        /// 首次归类
        /// </summary>
        [Description("首次归类")]
        First,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        Anomaly,

        /// <summary>
        /// 归类完成
        /// </summary>
        [Description("归类完成")]
        Done
    }

    /// <summary>
    /// 订单项归类类型
    /// </summary>
    [Flags]
    public enum ItemCategoryType
    {
        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Normal = 0,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 1,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 2,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginProof = 4,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 8,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 16,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 32,

        /// <summary>
        /// 香港管制
        /// </summary>
        [Description("香港管制")]
        HKForbid = 64,
    }
}
