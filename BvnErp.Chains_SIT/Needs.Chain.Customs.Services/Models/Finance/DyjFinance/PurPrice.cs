using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Underly;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 外部公司(Icgoo和外单)进价推送华芯通出入库系统
    /// </summary>
    public class PurPrice
    {

        public PurPrice()
        {

        }


        public void PushPurPrice()
        {

            var notices = new List<ApiNotice>();

            //状态改为推送中
            using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var view = new Views.Origins.ApiNoticesOrigin(responsitory).Where(t => t.PushType == Enums.PushType.PurchasePrice && t.PushStatus == Enums.PushStatus.Unpush);

                notices = view.ToList();
                var decIDs = notices.Select(t => t.ItemID).ToArray();

                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                            new
                            {
                                UpdateDate = DateTime.Now,
                                PushStatus = (int)PushStatus.Pushing
                            }, item => item.PushStatus == (int)PushStatus.Unpush && item.PushType == (int)Enums.PushType.PurchasePrice && decIDs.Contains(item.ItemID));
            }

            Task.Run(() =>
            {
                var decIds = notices.Select(t => t.ItemID).ToArray();

                var decheads = new Views.DecHeadsView().Where(t => decIds.Contains(t.ID) && t.IsSuccess).ToArray();
                var orderIDs = decheads.Select(t => t.OrderID).ToArray();
                var orderOrigins = new Views.OrdersView().Where(t => orderIDs.Contains(t.ID)).ToArray();

                foreach (var notice in notices)
                {
                    #region 推送

                    try
                    {
                        //按单生成推送结构

                        var model = new PurPriceData();
                        var dechead = decheads.FirstOrDefault(t => t.ID == notice.ItemID);
                        var order = orderOrigins.FirstOrDefault(t => t.ID == dechead.OrderID);
                        var taxFlow = new Views.DecTaxFlowsView().Where(t => t.DecheadID == dechead.ID && t.TaxType == DecTaxType.AddedValueTax).FirstOrDefault();
                        //订单项信息
                        var orderitemList = new Needs.Ccs.Services.Views.OrderItemsView().Where(t => t.OrderID == order.ID).ToArray();

                        model.入库日期 = dechead.DDate?.ToString("yyyy-MM-dd");
                        model.销方公司 = dechead.AgentName;
                        model.委托公司 = "";
                        model.购方公司 = order.Client.Company.Name;
                        model.发票类型 = order.ClientAgreement.InvoiceType == InvoiceType.Service ? "双抬头" : "单抬头";
                        model.发票取得日期 = dechead.DDate?.ToString("yyyy-MM-dd");
                        model.发票号 = taxFlow == null ? dechead.EntryId : taxFlow.TaxNumber;

                        //入库明细
                        var Items = new List<PurPriceItem>();
                        var list = dechead.Lists;
                        //合计应交关税
                        var TotalImportTax = 0M;

                        foreach (var item in list)
                        {
                            var product = new PurPriceItem();
                            var orderitem = orderitemList.FirstOrDefault(t => t.ID == item.OrderItemID);
                            //报关总价RMB
                            var totalPrice = (item.DeclTotal * order.CustomsExchangeRate.Value).ToRound(2);
                            //应交关税
                            decimal ImportTax = (totalPrice * orderitem.ImportTax.Rate).ToRound(2);

                            product.型号 = item.GoodsModel;
                            product.商品名称 = item.GName;
                            product.单位 = "个";
                            product.数量 = item.GQty;
                            //Round(Round(美金*海关汇率,2) / 1.002, 2) +应交关税
                            product.报关公司进价 = (totalPrice / ConstConfig.TransPremiumInsurance).ToRound(2) + ImportTax;
                            //Round(美金*海关汇率,2) +应交关税
                            product.完税价格 = totalPrice + ImportTax;
                            product.税率 = 0.13M;//order.ClientAgreement.InvoiceTaxRate;//此处改为统一使用0.13作为财务版税率
                            product.库存ID = item.DecListID;
                            product.备注 = "";
                            product.型号信息分类 = orderitem.Category.TaxCode;
                            product.型号信息分类值 = orderitem.Category.TaxName;

                            TotalImportTax += ImportTax;

                            Items.Add(product);
                        }

                        //处理最后一项的华芯通进价，使用减法。
                        var DecHeadPrice = list.Sum(t => t.DeclTotal);
                        //差额 = 整单的进价 - 项合计进价
                        //整单的进价
                        var inTotal = ((DecHeadPrice * order.CustomsExchangeRate.Value).ToRound(2) / ConstConfig.TransPremiumInsurance).ToRound(2) + TotalImportTax;
                        var differ = inTotal - Items.Sum(t => t.报关公司进价);
                        //最后一项补上差额
                        Items.Last().报关公司进价 = Items.Last().报关公司进价 + differ;

                        model.入库明细s = Items;
                        model.备注 = "";

                        var SendJson = new
                        {
                            request_service = PurPriceApiSetting.request_service,
                            request_item = PurPriceApiSetting.request_item,
                            token = PurPriceApiSetting.token,
                            data = model,
                        };

                        var apiurl = System.Configuration.ConfigurationManager.AppSettings[PurPriceApiSetting.ApiName] + PurPriceApiSetting.PuePriceHandle;

                        var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, SendJson);
                        var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);

                        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            //写ApiNotice推送日志
                            responsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                            {
                                ID = ChainsGuid.NewGuidUp(),
                                ApiNoticeID = notice.ID,
                                PushMsg = SendJson.Json(),
                                ResponseMsg = result,
                                CreateDate = DateTime.Now,
                            });

                            //成功，更新apiNotice
                            if (jResult.success)
                            {
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    UpdateDate = DateTime.Now,
                                    PushStatus = (int)PushStatus.Pushed
                                }, item => item.ID == notice.ID);
                            }
                            else
                            {
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    UpdateDate = DateTime.Now,
                                    PushStatus = (int)PushStatus.PushFailure
                                }, item => item.ID == notice.ID);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.CcsLog("进价推送大赢家错误：" + notice.ItemID);
                        continue;
                    }
                    #endregion
                }
            });

        }

    }


    public class PurPriceModel
    {
        public string request_service { get; set; }
        public string request_item { get; set; }
        public string token { get; set; }
        public PurPriceData data { get; set; }
    }

    public class PurPriceData
    {

        public string 入库日期 { get; set; }

        public string 销方公司 { get; set; }

        public string 委托公司 { get; set; }

        public string 购方公司 { get; set; }

        public string 发票类型 { get; set; }

        public string 发票号 { get; set; }

        public string 发票取得日期 { get; set; }

        public List<PurPriceItem> 入库明细s { get; set; }

        public string 备注 { get; set; }
    }

    public class PurPriceItem
    {
        public string 型号 { get; set; }
        public string 商品名称 { get; set; }
        public string 单位 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 报关公司进价 { get; set; }
        public decimal 完税价格 { get; set; }
        public decimal 税率 { get; set; }
        public string 库存ID { get; set; }
        public string 备注 { get; set; }
        public string 型号信息分类 { get; set; }
        public string 型号信息分类值 { get; set; }

    }
}
