using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Interfaces;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品预归类预处理一
    /// </summary>
    public sealed class PreClassifyStep1 : Classify
    {
        public PreClassifyStep1(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 预处理一归类完成之后需要做的操作
         * 1.更新归类操作人
         * 2.写产品归类日志
         * 3.写产品归类信息变更日志
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (PreClassifyProduct)e.Product;

                UpdateClassifyOperator(classifyProduct, reponsitory);
                WriteProductClassifyLog(classifyProduct, reponsitory);
                WriteProductClassifyChangeLog(classifyProduct, reponsitory);
            }
        }

        public override void DoClassify()
        {
            this.Product.ClassifyStatus = Enums.ClassifyStatus.First;
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep1;
            base.DoClassify();
        }

        public override void Lock()
        {
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep1;
            base.Lock();
        }

        public override void UnLock()
        {
            this.Product.ClassifyStep = Enums.ClassifyStep.PreStep1;
            base.UnLock();
        }

        //================================================= Begin ===================================================

        private void UpdateClassifyOperator(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyFirstOperator = classifyProduct.Admin.ID,
            }, item => item.ID == classifyProduct.ID);
        }

        private void WriteProductClassifyLog(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var declarant = classifyProduct.Admin;
            if (declarant != null)
            {
                string categoryType = null;
                foreach (Enums.ItemCategoryType type in Enum.GetValues(typeof(Enums.ItemCategoryType)))
                {
                    if ((classifyProduct.Type & type) > 0)
                    {
                        categoryType += type.GetDescription() + " ";
                    }
                }
                if (categoryType == null)
                {
                    categoryType = Enums.ItemCategoryType.Normal.GetDescription();
                }

                string summary = "报关员【" + declarant.RealName + "】完成了预处理一。归类结果:" +
                        " 品牌【" + classifyProduct.Manufacturer +
                        "】; 型号【" + classifyProduct.Model +
                        "】; 单价【" + classifyProduct.PreProduct.Price +
                        "】; 交易币种【" + classifyProduct.PreProduct.Currency +
                        "】; 海关编码【" + classifyProduct.HSCode +
                        "】; 报关品名【" + classifyProduct.ProductName +
                        "】; 类型【" + categoryType +
                        "】; 商检费用【" + (classifyProduct.IsInsp ? (decimal?)classifyProduct.InspectionFee : null) +
                        "】; 税务编码【" + classifyProduct.TaxCode +
                        "】; 税务名称【" + classifyProduct.TaxName +
                        "】; 关税率【" + classifyProduct.TariffRate +
                        "】; 增值税率【" + classifyProduct.AddedValueRate +
                        "】; 法一单位【" + classifyProduct.Unit1 +
                        "】; 法二单位【" + classifyProduct.Unit2 +
                        "】; 检验检疫编码【" + classifyProduct.CIQCode +
                        "】; 申报要素【" + classifyProduct.Elements +
                        "】; 摘要备注【" + classifyProduct.Summary + "】";
                classifyProduct.Log(Enums.LogTypeEnums.Classify, summary);
            }
        }

        private void WriteProductClassifyChangeLog(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var model = classifyProduct.Model;
            var inspFee = classifyProduct.IsInsp ? (decimal?)classifyProduct.InspectionFee : null;
            var catDefault = new Views.ProductCategoriesDefaultsView(reponsitory).Where(item => item.Model == model).FirstOrDefault();

            if (catDefault == null)
            {
                #region 新增自动归类记录

                catDefault = new ProductCategoriesDefault();
                catDefault.Model = classifyProduct.Model;
                catDefault.Manufacturer = classifyProduct.Manufacturer;
                catDefault.ProductName = classifyProduct.ProductName;
                catDefault.HSCode = classifyProduct.HSCode;
                catDefault.TariffRate = classifyProduct.TariffRate;
                catDefault.AddedValueRate = classifyProduct.AddedValueRate;
                catDefault.TaxCode = classifyProduct.TaxCode;
                catDefault.TaxName = classifyProduct.TaxName;
                catDefault.Type = classifyProduct.Type;
                catDefault.InspectionFee = inspFee;
                catDefault.Unit1 = classifyProduct.Unit1;
                catDefault.Unit2 = classifyProduct.Unit2;
                catDefault.CIQCode = classifyProduct.CIQCode;
                catDefault.Elements = classifyProduct.Elements;
                reponsitory.Insert(catDefault.ToLinq());

                #endregion
            }
            else
            {
                var declarant = classifyProduct.Admin;

                #region 产品归类变更日志

                if (classifyProduct.Manufacturer != catDefault.Manufacturer)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的品牌由【" + catDefault.Manufacturer + "】修改为【" + classifyProduct.Manufacturer + "】");
                }
                if (classifyProduct.ProductName != catDefault.ProductName)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的报关品名由【" + catDefault.ProductName + "】修改为【" + classifyProduct.ProductName + "】");
                }
                if (classifyProduct.HSCode != catDefault.HSCode)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的海关编码由【" + catDefault.HSCode + "】修改为【" + classifyProduct.HSCode + "】");
                }
                if (classifyProduct.TariffRate != catDefault.TariffRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的关税率由【" + catDefault.TariffRate / 100 + "】修改为【" + classifyProduct.TariffRate + "】");
                }
                if (classifyProduct.AddedValueRate != catDefault.AddedValueRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的增值税率由【" + catDefault.AddedValueRate / 100 + "】修改为【" + classifyProduct.AddedValueRate + "】");
                }
                if (classifyProduct.TaxCode != catDefault.TaxCode)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的税务编码由【" + catDefault.TaxCode + "】修改为【" + classifyProduct.TaxCode + "】");
                }
                if (classifyProduct.TaxName != catDefault.TaxName)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的税务名称由【" + catDefault.TaxName + "】修改为【" + classifyProduct.TaxName + "】");
                }
                if (classifyProduct.Unit1 != catDefault.Unit1)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的法定第一单位由【" + catDefault.Unit1 + "】修改为【" + classifyProduct.Unit1 + "】");
                }
                if ((classifyProduct.Unit2 ?? "") != (catDefault.Unit2 ?? ""))
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的法定第二单位由【" + catDefault.Unit2 + "】修改为【" + classifyProduct.Unit2 + "】");
                }
                if (classifyProduct.CIQCode != catDefault.CIQCode)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的检验检疫编码由【" + catDefault.CIQCode + "】修改为【" + classifyProduct.CIQCode + "】");
                }
                if (classifyProduct.Elements != catDefault.Elements)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的申报要素由【" + catDefault.Elements + "】修改为【" + classifyProduct.Elements + "】");
                }

                bool isDefaultCCC = ((catDefault.Type & Enums.ItemCategoryType.CCC) > 0);
                bool isDefaultOriginProof = ((catDefault.Type & Enums.ItemCategoryType.OriginProof) > 0);
                bool isDefaultInsp = ((catDefault.Type & Enums.ItemCategoryType.Inspection) > 0);
                bool isDefaultHighValue = ((catDefault.Type & Enums.ItemCategoryType.HighValue) > 0);
                bool isDefaultForbidden = ((catDefault.Type & Enums.ItemCategoryType.Forbid) > 0);
                bool isDefaultHKForbidden = ((catDefault.Type & Enums.ItemCategoryType.HKForbid) > 0);

                if (classifyProduct.IsCCC != isDefaultCCC)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的CCC认证由【" + (isDefaultCCC ? "是" : "否") + "】修改为【" + (classifyProduct.IsCCC ? "是" : "否") + "】");
                }
                if (classifyProduct.IsOriginProof != isDefaultOriginProof)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的原产地证明由【" + (isDefaultOriginProof ? "是" : "否") + "】修改为【" + (classifyProduct.IsOriginProof ? "是" : "否") + "】");
                }
                if (classifyProduct.IsInsp != isDefaultInsp)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的是否商检由【" + (isDefaultInsp ? "是" : "否") + "】修改为【" + (classifyProduct.IsInsp ? "是" : "否") + "】");
                }
                if (inspFee != catDefault.InspectionFee)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的商检费由【" + catDefault.InspectionFee + "】修改为【" + inspFee + "】");
                }
                if (classifyProduct.IsHighValue != isDefaultHighValue)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的是否高价值产品由【" + (isDefaultHighValue ? "是" : "否") + "】修改为【" + (classifyProduct.IsHighValue ? "是" : "否") + "】");
                }
                if (classifyProduct.IsForbid != isDefaultForbidden)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的是否管控由【" + (isDefaultForbidden ? "是" : "否") + "】修改为【" + (classifyProduct.IsForbid ? "是" : "否") + "】");
                }
                if (classifyProduct.IsHKForbid != isDefaultHKForbidden)
                {
                    classifyProduct.Log("报关员【" + declarant.RealName + "】将型号【" + model + "】的是否香港管制由【" + (isDefaultHKForbidden ? "是" : "否") + "】修改为【" + (classifyProduct.IsHKForbid ? "是" : "否") + "】");
                }

                #endregion

                #region 更新自动归类记录

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>(new
                {
                    //Model = classifyProduct.Product.Model,
                    Manufacture = classifyProduct.Manufacturer,
                    ProductName = classifyProduct.ProductName,
                    HSCode = classifyProduct.HSCode,
                    //TariffRate = classifyProduct.TariffRate,
                    //AddedValueRate = classifyProduct.AddedValueRate,
                    TaxCode = classifyProduct.TaxCode,
                    TaxName = classifyProduct.TaxName,
                    Type = (int?)classifyProduct.Type,
                   // ClassifyType = (int?)catDefault.ClassifyType,
                    InspectionFee = inspFee,
                    //Unit1 = classifyProduct.Unit1,
                    //Unit2 = classifyProduct.Unit2,
                    CIQCode = classifyProduct.CIQCode,
                    Elements = classifyProduct.Elements,
                    UpdateDate = DateTime.Now
                }, item => item.ID == catDefault.ID);

                #endregion
            }
        }

        //================================================= End =====================================================
    }
}
