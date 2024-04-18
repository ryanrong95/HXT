using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PreClassifyController : ApiController
    {
        /// <summary>
        /// 归类完成，提交归类信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SubmitClassified(ClassifiedResult result)
        {
            try
            {
                result.PartNumber = result.PartNumber.Trim();
                result.Manufacturer = result.Manufacturer.Trim();

                ClassifyStep step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), result.Step);
                PD_PreClassifyProduct item = new PD_PreClassifyProductsAll()[result.MainID];

                #region 异常验证

                if (item == null)
                {
                    throw new Exception("预归类产品" + result.MainID + "不存在");
                }

                #endregion

                #region 构建预归类产品

                var declarant = Needs.Underly.FkoFactory<Admin>.Create(result.CreatorID);

                //归类类型
                item.Admin = declarant;
                item.TaxCode = result.TaxCode;
                item.TaxName = result.TaxName;
                item.HSCode = result.HSCode;
                item.ProductName = result.TariffName;
                item.Elements = result.Elements;
                item.Summary = result.Summary;
                item.Manufacturer = result.Manufacturer;
                item.Unit1 = result.LegalUnit1;
                item.Unit2 = result.LegalUnit2;
                item.CIQCode = result.CIQCode;
                item.Summary = result.Summary;
                item.Type = ItemCategoryType.Normal;
                if (result.CIQ)
                {
                    item.Type |= ItemCategoryType.Inspection;
                }
                if (result.Ccc)
                {
                    item.Type |= ItemCategoryType.CCC;
                }
                if (result.Coo)
                {
                    item.Type |= ItemCategoryType.OriginProof;
                }
                //if (result.IsSysEmbargo)
                //{
                //    item.Type |= ItemCategoryType.Forbid;
                //}
                if (result.IsHighPrice)
                {
                    item.Type |= ItemCategoryType.HighValue;
                }
                if (result.Embargo)
                {
                    item.Type |= ItemCategoryType.Forbid;
                }
                if (result.HkControl)
                {
                    item.Type |= ItemCategoryType.HKForbid;
                }

                //关税率
                item.TariffRate = result.ImportPreferentialTaxRate + result.OriginRate;
                //增值税率
                item.AddedValueRate = result.VATRate;
                //消费税率
                item.ExciseTaxRate = result.ExciseTaxRate;

                //归类管控
                item.IsCCC = result.Ccc;
                item.IsOriginProof = result.Coo;
                item.IsSysForbid = result.IsSysEmbargo;
                item.IsSysCCC = result.IsSysCcc;
                item.IsHighValue = result.IsHighPrice;
                item.IsForbid = result.Embargo;
                item.IsHKForbid = result.HkControl;
                item.IsCustomsInspection = result.IsCustomsInspection;

                //商检
                if (result.CIQ)
                {
                    item.IsInsp = true;
                    item.InspectionFee = result.CIQprice;
                }
                else
                {
                    item.IsInsp = false;
                }
                item.Status = Status.Normal;
                var classify = PD_ClassifyFactory.Create(step, item);
                classify.DoClassify();

                #endregion

                //返回归类信息
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }
        }

        /// <summary>
        /// 继续归类，获取下一条归类信息
        /// </summary>
        /// <param name="step">处理步骤</param>
        /// <param name="creatorId">报关员</param>
        /// <param name="useType">产品预归类或咨询归类</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetNext(string step, string creatorId, string useType)
        {
            try
            {
                #region 获取归类信息

                ClassifyStep stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                ClassifyStatus targetClassifyStatus = ClassifyStatus.Unclassified;
                if (stepEnum == ClassifyStep.PreStep1)
                {
                    targetClassifyStatus = ClassifyStatus.Unclassified;
                }
                else if (stepEnum == ClassifyStep.PreStep2)
                {
                    targetClassifyStatus = ClassifyStatus.First;
                }

                PreProductUserType useTypeEnum = (PreProductUserType)Enum.Parse(typeof(PreProductUserType), useType);

                Expression<Func<PD_PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == targetClassifyStatus && item.PreProduct.UseType == useTypeEnum;
                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(item => item.CreateDate));
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate ?? DateTime.MaxValue));

                PD_PreClassifyProduct next = new PD_PreClassifyProductsAll().GetTop(1, expression, lamdasOrderByAscDateTime.ToArray(), null, creatorId, false).FirstOrDefault();

                if (next == null)
                {
                    return ApiResultModel.OutputResult(new JSingle<dynamic>() { code = 100, success = true, data = null });
                }

                #endregion

                #region 锁定

                var admin = Needs.Underly.FkoFactory<Admin>.Create(creatorId);
                var adminRole = new Needs.Ccs.Services.Views.AdminRolesTopView().FirstOrDefault(ar => ar.ID == admin.ID);
                next.Admin = admin;
                next.ClassifyStep = stepEnum;
                JMessage result = next.Lock();
                if (result.code == 300)
                {
                    //锁定失败，抛出异常
                    throw new Exception(result.data);
                }

                #endregion

                #region 返回归类信息

                var pvdataApi = new PvDataApiSetting();
                var wladminApi = new WlAdminApiSetting();
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        MainID = next.ID,
                        ClientCode = next.PreProduct.Client.ClientCode,
                        ClientName = next.PreProduct.Client.Company.Name,

                        PartNumber = next.Model,
                        Manufacturer = next.Manufacturer,
                        Origin = string.Empty,
                        UnitPrice = next.PreProduct.Price.ToString("0.0000"),
                        Currency = next.PreProduct.Currency,
                        CreateDate = next.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                        HSCode = next.HSCode,
                        TariffName = next.ProductName,
                        ImportPreferentialTaxRate = next.TariffRate?.ToString("0.0000"),//芯达通没有区分优惠税率和加征税率, 将进口关税赋给优惠税率
                        VATRate = next.AddedValueRate?.ToString("0.0000"),
                        ExciseTaxRate = next.ExciseTaxRate?.ToString("0.0000"),
                        TaxCode = next.TaxCode,
                        TaxName = next.TaxName,
                        LegalUnit1 = next.Unit1,
                        LegalUnit2 = next.Unit2,
                        CIQCode = next.CIQCode,
                        Elements = next.Elements,

                        OriginATRate = 0,//芯达通没有区分优惠税率和加征税率, 进口关税已经赋给优惠税率, 加征税率为0
                        CIQ = (next.Type & ItemCategoryType.Inspection) > 0,
                        CIQprice = next.InspectionFee.GetValueOrDefault(),
                        Ccc = (next.Type & ItemCategoryType.CCC) > 0,
                        Embargo = (next.Type & ItemCategoryType.Forbid) > 0,
                        HkControl = (next.Type & ItemCategoryType.HKForbid) > 0,
                        IsHighPrice = (next.Type & ItemCategoryType.HighValue) > 0,
                        Coo = (next.Type & ItemCategoryType.OriginProof) > 0,

                        PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                        CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified,
                        NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreGetNext,
                        Step = step,
                        UseType = useType,
                        CreatorID = admin.ID,
                        CreatorName = admin.RealName,
                        Role = adminRole.DeclarantRole.GetHashCode()
                    }
                };
                return ApiResultModel.OutputResult(json);

                #endregion
            }
            catch (Exception ex)
            {
                var json = new JSingle<dynamic>() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }
        }

        /// <summary>
        /// 预归类产品自动归类
        /// </summary>
        /// <param name="categories">预归类产品IDs</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AutoClassify(PreProductCategories categories)
        {
            /*
            int count = categories.MainIDs.Count();
            var preClassifyProducts = new PreClassifyProductsAll().GetTop(count, item => categories.MainIDs.Contains(item.ID), null, null);

            foreach (var preClassifyProduct in preClassifyProducts)
            {
                try
                {
                    var autoCategory = new AutoPreClassify(preClassifyProduct);
                    autoCategory.DoClassify();
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            */

            foreach (string id in categories.MainIDs)
            {
                try
                {
                    //自动归类
                    var preClassifyProduct = new PreClassifyProductsAll()[id];
                    var autoCategory = new AutoPreClassify(preClassifyProduct);
                    autoCategory.DoClassify();
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            //返回归类信息
            var json = new JMessage()
            {
                code = 200,
                success = true,
                data = "归类完成"
            };
            return ApiResultModel.OutputResult(json);
        }
    }
}