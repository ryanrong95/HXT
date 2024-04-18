using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Attributes;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using Needs.Underly;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClassifyController : ApiController
    {
        /// <summary>
        /// 归类完成，提交归类信息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage SubmitClassified(ClassifiedResult result)
        {
            try
            {
                result.PartNumber = result.PartNumber.Trim();
                result.Manufacturer = result.Manufacturer.Trim();

                ClassifyStep step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), result.Step);
                PD_ClassifyProduct item = new PD_ClassifyProductsAll()[result.ItemID];

                #region 异常验证

                if (item == null)
                {
                    throw new Exception("订单项" + result.ItemID + "不存在");
                }

                //如果从已完成列表进入编辑页面，归类操作点击时订单已被报价，则解锁，然后退回到列表
                if (step == ClassifyStep.DoneEdit && item.OrderStatus >= OrderStatus.Quoted)
                {
                    item.ClassifyStep = step;
                    item.UnLock();
                    throw new Exception("该订单已报价！");
                }

                #endregion

                #region 构建归类产品

                var declarant = Needs.Underly.FkoFactory<Admin>.Create(result.CreatorID);

                //归类类型
                if (item.Category == null)
                {
                    item.Category = new OrderItemCategory();
                }
                if (item.ImportTax == null)
                {
                    item.ImportTax = new OrderItemTax();
                }
                if (item.AddedValueTax == null)
                {
                    item.AddedValueTax = new OrderItemTax();
                }
                if (item.ExciseTax == null)
                {
                    item.ExciseTax = new OrderItemTax();
                }

                //归类类型
                item.Category.OrderItemID = item.ID;
                item.Category.Declarant = declarant;
                item.Category.TaxCode = result.TaxCode;
                item.Category.TaxName = result.TaxName;
                item.Category.HSCode = result.HSCode;
                item.Category.Name = result.TariffName;
                item.Category.Elements = result.Elements;
                item.Category.Unit1 = result.LegalUnit1;
                item.Category.Unit2 = result.LegalUnit2;
                item.Category.CIQCode = result.CIQCode;
                item.Category.Summary = result.Summary;
                item.Category.Type = ItemCategoryType.Normal;
                if (result.CIQ)
                {
                    item.Category.Type |= ItemCategoryType.Inspection;
                }
                if (result.Ccc)
                {
                    item.Category.Type |= ItemCategoryType.CCC;
                }
                if (result.Coo)
                {
                    item.Category.Type |= ItemCategoryType.OriginProof;
                }
                if (result.IsSysEmbargo || result.Embargo)
                {
                    item.Category.Type |= ItemCategoryType.Forbid;
                }
                if (result.HkControl)
                {
                    item.Category.Type |= ItemCategoryType.HKForbid;
                }
                if (result.IsHighPrice)
                {
                    item.Category.Type |= ItemCategoryType.HighValue;
                }
                if (result.IsDisinfected)
                {
                    item.Category.Type |= ItemCategoryType.Quarantine;
                }

                //品牌、型号变更
                item.Manufacturer = result.Manufacturer;
                item.Model = result.PartNumber;
                item.Unit = result.Unit;

                item.Admin = declarant;

                bool isPost = false;
                //类型：产品变更  
                //税费变化                
                decimal importTaxRate = result.ImportPreferentialTaxRate + result.OriginRate;
                if (step == ClassifyStep.ReClassify)
                {
                    bool isOrderChange = ModelChangeInsertOrderChangeNotice(item);
                    if (item.ImportTax.Rate != importTaxRate || item.ExciseTax.Rate != result.ExciseTaxRate || item.AddedValueTax.Rate != result.VATRate)
                    {
                        InsertOrderChangeNotice(item, item.ImportTax.Rate, importTaxRate, item.ExciseTax.Rate, result.ExciseTaxRate, item.AddedValueTax.Rate, result.VATRate);
                    }
                    else
                    {
                        if (!isOrderChange)
                        {
                            isPost = true;
                            //Post2Client(item);
                        }
                    }
                }

                //关税率
                item.ImportTax.OrderItemID = item.ID;
                item.ImportTax.Type = CustomsRateType.ImportTax;
                item.ImportTax.Rate = importTaxRate;
                item.ImportTax.ImportPreferentialTaxRate = result.ImportPreferentialTaxRate;
                item.ImportTax.OriginRate = result.OriginRate;
                item.ImportTax.ReceiptRate = item.ImportTax.Rate; //ReceiptRate 这个字段也要保存关税率

                //增值税率
                item.AddedValueTax.OrderItemID = item.ID;
                item.AddedValueTax.Type = CustomsRateType.AddedValueTax;
                item.AddedValueTax.Rate = result.VATRate;
                item.AddedValueTax.ReceiptRate = item.AddedValueTax.Rate; //ReceiptRate 这个字段也要保存增值税率

                //消费税率
                item.ExciseTax.OrderItemID = item.ID;
                item.ExciseTax.Type = CustomsRateType.ConsumeTax;
                item.ExciseTax.Rate = result.ExciseTaxRate;
                item.ExciseTax.ReceiptRate = item.ExciseTax.Rate; //ReceiptRate 这个字段也要保存消费税率

                //归类管控
                item.IsCCC = result.Ccc;
                item.IsForbid = result.Embargo;
                item.IsOriginProof = result.Coo;
                item.IsSysForbid = result.IsSysEmbargo;
                item.IsSysCCC = result.IsSysCcc;
                item.IsHighValue = result.IsHighPrice;
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
                    item.InspectionFee = null;
                }


                var classify = PD_ClassifyFactory.Create(step, item);
                classify.DoClassify();

                //Post给客户端需要在归类动作完成后，归类完成才会把产品变更的状态改为 完成，
                //Post的时候会判断该订单的产品变更状态，如果还有未完成的就不提交
                if (isPost)
                {
                    Post2Client(item);
                }

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
        /// 如果重新归类的时候，发现型号变更了，需要生成订单变更
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <returns></returns>
        private bool ModelChangeInsertOrderChangeNotice(PD_ClassifyProduct classifyProduct)
        {
            bool isOrderChange = false;
            var ModelChange = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView().GetTop(1, item => item.OrderItemID == classifyProduct.ID && item.Type == OrderItemChangeType.ProductModelChange).FirstOrDefault();
            if (ModelChange != null)
            {
                isOrderChange = true;
                var orderChangeNotice = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, item => item.OrderID == classifyProduct.OrderID).FirstOrDefault();
                if (orderChangeNotice == null)
                {
                    orderChangeNotice = new OrderChangeNotice() { ID = ChainsGuid.NewGuidUp(), OrderID = classifyProduct.OrderID, Type = OrderChangeType.ArrivalChange, processState = ProcessState.Processing };
                    orderChangeNotice.Enter();
                }

                OrderChangeNoticeLog orderChangeNoticeLog = new OrderChangeNoticeLog();
                orderChangeNoticeLog.ID = ChainsGuid.NewGuidUp();
                orderChangeNoticeLog.OrderChangeNoticeID = orderChangeNotice.ID;
                orderChangeNoticeLog.OrderID = classifyProduct.OrderID;
                orderChangeNoticeLog.OrderItemID = classifyProduct.ID;
                orderChangeNoticeLog.AdminID = classifyProduct.Admin.ID;
                orderChangeNoticeLog.CreateDate = DateTime.Now;
                string summary = "报关员【" + classifyProduct.Admin.ByName + "】,将型号【" + ModelChange.OldValue + "】,修改为【" + ModelChange.NewValue + "】";
                orderChangeNoticeLog.Summary = summary;
                orderChangeNoticeLog.Enter();
            }
            return isOrderChange;
        }

        /// <summary>
        /// 生成订单变更通知  
        /// </summary>
        /// <param name="classifyProduct"></param>
        /// <param name="originalTariff"></param>
        /// <param name="Tariff"></param>
        /// <param name="originalExciseTax"></param>
        /// <param name="ExciseTax"></param>
        /// <param name="originalAddedValueTax"></param>
        /// <param name="AddedValueTax"></param>
        private void InsertOrderChangeNotice(PD_ClassifyProduct classifyProduct, decimal originalTariff, decimal Tariff, decimal originalExciseTax, decimal ExciseTax, decimal originalAddedValueTax, decimal AddedValueTax)
        {
            var orderChangeNotice = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, item => item.OrderID == classifyProduct.OrderID).FirstOrDefault();
            if (orderChangeNotice == null)
            {
                orderChangeNotice = new OrderChangeNotice() { ID = ChainsGuid.NewGuidUp(), OrderID = classifyProduct.OrderID, Type = OrderChangeType.ArrivalChange, processState = ProcessState.Processing };
                orderChangeNotice.Enter();

            }

            OrderChangeNoticeLog orderChangeNoticeLog = new OrderChangeNoticeLog();
            orderChangeNoticeLog.ID = ChainsGuid.NewGuidUp();
            orderChangeNoticeLog.OrderChangeNoticeID = orderChangeNotice.ID;
            orderChangeNoticeLog.OrderID = classifyProduct.OrderID;
            orderChangeNoticeLog.OrderItemID = classifyProduct.ID;
            orderChangeNoticeLog.AdminID = classifyProduct.Admin.ID;
            orderChangeNoticeLog.CreateDate = DateTime.Now;
            string summary = "报关员【" + classifyProduct.Admin.RealName + "】,修改了型号【" + classifyProduct.Model + "】,";
            if (originalTariff != Tariff)
            {
                summary += "关税率【" + originalTariff + "】改为【" + Tariff + "】,";
            }
            if (originalExciseTax != ExciseTax)
            {
                summary += "消费税率【" + originalExciseTax + "】改为【" + ExciseTax + "】,";
            }
            if (originalAddedValueTax != AddedValueTax)
            {
                summary += "增值税率【" + originalAddedValueTax + "】改为【" + AddedValueTax + "】,";
            }
            orderChangeNoticeLog.Summary = summary;
            orderChangeNoticeLog.Enter();
        }

        private void Post2Client(PD_ClassifyProduct classifyProduct)
        {
            var orderChangeNotice = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, item => item.OrderID == classifyProduct.OrderID).FirstOrDefault();

            if (orderChangeNotice == null)
            {
                var productList = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView();
                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState != Needs.Ccs.Services.Enums.ProcessState.Processed;
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == classifyProduct.OrderID;
                lamdas.Add(lambda1);

                var products = productList.GetPageList(1, 20, expression, lamdas.ToArray());

                if (products.Count() >= 1)
                {

                }
                else
                {
                    string[] mainOrderID = classifyProduct.OrderID.Split('-');

                    var Orders = new Needs.Ccs.Services.Views.OrdersViewBase<Order>().Where(item => item.MainOrderID == mainOrderID[0] &&
                                                            item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled &&
                                                            item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned).ToList();

                    List<TinyOrderDeclareFlags> declareFlags = new List<TinyOrderDeclareFlags>();

                    foreach (var order in Orders)
                    {
                        if (order.ID != classifyProduct.OrderID)
                        {
                            TinyOrderDeclareFlags tinyOrder = new TinyOrderDeclareFlags();
                            tinyOrder.TinyOrderID = order.ID;
                            tinyOrder.IsDeclare = false;
                            if (order.DeclareFlag == Needs.Ccs.Services.Enums.DeclareFlagEnums.Able)
                            {
                                tinyOrder.IsDeclare = true;
                            }
                            declareFlags.Add(tinyOrder);
                        }
                    }

                    TinyOrderDeclareFlags thistinyOrder = new TinyOrderDeclareFlags();
                    thistinyOrder.TinyOrderID = classifyProduct.OrderID;
                    thistinyOrder.IsDeclare = true;
                    declareFlags.Add(thistinyOrder);

                    var confirm = new ClientConfirm();
                    confirm.OrderID = mainOrderID[0];
                    confirm.AdminID = classifyProduct.Admin.ErmAdminID;
                    confirm.Type = ConfirmType.DirectConfirm;
                    confirm.DeclareFlags = declareFlags;

                    var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                    var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;

                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = mainOrderID[0],
                        TinyOrderID = classifyProduct.OrderID,
                        Url = apiurl,
                        RequestContent = confirm.Json(),
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    apiLog.Enter();


                    var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, confirm);
                    var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);

                    if (message.code != 200)
                    {
                        OrderLog log = new OrderLog();
                        log.OrderID = classifyProduct.OrderID;
                        //log.Admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
                        log.OrderStatus = Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed;
                        log.Summary = "到货异常处理推送代仓储失败:" + message.data;
                        log.Enter();
                    }

                    var currentOrder = Orders.Where(t => t.ID == classifyProduct.OrderID).FirstOrDefault();
                    currentOrder.GenerateBill(currentOrder.OrderBillType, currentOrder.PointedAgencyFee);
                }
            }
        }

        /// <summary>
        /// 继续归类，获取下一条归类信息
        /// </summary>
        /// <param name="step"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetNext(string step, string creatorId)
        {
            try
            {
                #region 获取归类信息

                ClassifyStep stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                ClassifyStatus targetClassifyStatus = ClassifyStatus.Unclassified;
                if (stepEnum == ClassifyStep.Step1)
                {
                    targetClassifyStatus = ClassifyStatus.Unclassified;
                }
                else if (stepEnum == ClassifyStep.Step2)
                {
                    targetClassifyStatus = ClassifyStatus.First;
                }

                Expression<Func<PD_ClassifyProduct, bool>> expression = item => item.ClassifyStatus == targetClassifyStatus;
                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_ClassifyProduct, DateTime>>)(item => item.CreateDate));

                PD_ClassifyProduct next = new PD_ClassifyProductsAll().GetTop(1, expression, lamdasOrderByAscDateTime.ToArray(), null, creatorId, false).FirstOrDefault();

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

                #region 合同发票

                var order = new Needs.Ccs.Services.Views.OrdersView()[next.OrderID];
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                var t2 = order.CreateDate;
                var pis = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice)
                .ToList().Select(item => new
                {
                    item.ID,
                    FileName = item.Name,
                    item.FileFormat,
                    Url = DateTime.Compare(t2, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                    // Url = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                }).Json();

                #endregion

                #region 特殊类型

                var specialTypes = new Needs.Ccs.Services.Views.OrderVoyagesOriginView().Where(t => t.Order.ID == next.OrderID).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var st in specialTypes)
                {
                    sb.Append(st.Type.GetDescription() + "|");
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
                        ItemID = next.ID,
                        MainID = next.OrderID,
                        OrderedDate = next.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        ClientCode = next.Client.ClientCode,
                        ClientName = next.Client.Company.Name,

                        PartNumber = next.Model,
                        Manufacturer = next.Manufacturer,
                        Origin = next.Origin,
                        UnitPrice = next.UnitPrice.ToString("0.0000"),
                        Quantity = next.Quantity,
                        Unit = next.Unit,
                        Currency = next.Currency,
                        TotalPrice = next.TotalPrice.ToString("0.0000"),
                        CreateDate = next.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                        HSCode = next.Category?.HSCode,
                        TariffName = next.Category?.Name,
                        ImportPreferentialTaxRate = next.ImportTax?.Rate.ToString("0.0000"),//芯达通没有区分优惠税率和加征税率, 将进口关税赋给优惠税率
                        VATRate = next.AddedValueTax?.Rate.ToString("0.0000"),
                        ExciseTaxRate = next.ExciseTax?.Rate.ToString("0.0000"),
                        TaxCode = next.Category?.TaxCode,
                        TaxName = next.Category?.TaxName,
                        LegalUnit1 = next.Category?.Unit1,
                        LegalUnit2 = next.Category?.Unit2,
                        CIQCode = next.Category?.CIQCode,
                        Elements = next.Category?.Elements,

                        OriginATRate = 0,//芯达通没有区分优惠税率和加征税率, 进口关税已经赋给优惠税率, 加征税率为0
                        CIQ = (next.Category?.Type & ItemCategoryType.Inspection) > 0,
                        CIQprice = next.InspectionFee.GetValueOrDefault(),
                        Ccc = (next.Category?.Type & ItemCategoryType.CCC) > 0,
                        Embargo = (next.Category?.Type & ItemCategoryType.Forbid) > 0,
                        HkControl = (next.Category?.Type & ItemCategoryType.HKForbid) > 0,
                        IsHighPrice = (next.Category?.Type & ItemCategoryType.HighValue) > 0,
                        Coo = (next.Category?.Type & ItemCategoryType.OriginProof) > 0,

                        PIs = pis,
                        SpecialType = sb.Length > 0 ? sb.ToString().TrimEnd('|') : "--",
                        PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                        CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.SubmitClassified,
                        NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.GetNext,
                        Step = step,
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
        /// 
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUnitPriceLogs(string partNumber, string orderID = "")
        {
            try
            {
                partNumber = partNumber.FixSpecialChars();

                //2020-12-04 荣检与魏晓毅确认显示近三年的产品报价历史记录
                var productQuotesOrigin = new Needs.Ccs.Services.Views.OrderItemUnitPriceLogsView().
                     Where(t => t.Model == partNumber && t.CreateDate >= DateTime.Now.AddMonths(-36) && t.OrderStatus >= OrderStatus.Declared)
                    .OrderByDescending(pq => pq.CreateDate).ToList();

                var productQuotes = productQuotesOrigin.Where(t => t.OrderStatus != OrderStatus.Returned && t.OrderStatus != OrderStatus.Canceled && t.OrderID != orderID)
                    .Select(pq => new
                    {
                        pq.ID,
                        pq.UnitPrice,
                        pq.Currency,
                        pq.Quantity,
                        pq.ClientName,
                        CreateDate = pq.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });
                if (productQuotes.Count() == 0)
                {
                    if (productQuotes.Count() == 0)
                    {
                        var json100 = new JMessage() { code = 100, success = true, data = null };
                        return ApiResultModel.OutputResult(json100);
                    }
                }

                var json = new
                {
                    code = 200,
                    success = true,
                    data = productQuotes,
                    avgUnitPrice = productQuotes.Average(t => t.UnitPrice)
                };
                return ApiResultModel.OutputResult(json);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return ApiResultModel.OutputResult(json);
            }
        }
    }
}
