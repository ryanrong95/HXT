using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class ClassifyProductExtends
    {
        /// <summary>
        /// 产品管控类型 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="model">型号</param>
        /// <returns></returns>
        public static Enums.ItemCategoryType ControlType(this Interfaces.IProduct product, string model)
        {
            ProductControlsView view = new ProductControlsView(model);
            var productControls = view.ToList();

            var type = Enums.ItemCategoryType.Normal;

            foreach (var item in productControls)
            {
                if (item.Type == Enums.ProductControlType.CCC)
                {
                    type |= Enums.ItemCategoryType.CCC;
                }
                if (item.Type == Enums.ProductControlType.Forbid)
                {
                    type |= Enums.ItemCategoryType.Forbid;
                }
            }

            return type;
        }

        /// <summary>
        /// 填充自动归类信息
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="categoryDefault"></param>
        /// <returns></returns>
        public static void Supplement(this ClassifyProduct classifyProduct, ProductCategoriesDefault categoryDefault)
        {
            //海关归类
            classifyProduct.Category = new OrderItemCategory();
            classifyProduct.Category.OrderItemID = classifyProduct.ID;
            //classifyProduct.Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);\
            classifyProduct.Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
            classifyProduct.Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
            classifyProduct.Category.TaxCode = categoryDefault.TaxCode;
            classifyProduct.Category.TaxName = categoryDefault.TaxName;
            classifyProduct.Category.HSCode = categoryDefault.HSCode;
            classifyProduct.Category.Name = categoryDefault.ProductName;
            classifyProduct.Category.Elements = categoryDefault.Elements;
            classifyProduct.Category.Unit1 = categoryDefault.Unit1;
            classifyProduct.Category.Unit2 = categoryDefault.Unit2;
            classifyProduct.Category.CIQCode = categoryDefault.CIQCode;
            classifyProduct.Category.Summary = categoryDefault.Summary;
            classifyProduct.Category.Type = categoryDefault.Type.GetValueOrDefault();
            if ((classifyProduct.Category.Type & Enums.ItemCategoryType.Inspection) > 0)
            {
                classifyProduct.IsInsp = true;
                classifyProduct.InspectionFee = categoryDefault.InspectionFee;
            }

            //关税
            classifyProduct.ImportTax = new OrderItemTax();
            classifyProduct.ImportTax.OrderItemID = classifyProduct.ID;
            classifyProduct.ImportTax.Type = Enums.CustomsRateType.ImportTax;
            classifyProduct.ImportTax.Rate = categoryDefault.TariffRate.GetValueOrDefault()/100;

            //增值税
            classifyProduct.AddedValueTax = new OrderItemTax();
            classifyProduct.AddedValueTax.OrderItemID = classifyProduct.ID;
            classifyProduct.AddedValueTax.Type = Enums.CustomsRateType.AddedValueTax;
            classifyProduct.AddedValueTax.Rate = categoryDefault.AddedValueRate.GetValueOrDefault()/100;
        }
    }
}