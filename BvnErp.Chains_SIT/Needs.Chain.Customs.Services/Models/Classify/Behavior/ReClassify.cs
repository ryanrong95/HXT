using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Interfaces;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品变更/重新归类
    /// </summary>
    public class ReClassify : Classify
    {
        public ReClassify(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 重新归类完成之后需要做的操作
         * 1.写产品归类日志
         * 2.写产品归类信息变更日志
         * 3.同步订单特殊类型(OrderVoyage)
         * 4.更新产品变更通知的处理状态
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (ClassifyProduct)e.Product;

                WriteProductClassifyLog(classifyProduct, reponsitory);
                WriteProductClassifyChangeLog(classifyProduct, reponsitory);
                SetOrderVoyage(classifyProduct, reponsitory);
                UpdateOrderItemChangeNotice(classifyProduct, reponsitory);
            }
        }

        /** 重新归类异常之后需要做的操作
         * 1.写产品归类异常日志
         * 
         * 冗余：正常到重新归类阶段了，应该不可能再点归类异常了
         */
        private void Product_Anomalied(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (ClassifyProduct)e.Product;

                WriteProductClassifyAnomalyLog(classifyProduct, reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = ClassifyStatus.Done;
            this.Product.ClassifyStep = ClassifyStep.ReClassify;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.ClassifyStep = ClassifyStep.ReClassify;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.ClassifyStep = ClassifyStep.ReClassify;
            base.UnLock();
        }

        public override void Anomaly()
        {
            this.Product.ClassifyStatus = ClassifyStatus.Anomaly;
            this.Product.ClassifyStep = ClassifyStep.ReClassify;
            base.Anomaly();
        }

        //================================================= Begin ===================================================

        private void WriteProductClassifyLog(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var declarant = classifyProduct.Admin;
            if (declarant != null)
            {
                string categoryType = null;
                foreach (Enums.ItemCategoryType type in Enum.GetValues(typeof(Enums.ItemCategoryType)))
                {
                    if ((classifyProduct.Category.Type & type) > 0)
                    {
                        categoryType += type.GetDescription() + " ";
                    }
                }
                if (categoryType == null)
                {
                    categoryType = Enums.ItemCategoryType.Normal.GetDescription();
                }

                string summary = "报关员【" + declarant.ByName + "】完成了预处理二。归类结果:" +
                        " 品牌【" + classifyProduct.Manufacturer +
                        "】; 型号【" + classifyProduct.Model +
                        "】; 单价【" + classifyProduct.UnitPrice +
                        "】; 交易币种【" + classifyProduct.Currency +
                        "】; 海关编码【" + classifyProduct.Category.HSCode +
                        "】; 报关品名【" + classifyProduct.Category.Name +
                        "】; 类型【" + categoryType +
                        "】; 商检费用【" + classifyProduct.InspectionFee +
                        "】; 税务编码【" + classifyProduct.Category.TaxCode +
                        "】; 税务名称【" + classifyProduct.Category.TaxName +
                        "】; 关税率【" + classifyProduct.ImportTax.Rate +
                        "】; 增值税率【" + classifyProduct.AddedValueTax.Rate +
                        "】; 法一单位【" + classifyProduct.Category.Unit1 +
                        "】; 法二单位【" + classifyProduct.Category.Unit2 +
                        "】; 检验检疫编码【" + classifyProduct.Category.CIQCode +
                        "】; 申报要素【" + classifyProduct.Category.Elements +
                        "】; 摘要备注【" + classifyProduct.Category.Summary + "】";
                classifyProduct.Log(Enums.LogTypeEnums.Classify, summary);
            }
        }

        private void WriteProductClassifyChangeLog(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var model = classifyProduct.Model;
            var inspFee = classifyProduct.InspectionFee;
            var catDefault = new Views.ProductCategoriesDefaultsView(reponsitory).Where(item => item.Model == model).FirstOrDefault();

            if (catDefault == null)
            {
                #region 新增自动归类记录

                catDefault = new ProductCategoriesDefault();
                catDefault.Model = classifyProduct.Model;
                catDefault.Manufacturer = classifyProduct.Manufacturer;
                catDefault.ProductName = classifyProduct.Category.Name;
                catDefault.HSCode = classifyProduct.Category.HSCode;
                catDefault.TariffRate = classifyProduct.ImportTax.Rate;
                catDefault.AddedValueRate = classifyProduct.AddedValueTax.Rate;
                catDefault.TaxCode = classifyProduct.Category.TaxCode;
                catDefault.TaxName = classifyProduct.Category.TaxName;
                catDefault.Type = classifyProduct.Category.Type;
                catDefault.InspectionFee = inspFee;
                catDefault.Unit1 = classifyProduct.Category.Unit1;
                catDefault.Unit2 = classifyProduct.Category.Unit2;
                catDefault.CIQCode = classifyProduct.Category.CIQCode;
                catDefault.Elements = classifyProduct.Category.Elements;
                reponsitory.Insert(catDefault.ToLinq());

                #endregion
            }
            else
            {
                var declarant = classifyProduct.Admin;

                #region 产品归类变更日志

                if (classifyProduct.Manufacturer != catDefault.Manufacturer)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的品牌由【" + catDefault.Manufacturer + "】修改为【" + classifyProduct.Manufacturer + "】");
                }
                if (classifyProduct.Category.Name != catDefault.ProductName)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的报关品名由【" + catDefault.ProductName + "】修改为【" + classifyProduct.Category.Name + "】");
                }
                if (classifyProduct.Category.HSCode != catDefault.HSCode)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的海关编码由【" + catDefault.HSCode + "】修改为【" + classifyProduct.Category.HSCode + "】");
                }
                if (classifyProduct.ImportTax.Rate != catDefault.TariffRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的关税率由【" + catDefault.TariffRate / 100 + "】修改为【" + classifyProduct.ImportTax.Rate + "】");
                }
                if (classifyProduct.AddedValueTax.Rate != catDefault.AddedValueRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的增值税率由【" + catDefault.AddedValueRate / 100 + "】修改为【" + classifyProduct.AddedValueTax.Rate + "】");
                }
                if (classifyProduct.Category.TaxCode != catDefault.TaxCode)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的税务编码由【" + catDefault.TaxCode + "】修改为【" + classifyProduct.Category.TaxCode + "】");
                }
                if (classifyProduct.Category.TaxName != catDefault.TaxName)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的税务名称由【" + catDefault.TaxName + "】修改为【" + classifyProduct.Category.TaxName + "】");
                }
                if (classifyProduct.Category.Unit1 != catDefault.Unit1)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的法定第一单位由【" + catDefault.Unit1 + "】修改为【" + classifyProduct.Category.Unit1 + "】");
                }
                if ((classifyProduct.Category.Unit2 ?? "") != (catDefault.Unit2 ?? ""))
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的法定第二单位由【" + catDefault.Unit2 + "】修改为【" + classifyProduct.Category.Unit2 + "】");
                }
                if (classifyProduct.Category.CIQCode != catDefault.CIQCode)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的检验检疫编码由【" + catDefault.CIQCode + "】修改为【" + classifyProduct.Category.CIQCode + "】");
                }
                if (classifyProduct.Category.Elements != catDefault.Elements)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的申报要素由【" + catDefault.Elements + "】修改为【" + classifyProduct.Category.Elements + "】");
                }

                bool isDefaultCCC = ((catDefault.Type & Enums.ItemCategoryType.CCC) > 0);
                bool isDefaultOriginProof = ((catDefault.Type & Enums.ItemCategoryType.OriginProof) > 0);
                bool isDefaultInsp = ((catDefault.Type & Enums.ItemCategoryType.Inspection) > 0);
                bool isDefaultHighValue = ((catDefault.Type & Enums.ItemCategoryType.HighValue) > 0);
                bool isDefaultForbidden = ((catDefault.Type & Enums.ItemCategoryType.Forbid) > 0);
                bool isDefaultHKForbidden = ((catDefault.Type & Enums.ItemCategoryType.HKForbid) > 0);

                if (classifyProduct.IsCCC != isDefaultCCC)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的CCC认证由【" + (isDefaultCCC ? "是" : "否") + "】修改为【" + (classifyProduct.IsCCC ? "是" : "否") + "】");
                }
                if (classifyProduct.IsOriginProof != isDefaultOriginProof)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的原产地证明由【" + (isDefaultOriginProof ? "是" : "否") + "】修改为【" + (classifyProduct.IsOriginProof ? "是" : "否") + "】");
                }
                if (classifyProduct.IsInsp != isDefaultInsp)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的是否商检由【" + (isDefaultInsp ? "是" : "否") + "】修改为【" + (classifyProduct.IsInsp ? "是" : "否") + "】");
                }
                if ((inspFee != null || catDefault.InspectionFee != null) && inspFee != catDefault.InspectionFee)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的商检费由【" + catDefault.InspectionFee + "】修改为【" + inspFee + "】");
                }
                if (classifyProduct.IsHighValue != isDefaultHighValue)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的是否高价值产品由【" + (isDefaultHighValue ? "是" : "否") + "】修改为【" + (classifyProduct.IsHighValue ? "是" : "否") + "】");
                }
                if (classifyProduct.IsForbid != isDefaultForbidden)
                {
                    classifyProduct.Log("报关员【" + declarant.ByName + "】将型号【" + model + "】的是否管控由【" + (isDefaultForbidden ? "是" : "否") + "】修改为【" + (classifyProduct.IsForbid ? "是" : "否") + "】");
                }

                #endregion

                #region 更新自动归类记录

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>(new
                {
                    //Model = classifyProduct.Product.Model,
                    Manufacture = classifyProduct.Manufacturer,
                    ProductName = classifyProduct.Category.Name,
                    HSCode = classifyProduct.Category.HSCode,
                    // TariffRate = classifyProduct.ImportTax.Rate,
                    // AddedValueRate = classifyProduct.AddedValueTax.Rate,
                    TaxCode = classifyProduct.Category.TaxCode,
                    TaxName = classifyProduct.Category.TaxName,
                    Type = (int?)classifyProduct.Category.Type,
                    //ClassifyType = (int?)catDefault.ClassifyType,
                    InspectionFee = inspFee,
                    // Unit1 = classifyProduct.Category.Unit1,
                    // Unit2 = classifyProduct.Category.Unit2,
                    CIQCode = classifyProduct.Category.CIQCode,
                    Elements = classifyProduct.Category.Elements,
                    UpdateDate = DateTime.Now
                }, item => item.ID == catDefault.ID);

                #endregion
            }
        }

        private void SetOrderVoyage(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var orderID = classifyProduct.OrderID;
            var declarant = classifyProduct.Admin;

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC Begin

            OrderItem[] orderItems = new Order() { ID = orderID, }.Items.ToArray();
            OrderItem[] orderItemsHasCategory = orderItems.Where(t => t.Category != null).ToArray();
            Dictionary<OrderSpecialType, ItemCategoryType> dicCheckOrderSpecialType = new Dictionary<OrderSpecialType, ItemCategoryType>();
            dicCheckOrderSpecialType.Add(OrderSpecialType.HighValue, ItemCategoryType.HighValue);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Inspection, ItemCategoryType.Inspection);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Quarantine, ItemCategoryType.Quarantine);
            dicCheckOrderSpecialType.Add(OrderSpecialType.CCC, ItemCategoryType.CCC);

            foreach (var dic in dicCheckOrderSpecialType)
            {
                if (orderItemsHasCategory != null && orderItemsHasCategory.Any())
                {
                    bool isTheType = orderItemsHasCategory.Any(t => (t.Category.Type.GetHashCode() & dic.Value.GetHashCode()) > 0);
                    var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage();
                    orderVoyage.Order = new Order() { ID = orderID, };
                    orderVoyage.Type = dic.Key;
                    if (isTheType)
                    {
                        orderVoyage.Enter();
                    }
                    else
                    {
                        orderVoyage.Abandon();
                    }
                }
            }

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC End
        }

        private void UpdateOrderItemChangeNotice(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var changeNotice = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView(reponsitory).GetTop(100, i => i.OrderItemID == classifyProduct.ID);
            foreach (var cn in changeNotice)
            {
                cn.ProcessState = Enums.ProcessState.Processed;
                cn.Enter();
            }
        }


        private void WriteProductClassifyAnomalyLog(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var declarant = classifyProduct.Admin;
            if (declarant != null)
            {
                string summary = "报关员【" + declarant.RealName + "】将产品置为归类异常，等待跟单员审批";
                classifyProduct.Log(Enums.LogTypeEnums.Classify, summary);
            }
        }

        //================================================= End =====================================================
    }
}
