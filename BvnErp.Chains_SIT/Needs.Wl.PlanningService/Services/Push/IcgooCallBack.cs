using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
//using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class IcgooCallBack : ApiCallBack
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public IcgooCallBack() : base("icgoo")
        {

        }

        protected override void PushClassifyResult(PreClassifyProduct cResult)
        {           
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 推送前：构建推送报文

                //Type 转换
                int type = (int)IcgooClassifyTypeEnums.Normal;
                if ((cResult.Type & ItemCategoryType.Inspection) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.Inspection;
                }
                if ((cResult.Type & ItemCategoryType.CCC) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.CCC;
                }
                if ((cResult.Type & ItemCategoryType.HighValue) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.HighValue;
                }
                if ((cResult.Type & ItemCategoryType.HKForbid) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.HKLimit;
                }
                if ((cResult.Type & ItemCategoryType.Forbid) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.Embargo;
                }
              
                string json = JsonConvert.SerializeObject(new
                {
                    id = cResult.ID,
                    sale_orderline_id = cResult.PreProduct.ProductUnionCode,
                    partno = cResult.Model.ToUnicode(),
                    supplier = cResult.PreProduct.Supplier,
                    mfr = cResult.Manufacturer,
                    brand = cResult.Manufacturer,
                    origin = "",
                    customs_rate = cResult.TariffRate,
                    add_rate = cResult.AddedValueRate,
                    product_name = cResult.ProductName.ToUnicode(),
                    category = "",
                    type = type,
                    hs_code = cResult.HSCode,
                    tax_code = cResult.TaxCode,
                });

                string postjson = "{\"total_item_num\":1,\"custom_partner\":"+base.Client.IcgooPartnerNo+",\"item\":[ " + json + "]}";

                #endregion

                #region 推送

                string postdata = postjson.Replace("&", "%26");
                var url = base.ApiSetting.Apis[ApiType.CResult].Url;
                string msg = string.Format("{0}={1}", "packetdata", postdata);
                string result = base.PushMsg(url, msg);

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

                List<PostBack> list = JsonConvert.DeserializeObject<List<PostBack>>(result);
                bool isAllSuccess = true;

                foreach (var item in list)
                {
                    if (item.status == false)
                    {
                        isAllSuccess = false;
                    }
                }

                base.AfterPush(isAllSuccess, postdata, result);

                #endregion
            }
        }

        protected override void PushDutiablePrice(DecHead decHead)
        {
            string postdata = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                try
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

                    List<LambdaExpression> lamdas = new List<LambdaExpression>();
                    Expression<Func<DutiablePriceItem, bool>> expression = item => item.DecHeadID == decHead.ID;

                    var results = new Needs.Ccs.Services.Views.DutiablePriceView<DutiablePriceItem>().GetIQueryableResult(expression);

                    List<IcgooDutiablePriceItem> calcedItems = new List<IcgooDutiablePriceItem>();

                    foreach (var item in results)
                    {
                        IcgooDutiablePriceItem m = new IcgooDutiablePriceItem();
                        m.id = item.OrderItemID;
                        m.sale_orderline_id = item.ProductUniqueCode;
                        m.partno = item.Model;
                        m.supplier = item.Supplier;
                        m.mfr = item.Manfacture;
                        m.brand = item.Manfacture;
                        m.origin = item.Origin;
                        m.customs_rate = CalcAdded(item.Origin, item.HSCode, item.TariffRate)[1];
                        m.origin_tax = CalcAdded(item.Origin, item.HSCode, item.TariffRate)[0];
                        m.add_rate = item.AddedValueRate;
                        m.product_name = item.ProductName.ToUnicode();
                        m.category = "";
                        m.hs_code = item.HSCode;
                        m.tax_code = item.TaxCode;
                        m.qty = Convert.ToInt32(item.Qty);
                        calcedItems.Add(m);
                    }

                    string json = JsonConvert.SerializeObject(new
                    {
                        total_item_num = calcedItems.Count(),
                        custom_partner = base.Client.IcgooPartnerNo,
                        item = calcedItems
                    });

                    #endregion

                    #region 推送

                    postdata = json.Replace("&", "%26");
                    var url = base.ApiSetting.Apis[ApiType.DutiablePrice].Url;
                    string msg = string.Format("{0}={1}", "packetdata", postdata);
                    string result = base.PushMsg(url, msg);

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

                    List<IcgooDutiablePriceJson> list = JsonConvert.DeserializeObject<List<IcgooDutiablePriceJson>>(result);
                    bool isAllSuccess = true;

                    foreach (var item in list)
                    {
                        if (item.status == false)
                        {
                            isAllSuccess = false;
                        }
                    }

                    base.AfterPush(isAllSuccess, postdata, result);

                    #endregion
                }
                catch (Exception ex)
                {
                    //写ApiNotice推送日志
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        ApiNoticeID = base.ApiNotice.ID,
                        PushMsg = postdata,
                        ResponseMsg = "PushFail",
                        CreateDate = DateTime.Now,
                    });
                    //发送邮件
                    //string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //SmtpContext.Current.Send(receivers, "Icgoo推送完税价格失败", ex.ToString());
                }

            }
        }

        /// <summary>
        /// 计算加征税率
        /// </summary>
        /// <param name="origin">原产地</param>
        /// <param name="hsCode">海关编码</param>
        /// <param name="tariffRate">关税率</param>
        /// <returns></returns>
        private decimal[] CalcAdded(string origin, string hsCode, decimal tariffRate)
        {
            var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();

            var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax &&
                                            item.Origin == origin &&
                                            item.CustomsTariffID == hsCode).FirstOrDefault();

            if (reTariff != null)
            {
                return new decimal[2] { reTariff.Rate / 100, tariffRate - reTariff.Rate / 100 };
            }
            else
            {
                return new decimal[2] { 0, tariffRate };
            }
        }

        protected override void PushIcgooTariffDiff(OrderItem cResult,string supplier)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 推送前：构建推送报文

                //Type 转换
                int type = (int)IcgooClassifyTypeEnums.Normal;
                if ((cResult.Category.Type & ItemCategoryType.Inspection) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.Inspection;
                }
                if ((cResult.Category.Type & ItemCategoryType.CCC) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.CCC;
                }
                if ((cResult.Category.Type & ItemCategoryType.HighValue) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.HighValue;
                }
                if ((cResult.Category.Type & ItemCategoryType.HKForbid) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.HKLimit;
                }
                if ((cResult.Category.Type & ItemCategoryType.Forbid) > 0)
                {
                    type = (int)IcgooClassifyTypeEnums.Embargo;
                }

                string json = JsonConvert.SerializeObject(new
                {
                    id = cResult.ID,
                    sale_orderline_id = cResult.ProductUniqueCode,
                    partno = cResult.Model.ToUnicode(),
                    supplier = supplier,
                    mfr = cResult.Manufacturer,
                    brand = cResult.Manufacturer,
                    origin = "",
                    customs_rate = cResult.ImportTax.Rate,
                    add_rate = cResult.AddedValueTax.Rate,
                    product_name = cResult.Name.ToUnicode(),
                    category = "",
                    type = type,
                    hs_code = cResult.Category.HSCode,
                    tax_code = cResult.Category.TaxCode,
                });

                string postjson = "{\"total_item_num\":1,\"custom_partner\":"+base.Client.IcgooPartnerNo+",\"item\":[ " + json + "]}";

                #endregion

                #region 推送

                string postdata = postjson.Replace("&", "%26");
                var url = base.ApiSetting.Apis[ApiType.CResult].Url;
                string msg = string.Format("{0}={1}", "packetdata", postdata);
                string result = base.PushMsg(url, msg);

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

                List<PostBack> list = JsonConvert.DeserializeObject<List<PostBack>>(result);
                bool isAllSuccess = true;

                foreach (var item in list)
                {
                    if (item.status == false)
                    {
                        isAllSuccess = false;
                    }
                }

                base.AfterPush(isAllSuccess, postdata, result);

                #endregion
            }
        }
    }
}
