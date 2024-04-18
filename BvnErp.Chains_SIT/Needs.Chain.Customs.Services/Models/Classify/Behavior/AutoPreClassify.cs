using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Needs.Underly;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品预归类自动归类
    /// </summary>
    public class AutoPreClassify : Classify
    {
        public AutoPreClassify(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 自动归类完成之后需要做的操作
         * 1. 更新归类操作人
         * 2. 写信息推送通知
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (PreClassifyProduct)e.Product;

                if (classifyProduct.ClassifyStatus == ClassifyStatus.Done)
                {
                    UpdateSecondOperator(classifyProduct, reponsitory);
                    ToApiNotice(classifyProduct, reponsitory);
                }
                else
                {
                    UpdateFirstOperator(classifyProduct, reponsitory);
                }
            }
        }

        public override void DoClassify()
        {
            var classifyProduct = (PreClassifyProduct)this.Product;

            //2021.10.12 新要求： 产品来源为调货报关的，不做自动归类，放在预处理一
            if (classifyProduct.PreProduct.Source == "调货报关")
                return;

            var apisetting = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AutoClassify;

            //是否需要验证价格浮动
            bool isVerifyPriceFluctuation = classifyProduct.PreProduct.CompanyType != CompanyTypeEnums.OutSide ? false : true;
            //调用中心数据接口获取自动归类信息
            var data = new
            {
                PartNumber = classifyProduct.Model,
                Manufacturer = classifyProduct.Manufacturer,
                UnitPrice = classifyProduct.PreProduct.Price,
                isVerifyPriceFluctuation = true,
                highPriceLimit = 0.8,
                lowPriceLimit = 0.3,
                Origin = string.Empty,
            };
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(url, data);
            //code: 100 - 接口调用成功，但是没有自动归类信息、200 - 接口调用成功，有自动归类数据、300 - 接口调用异常
            if (result.code == 200)
            {
                if ((bool)result.data.IsSpecialType)
                    classifyProduct.ClassifyStatus = Enums.ClassifyStatus.First;
                else
                {
                    classifyProduct.ClassifyStatus = Enums.ClassifyStatus.Done;

                    //进一步核对自动归类信息与dyj提交过来的预处理一的归类信息是否完全一致，如果不一致，则只到预处理二，需要报关员进一步确认 
                    if (classifyProduct.PreProduct.CompanyType == CompanyTypeEnums.Inside && !string.IsNullOrEmpty(classifyProduct.HSCode))
                    {
                        if (classifyProduct.HSCode != (string)result.data.HSCode || classifyProduct.Elements?.ToHalfAngle() != ((string)result.data.Elements).ToHalfAngle() ||
                            classifyProduct.TaxCode != (string)result.data.TaxCode || classifyProduct.TaxName != (string)result.data.TaxName)
                            classifyProduct.ClassifyStatus = Enums.ClassifyStatus.First;
                    }
                }
                Supplement(classifyProduct, result.data);
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
                    Summary = "产品预归类 - 自动归类"
                };
                log.Enter();

                //如果没有自动归类记录，针对icgoo提交过来的预归类产品，需要再做dyj大数据归类
                if (classifyProduct.PreProduct.CompanyType == CompanyTypeEnums.Icgoo || classifyProduct.PreProduct.CompanyType == CompanyTypeEnums.FastBuy)
                {
                    var dyjClassify = new DyjDataClassify(classifyProduct);
                    dyjClassify.DoClassify();
                }
            }
        }

        private void Supplement(PreClassifyProduct classifyProduct, dynamic autoClassified)
        {
            classifyProduct.ProductName = autoClassified.TariffName;
            classifyProduct.TariffRate = autoClassified.ImportPreferentialTaxRate + autoClassified.OriginRate;
            classifyProduct.AddedValueRate = autoClassified.VATRate;
            classifyProduct.ExciseTaxRate = autoClassified.ExciseTaxRate;
            classifyProduct.Unit1 = autoClassified.LegalUnit1;
            classifyProduct.Unit2 = autoClassified.LegalUnit2;
            classifyProduct.CIQCode = autoClassified.CIQCode;

            classifyProduct.HSCode = autoClassified.HSCode;
            classifyProduct.TaxCode = autoClassified.TaxCode;
            classifyProduct.TaxName = autoClassified.TaxName;
            classifyProduct.Elements = autoClassified.Elements;

            //特殊类型
            classifyProduct.Type = ItemCategoryType.Normal;
            if ((bool)autoClassified.CIQ)
            {
                classifyProduct.Type |= ItemCategoryType.Inspection;
                classifyProduct.InspectionFee = autoClassified.CIQprice;
            }
            if ((bool)autoClassified.Ccc)
            {
                classifyProduct.Type |= ItemCategoryType.CCC;
            }
            if ((bool)autoClassified.Coo)
            {
                classifyProduct.Type |= ItemCategoryType.OriginProof;
            }
            if ((bool)autoClassified.Embargo)
            {
                classifyProduct.Type |= ItemCategoryType.Forbid;
            }
            if ((bool)autoClassified.HkControl)
            {
                classifyProduct.Type |= ItemCategoryType.HKForbid;
            }
            if ((bool)autoClassified.IsHighPrice)
            {
                classifyProduct.Type |= ItemCategoryType.HighValue;
            }
            if ((bool)autoClassified.IsDisinfected)
            {
                classifyProduct.Type |= ItemCategoryType.Quarantine;
            }
        }

        private void UpdateFirstOperator(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyFirstOperator = Icgoo.DefaultCreator,
            }, item => item.ID == classifyProduct.ID);
        }

        private void UpdateSecondOperator(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyFirstOperator = Icgoo.DefaultCreator,
                ClassifySecondOperator = Icgoo.DefaultCreator,
            }, item => item.ID == classifyProduct.ID);
        }

        private void ToApiNotice(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
            {
                ID = ChainsGuid.NewGuidUp(),
                PushType = (int)Enums.PushType.ClassifyResult,
                ClientID = classifyProduct.PreProduct.ClientID,
                ItemID = classifyProduct.ID,
                PushStatus = (int)Enums.PushStatus.Unpush,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            });
        }
    }
}
