using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Needs.Wl.PlanningService
{
    public class DyjCallBack : ApiCallBack
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public DyjCallBack() : base("dyj")
        {

        }

        protected override void PushClassifyResult(PreClassifyProduct cResult)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 推送前：构建推送报文

                //构建产品归类结果的报文
                bool IsInsp = (cResult.Type & ItemCategoryType.Inspection) > 0 ? true : false;
                bool IsHighValue = (cResult.Type & ItemCategoryType.HighValue) > 0 ? true : false;
                bool IsCCC = (cResult.Type & ItemCategoryType.CCC) > 0 ? true : false;
                bool IsOrigin = (cResult.Type & ItemCategoryType.OriginProof) > 0 ? true : false;
                bool IsForbidden = (cResult.Type & ItemCategoryType.Forbid) > 0 ? true : false;

                bool IsSpecial = false;
                if (IsInsp || IsHighValue || IsCCC || IsOrigin || IsForbidden)
                {
                    IsSpecial = true;
                }

                // 不需要转义了改为Post方式提交
                //string postdata = new
                //{
                //    id = cResult.PreProduct.ProductUnionCode,
                //    MFC = cResult.Manufacturer.Replace("&", "").MFCUrlEncoding(),
                //    Area = cResult.PreProduct.AreaOfProduction.UrlEncoding(),
                //    BatchNo = cResult.PreProduct.BatchNo.Replace("&", "").UrlEncoding(),                  
                //    Description = cResult.PreProduct.Description.Replace("&", "").UrlEncoding(),
                //    GoodName = cResult.TaxName.UrlEncoding(),
                //    UseFor = cResult.PreProduct.UseFor.UrlEncoding(),
                //    CustomsCode = cResult.ProductName.UrlEncoding(),
                //    IsBusinessCheck = (IsInsp ? "1" : "0").UrlEncoding(),
                //    IsSpecialPack = (IsSpecial ? "1" : "0").UrlEncoding()
                //}.Json();


                string postdata = new
                {
                    id = cResult.PreProduct.ProductUnionCode,
                    MFC = cResult.Manufacturer,
                    Area = cResult.PreProduct.AreaOfProduction,
                    BatchNo = cResult.PreProduct.BatchNo,
                    Description = cResult.PreProduct.Description,
                    GoodName = cResult.TaxName,
                    GoodNameID = cResult.TaxCode,
                    UseFor = cResult.PreProduct.UseFor,
                    CustomsCode = cResult.ProductName,
                    IsBusinessCheck = (IsInsp ? "1" : "0"),
                    IsSpecialPack = (IsSpecial ? "1" : "0"),
                    OtherAmount = cResult.InspectionFee == null ? 0 : cResult.InspectionFee,
                    CustomsTaxlv = cResult.TariffRate,
                    key = "bd74ee897e5c9bf097117ff30b9862c6",
                    IsHighValue = (IsHighValue ? "1" : "0"),
                    IsCCC = (IsCCC ? "1" : "0"),
                    IsOrigin = (IsOrigin ? "1" : "0"),
                    IsForbidden = (IsForbidden ? "1" : "0"),
                }.Json();


                //构建产品申报要素的报文
                string[] elements = cResult.Elements.Split('|');
                List<InsideEletmentsPost> lists = new List<InsideEletmentsPost>();
                int i = 0;
                foreach (var ele in elements)
                {
                    InsideEletmentsPost p = new InsideEletmentsPost();
                    p.objid = cResult.HSCode + "_" + i.ToString();
                    p.objname = ele;
                    lists.Add(p);
                    i++;
                }

                // 不需要转义了改为Post方式提交
                //string PostElementsResult = new
                //{
                //    partno = cResult.Model,
                //    mfc = cResult.Manufacturer,
                //    list = lists,
                //    key = "bd74ee897e5c9bf097117ff30b9862c6"
                //}.Json().UrlEncoding();

                string PostElementsResult = new
                {
                    partno = cResult.Model,
                    mfc = cResult.Manufacturer,
                    list = lists,
                    key = "bd74ee897e5c9bf097117ff30b9862c6"
                }.Json();

                #endregion

                #region 推送

                //推送申报要素
                var elementsUrl = base.ApiSetting.Apis[ApiType.CElements].Url;
                HttpRequest request = new HttpRequest
                {
                    Timeout = this.ApiSetting.Timeout,
                };
                //string elementsResult = request.Get(string.Format(elementsUrl, PostElementsResult));
                string elementsResult = base.PushMsg(elementsUrl, PostElementsResult);

                //申报要素结果 推送日志
                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    ApiNoticeID = base.ApiNotice.ID,
                    PushMsg = PostElementsResult,
                    ResponseMsg = elementsResult,
                    CreateDate = DateTime.Now,
                });

                System.Threading.Thread.Sleep(1000);

                //推送归类结果
                var url = base.ApiSetting.Apis[ApiType.CResult].Url;
                request = new HttpRequest
                {
                    Timeout = this.ApiSetting.Timeout,
                };
                //string result = request.Get(string.Format(url, postdata));
                string result = base.PushMsg(url, postdata);

                //归类结果推送日志
                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    ApiNoticeID = base.ApiNotice.ID,
                    PushMsg = postdata,
                    ResponseMsg = result,
                    CreateDate = DateTime.Now,
                });

                result = result.Replace("\\", "");
                string jsonResult = result.Substring(1, result.Length - 2);


                InsidePreProducts pre = JsonConvert.DeserializeObject<InsidePreProducts>(jsonResult);

                //该单据状态已修改，状态改为已成功
                if (pre.isSuccess == false && pre.message == "该单据状态已修改" && pre.Status == "400")
                {
                    pre.isSuccess = true;
                }

                //InsidePreProducts elementsPre = JsonConvert.DeserializeObject<InsidePreProducts>(elementsResult);
                #endregion

                base.AfterPush(pre.isSuccess, postdata, result);

                //如果大赢家推送返回这个错误，则把状态改为未推送
                if (pre.message.Contains("System.Exception: 远程服务器返回错误: (500) 内部服务器错误。"))
                {
                    responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                       new
                       {
                           PushStatus = PushStatus.Unpush,
                           UpdateDate = DateTime.Now
                       }, item => item.ItemID == this.ApiNotice.ItemID);
                }
            }
        }

        protected override void PushDutiablePrice(DecHead decHead)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 推送前：构建推送报文

                var changeNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Where(t => t.OderID == decHead.OrderID).FirstOrDefault();
                if (changeNotice != null)
                {
                    //如果有税费变更，且税费变更未处理，则不推送，直接返回
                    if (changeNotice.ProcessState != (int)ProcessState.Processed)
                    {
                        return;
                    }
                }

                //使用订单中的海关汇率 ryan 20200806
                var orderOrigin = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == decHead.OrderID).FirstOrDefault();
                int decimalForTotalPrice = 0;
                if (decHead.isTwoStep)
                {
                    decimalForTotalPrice = 2;
                }

                var orderCusExchangeRate = orderOrigin.CustomsExchangeRate;

                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<DutiablePriceItem, bool>> expression = item => item.DecHeadID == decHead.ID;

                var Result = new Needs.Ccs.Services.Views.DutiablePriceView<DutiablePriceItem>(reponsitory).GetIQueryableResult(expression);

                List<DutiablePriceItem> calcedItems = new List<DutiablePriceItem>();

                var agreement = new Needs.Ccs.Services.Views.ClientAgreementsView(reponsitory).Where(item => item.ID == orderOrigin.ClientAgreementID).SingleOrDefault();
                //确定单双抬头
                var invoiceType = agreement.InvoiceType;

                //订单项信息
                var orderitemList = new Needs.Ccs.Services.Views.OrderItemsView(reponsitory).Where(t => t.OrderID == orderOrigin.ID).ToArray();
                var count = orderitemList.Count();//条数
                //杂费合计
                var MiscFeesView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(item =>
                            item.OrderID == orderOrigin.ID
                            && item.Type != (int)OrderPremiumType.AgencyFee
                            && item.Type != (int)OrderPremiumType.InspectionFee
                            && item.Status == (int)Status.Normal).ToArray();
                var MiscFees = MiscFeesView.Length > 0 ? MiscFeesView.Sum(item => item.Count * item.UnitPrice * item.Rate) : 0M;

                var TotalImportTax = 0M;

                foreach (var item in Result)
                {
                    DutiablePriceItem m = new DutiablePriceItem();
                    m.ProductUniqueCode = item.ProductUniqueCode;

                    //海关进价
                    m.CusInputPrice = (item.DutiablePrice * orderCusExchangeRate.Value).ToRound(decimalForTotalPrice) + ((item.DutiablePrice * orderCusExchangeRate.Value).ToRound(decimalForTotalPrice) * item.TariffRate).ToRound(2);
                    //2、单抬头：全额开票中的不含税价格
                    var orderitem = orderitemList.FirstOrDefault(t => t.ID == item.OrderItemID);
                    //开票时，使用实收汇率计算税费 ryan  20211122
                    var topPrice = (orderitem.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                    var total = (topPrice * orderOrigin.CustomsExchangeRate.Value).ToRound(2);
                    //关税
                    decimal ImportTax = (total * orderitem.ImportTax.Rate).ToRound(2);
                    decimal ImportReal = (total * orderitem.ImportTax.ReceiptRate).ToRound(2);

                    TotalImportTax += ImportTax;

                    //计算报关价格
                    //1、双抬头：完税价格
                    if (invoiceType == InvoiceType.Service)
                    {
                        m.DutiablePrice = m.CusInputPrice;
                    }
                    else
                    {
                        //代理费
                        var aveAgencyFee = orderitem.TotalPrice * orderOrigin.RealExchangeRate.Value * agreement.AgencyRate * (1 + agreement.InvoiceTaxRate);
                        //消费税
                        decimal ExciseTax = ((total + ImportTax) / (1 - orderitem.ExciseTax.ReceiptRate) * orderitem.ExciseTax.ReceiptRate).ToRound(2);
                        var exciseTaxRate = orderitem.ExciseTax.ReceiptRate;
                        decimal AddedValueTax = (((total + ImportTax) + (total + ImportTax) / (1 - exciseTaxRate) * exciseTaxRate) * orderitem.AddedValueTax.ReceiptRate).ToRound(2);
                        //商检费
                        decimal InspectionFee = orderitem.InspectionFee.GetValueOrDefault() * (1 + agreement.InvoiceTaxRate);
                        //平摊杂费（不含商检费）
                        var miscFee = MiscFees * (1 + agreement.InvoiceTaxRate);
                        var aveMiscFee = (miscFee / count);

                        //价税合计
                        var TaxedDutiablePrice = (orderitem.TotalPrice * orderOrigin.RealExchangeRate.Value + ImportReal + ExciseTax + AddedValueTax + aveAgencyFee + InspectionFee + aveMiscFee).ToRound(4);

                        //价
                        m.DutiablePrice = (TaxedDutiablePrice / (1 + agreement.InvoiceTaxRate)).ToRound(4);
                    }

                    //20220627 新增字段 ryan 
                    m.DecListID = item.DecListID;
                    //Round(Round(美金*海关汇率,2) / 1.002, 2) +应交关税
                    m.InPrice = ((item.DeclTotal * orderOrigin.CustomsExchangeRate.Value).ToRound(2) / ConstConfig.TransPremiumInsurance).ToRound(2) + ImportTax;

                    m.TaxName = item.TaxName;
                    m.TaxCode = item.TaxCode;
                    m.DeclareDate = item.DeclareDate;
                    m.EntryId = item.EntryId;
                    m.Model = item.Model;
                    m.Qty = item.Qty;
                    m.HSCode = item.HSCode;
                    m.TariffRate = item.TariffRate;
                    m.TariffReceiptRate = item.TariffReceiptRate;
                    m.DeclTotal = item.DeclTotal;
                    m.DeclTotalRMB = (item.DeclTotal * orderCusExchangeRate.Value).ToRound(2);
                    m.TariffPay = (m.DeclTotalRMB * item.TariffRate).ToRound(2);
                    m.ValueVat = (((m.DeclTotalRMB + m.TariffPay) + (m.DeclTotalRMB + m.TariffPay) / (1 - item.ConsumeTaxRate) * item.ConsumeTaxRate) * item.AddedValueRate).ToRound(2);
                    m.ExciseTax = ((m.DeclTotalRMB + m.TariffPay) / (1 - item.ConsumeTaxRate) * item.ConsumeTaxRate).ToRound(2);
                    m.AddedValueRate = item.AddedValueRate;
                    calcedItems.Add(m);
                }

                decimal totalValueVat = calcedItems.Sum(t => t.ValueVat);
                decimal totalExciseTax = calcedItems.Sum(t => t.ExciseTax);

                var flowView = new Needs.Ccs.Services.Views.DecTaxFlowsView(reponsitory).Where(item => item.DecheadID == decHead.ID).ToArray();
                var addTaxFlow = flowView.Where(t => t.TaxType == DecTaxType.AddedValueTax).FirstOrDefault();
                var tariffTaxFlow = flowView.Where(t => t.TaxType == DecTaxType.Tariff).FirstOrDefault();
                var icgooOrderID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => t.OrderID == decHead.OrderID).FirstOrDefault();


                //处理最后一项的芯达通进价，使用减法。
                var DecHeadPrice = calcedItems.Sum(t => t.DeclTotal);
                //var traffTotal = tariffTaxFlow == null ? 0 : tariffTaxFlow.Amount;
                //差额 = 整单的进价 - 项合计进价
                //整单的进价
                var inTotal = ((DecHeadPrice * orderOrigin.CustomsExchangeRate.Value).ToRound(2) / ConstConfig.TransPremiumInsurance).ToRound(2) + TotalImportTax;
                var differ = inTotal - calcedItems.Sum(t => t.InPrice);
                //最后一项补上差额
                calcedItems.Last().InPrice = calcedItems.Last().InPrice + differ;


                var PostMessage = calcedItems.Select(item => new
                {
                    单据号 = item.ProductUniqueCode == "" ? "" : Encryption.Encrypt(item.ProductUniqueCode),
                    完税价格 = item.DutiablePrice,
                    海关进价 = item.CusInputPrice,
                    芯达通进价 = item.InPrice,
                    库存ID = item.DecListID,
                    合同号 = decHead.ContrNo.Trim(),
                    型号信息分类 = item.TaxName,
                    型号信息分类值 = item.TaxCode,
                    报关完成日期 = decHead.IEDate.Insert(4, "-").Insert(7, "-"),
                    报关日期 = decHead.DDate.HasValue ? decHead.DDate.Value.ToString("yyyy-MM-dd") : "",
                    报关单号 = decHead.EntryId,
                    开票公司AA = "",
                    规格型号E = item.Model,
                    数量G = item.Qty,
                    商品编号 = item.HSCode,
                    海关汇率 = orderCusExchangeRate,
                    应交关税率 = item.TariffRate,
                    实交关税率 = item.TariffReceiptRate,
                    报关总价 = item.DeclTotal,
                    应交关税 = item.TariffPay,
                    实交关税 = (item.DeclTotalRMB * item.TariffReceiptRate).ToRound(2),
                    实交增值税 = totalValueVat >= 50 ? item.ValueVat : 0,
                    实交消费税 = totalExciseTax >= 50 ? item.ExciseTax : 0,
                    完税价格增值税 = (((item.DeclTotalRMB + item.TariffPay) + (item.DeclTotalRMB + item.TariffPay) / (1 - item.ConsumeTaxRate) * item.ConsumeTaxRate) * item.AddedValueRate).ToRound(2),
                    增值税海关缴款书号码 = addTaxFlow == null ? "" : addTaxFlow.TaxNumber,
                    关税海关缴款书号码 = tariffTaxFlow == null ? "" : tariffTaxFlow.TaxNumber,
                    增值税缴款书金额 = addTaxFlow == null ? 0 : addTaxFlow.Amount,
                    大赢家订单编号 = icgooOrderID == null ? "" : icgooOrderID.IcgooOrder,
                    发票类型 = invoiceType == InvoiceType.Service ? "双抬头" : "单抬头"
                }).OrderByDescending(item => item.单据号);

                var SendJson = new
                {
                    requestitem = "匹配价格",
                    data = PostMessage,
                    key = base.ApiSetting.Apis[ApiType.DutiablePrice].Key
                }.Json();

                #endregion

                #region 推送

                string postdata = SendJson.Replace("&", "%26");
                var url = base.ApiSetting.Apis[ApiType.DutiablePrice].Url;
                string result = PushMsg(url, postdata);

                #endregion

                #region 推送后：写ApiNotice推送日志，更新ApiNotice状态

                //写ApiNotice推送日志
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    ApiNoticeID = base.ApiNotice.ID,
                    PushMsg = postdata,
                    ResponseMsg = result,
                    CreateDate = DateTime.Now,
                });

                DutiablePriceJson model = JsonConvert.DeserializeObject<DutiablePriceJson>(result);
                bool isAllSuccess = true;

                foreach (var item in model.data)
                {
                    if (item.状态 == false)
                    {
                        isAllSuccess = false;
                    }
                }

                base.AfterPush(isAllSuccess, postdata, result);

                #endregion
            }
        }

        protected override void PushIcgooTariffDiff(OrderItem cResult, string supplier)
        {
            throw new NotImplementedException();
        }
    }
}
