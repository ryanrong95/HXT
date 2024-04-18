using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Interfaces;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Descriptions;
using Needs.Ccs.Services.ApiSettings;
using System.Configuration;
using Needs.Underly;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品归类自动归类
    /// </summary>
    public class AutoClassify : Classify
    {
        public AutoClassify(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 自动归类完成之后需要做的操作
         * 1.更新归类操作人
         * 2.提交报价信息
         * 3.更新订单信息
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (ClassifyProduct)e.Product;

#pragma warning disable
#if PvData
                if (classifyProduct.ClassifyStatus == ClassifyStatus.Done)
                {
                    UpdateSecondOperator(classifyProduct, reponsitory);
                    ProductQuote(classifyProduct);
                    UpdateOrder(classifyProduct, reponsitory);
                }
                else
                {
                    UpdateFirstOperator(classifyProduct, reponsitory);
                }
#else
                UpdateFirstOperator(classifyProduct, reponsitory);
                WriteProductClassifyLog(classifyProduct, reponsitory);
#endif
#pragma warning restore
            }
        }

        public override void DoClassify()
        {
            var classifyProduct = (ClassifyProduct)this.Product;

#pragma warning disable
#if PvData
            var apisetting = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AutoClassify;

            //是否需要验证价格浮动
            bool isVerifyPriceFluctuation = classifyProduct.Client.ClientType == ClientType.Internal ? false : true;
            //调用中心数据接口获取自动归类信息
            var data = new
            {
                PartNumber = classifyProduct.Model,
                Manufacturer = classifyProduct.Manufacturer,
                UnitPrice = classifyProduct.UnitPrice,
                isVerifyPriceFluctuation = true,
                highPriceLimit = 0.8,
                lowPriceLimit = 0.3,
                Origin = classifyProduct.Origin,
            };
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(url, data);
            //code: 100 - 接口调用成功，但是没有自动归类信息、200 - 接口调用成功，有自动归类数据、300 - 接口调用异常
            if (result.code == 200)
            {
                Supplement(classifyProduct, result.data);
                if ((bool)result.data.IsPriceFluctuation)
                    classifyProduct.ClassifyStatus = ClassifyStatus.Unclassified;
                else if ((bool)result.data.IsSpecialType)
                    classifyProduct.ClassifyStatus = ClassifyStatus.First;
                else
                    classifyProduct.ClassifyStatus = ClassifyStatus.Done;
                classifyProduct.DoClassify();
            }
            else
            {
                var log = new Models.ClassifyApiLogs()
                {
                    ClassifyProductID = classifyProduct.ID,
                    Url = url,
                    RequestContent = data.Json(),
                    ResponseContent = result.Json(),
                    Summary = "产品归类 - 自动归类"
                };
                log.Enter();
            }
#else
            var categoryDefault = new Views.ProductCategoriesDefaultsView().FirstOrDefault(p => p.Model == classifyProduct.Model);
            if (categoryDefault != null && (!string.IsNullOrWhiteSpace(categoryDefault.TaxCode)) && (!string.IsNullOrWhiteSpace(categoryDefault.TaxName)))
            {
                var productCategories = new Views.Origins.OrderItemsOrigin().Where(item => item.Model == classifyProduct.Model && item.CreateDate >= DateTime.Now.AddMonths(-3));//三个月之前的历史记录
                if (productCategories.Count() == 0)
                {
                    AutoClassfiy(classifyProduct, categoryDefault);
                }
                else
                {
                    var vagPrice = productCategories.Count() == 0 ? 0 : productCategories.Average(item => item.UnitPrice); //三个月单价平均值
                    var percentValue = System.Math.Abs(classifyProduct.UnitPrice - vagPrice) / vagPrice;  //增长率
                    if (percentValue < (decimal)0.1)
                    {
                        AutoClassfiy(classifyProduct, categoryDefault);
                    }
                }

            }
#endif
#pragma warning restore
        }

        /// <summary>
        /// 自动归类
        /// </summary>
        /// <param name="classify"></param>
        /// <param name="prodefault"></param>
        private void AutoClassfiy(ClassifyProduct classify, ProductCategoriesDefault prodefault)
        {
            classify.ClassifyStatus = Enums.ClassifyStatus.First;
            var tariff = new Views.CustomsTariffsView().Where(item => item.HSCode == prodefault.HSCode).FirstOrDefault();
            if (tariff != null)
            {
                var elements = tariff.Elements;
                var elementArr = elements.Split(';');
                for (int i = 0; i < elementArr.Length; i++)
                {
                    var arr = elementArr[i].Split(':');
                    if (arr[1] == "品牌")
                    {
                        var categoryElementArr = prodefault.Elements.Split('|');
                        categoryElementArr[i] = classify.Manufacturer + "牌";
                        prodefault.Elements = string.Join("|", categoryElementArr);
                    }
                }
            }
            classify.Supplement(prodefault);
            classify.DoClassify();
        }

        private void Supplement(ClassifyProduct classifyProduct, dynamic autoClassified)
        {
            //海关归类
            classifyProduct.Category = new OrderItemCategory();
            classifyProduct.Category.OrderItemID = classifyProduct.ID;
            classifyProduct.Category.TaxCode = autoClassified.TaxCode;
            classifyProduct.Category.TaxName = autoClassified.TaxName;
            classifyProduct.Category.HSCode = autoClassified.HSCode;
            classifyProduct.Category.Name = autoClassified.TariffName;
            classifyProduct.Category.Elements = autoClassified.Elements;
            classifyProduct.Category.Unit1 = autoClassified.LegalUnit1;
            classifyProduct.Category.Unit2 = autoClassified.LegalUnit2;
            classifyProduct.Category.CIQCode = autoClassified.CIQCode;

            //特殊类型
            classifyProduct.Category.Type = ItemCategoryType.Normal;
            if ((bool)autoClassified.CIQ)
            {
                classifyProduct.Category.Type |= ItemCategoryType.Inspection;
            }
            if ((bool)autoClassified.Ccc)
            {
                classifyProduct.Category.Type |= ItemCategoryType.CCC;
            }
            if ((bool)autoClassified.Coo)
            {
                classifyProduct.Category.Type |= ItemCategoryType.OriginProof;
            }
            if ((bool)autoClassified.Embargo)
            {
                classifyProduct.Category.Type |= ItemCategoryType.Forbid;
            }
            if ((bool)autoClassified.HkControl)
            {
                classifyProduct.Category.Type |= ItemCategoryType.HKForbid;
            }
            if ((bool)autoClassified.IsHighPrice)
            {
                classifyProduct.Category.Type |= ItemCategoryType.HighValue;
            }
            if ((bool)autoClassified.IsDisinfected)
            {
                classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
            }

            //商检
            if ((bool)autoClassified.CIQ)
            {
                classifyProduct.IsInsp = true;
                classifyProduct.InspectionFee = autoClassified.CIQprice;
            }

            //关税
            classifyProduct.ImportTax = new OrderItemTax();
            classifyProduct.ImportTax.OrderItemID = classifyProduct.ID;
            classifyProduct.ImportTax.Type = Enums.CustomsRateType.ImportTax;
            classifyProduct.ImportTax.Rate = autoClassified.ImportPreferentialTaxRate + autoClassified.OriginRate;
            classifyProduct.ImportTax.ImportPreferentialTaxRate = autoClassified.ImportPreferentialTaxRate;
            classifyProduct.ImportTax.OriginRate = autoClassified.OriginRate;
            classifyProduct.ImportTax.ReceiptRate = classifyProduct.ImportTax.Rate;

            //增值税
            classifyProduct.AddedValueTax = new OrderItemTax();
            classifyProduct.AddedValueTax.OrderItemID = classifyProduct.ID;
            classifyProduct.AddedValueTax.Type = Enums.CustomsRateType.AddedValueTax;
            classifyProduct.AddedValueTax.Rate = autoClassified.VATRate;
            classifyProduct.AddedValueTax.ReceiptRate = classifyProduct.AddedValueTax.Rate;

            //消费税
            classifyProduct.ExciseTax = new OrderItemTax();
            classifyProduct.ExciseTax.OrderItemID = classifyProduct.ID;
            classifyProduct.ExciseTax.Type = Enums.CustomsRateType.ConsumeTax;
            classifyProduct.ExciseTax.Rate = autoClassified.ExciseTaxRate;
            classifyProduct.ExciseTax.ReceiptRate = classifyProduct.ExciseTax.Rate;
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

                string summary = "报关员【" + declarant.RealName + "】完成了预处理一。归类结果:" +
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

        private void ProductQuote(ClassifyProduct classifyProduct)
        {
            //如果归类完成，调用中心数据的接口，提交报价信息
            var pvdataApi = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.ProductQuote;
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
            {
                PartNumber = classifyProduct.Model,
                Manufacturer = classifyProduct.Manufacturer,
                Origin = classifyProduct.Origin,
                Currency = classifyProduct.Currency,
                UnitPrice = classifyProduct.UnitPrice,
                Quantity = classifyProduct.Quantity,
                CIQprice = classifyProduct.InspectionFee.GetValueOrDefault()
            });
        }

        private void UpdateFirstOperator(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
            {
                ClassifyFirstOperator = Icgoo.DefaultCreator,
            }, item => item.ID == classifyProduct.Category.ID);
        }

        private void UpdateSecondOperator(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
            {
                ClassifyFirstOperator = Icgoo.DefaultCreator,
                ClassifySecondOperator = Icgoo.DefaultCreator,
            }, item => item.ID == classifyProduct.Category.ID);
        }

        private void UpdateOrder(ClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var orderID = classifyProduct.OrderID;
            var declarant = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);

            var order = new Views.OrdersView(reponsitory)[orderID];
            //如果订单已经报价，不需要再做后续处理
            if (order.OrderStatus >= OrderStatus.Quoted)
                return;

            //未完成归类的产品数量
            int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Count(item => item.OrderID == orderID &&
                    (item.ClassifyStatus == (int)Enums.ClassifyStatus.Unclassified || item.ClassifyStatus == (int)Enums.ClassifyStatus.First) &&
                    item.Status == (int)Enums.Status.Normal);

            if (count == 0)
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    OrderStatus = (int)Enums.OrderStatus.Classified
                }, item => item.ID == orderID);

                if (declarant != null)
                {
                    order?.Log(declarant, "报关员【" + declarant.RealName + "】完成了订单归类，等待跟单员报价");
                    order.Trace(declarant, Enums.OrderTraceStep.Processing, "您的订单产品已归类完成");
                }

                //Icgoo和大赢家订单归类完成，系统自动完成报价和确认报价，进入待报关状态
                if (classifyProduct.OrderType != Enums.OrderType.Outside)
                {
                    var classifiedOrder = new Views.ClassifiedInsideOrdersView(reponsitory)[orderID];
                    classifiedOrder.SetAdmin(classifiedOrder.Client.Merchandiser);
                    classifiedOrder?.GenerateBill();
                    classifiedOrder?.Quote();

                    var quotedOrder = new Views.QuotedInsideOrdersView(reponsitory)[orderID];
                    quotedOrder?.IcgooQuoteConfirm();
                    quotedOrder?.ToReceivables();
                }
            }
        }

        //================================================= End =====================================================
    }
}
