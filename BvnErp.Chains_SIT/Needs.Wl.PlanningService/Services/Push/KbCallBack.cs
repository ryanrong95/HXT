using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
//using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class KbCallBack : ApiCallBack
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public KbCallBack() : base("kb")
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
                    partno = cResult.Model,
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

                string postjson = "{\"total_item_num\":1,\"custom_partner\":94,\"item\":[ " + json + "]}";

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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 推送前：构建推送报文

                var changeNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Where(t => t.OderID == decHead.OrderID).FirstOrDefault();
                if (changeNotice != null)
                {
                    //如果有税费变更，且税费变更未处理，则不推送，直接返回
                    if (changeNotice.ProcessState == (int)ProcessState.UnProcess)
                    {
                        return;
                    }
                }

                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<DutiablePriceItem, bool>> expression = item => item.DecHeadID == decHead.ID;

                var Result = new Needs.Ccs.Services.Views.DutiablePriceView<DutiablePriceItem>().GetIQueryableResult(expression);

                List<IcgooDutiablePriceItem> calcedItems = new List<IcgooDutiablePriceItem>();

                foreach (var item in Result)
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
                    m.qty = Convert.ToInt16(item.Qty);
                    calcedItems.Add(m);
                }

                string json = JsonConvert.SerializeObject(new
                {
                    total_item_num = calcedItems.Count(),
                    custom_partner = 94,
                    item = calcedItems
                });

                #endregion

                #region 推送

                string postdata = json.Replace("&", "%26");
                var url = base.ApiSetting.Apis[ApiType.DutiablePrice].Url;
                string msg = string.Format("{0}={1}", "packetdata", postdata);
                string result = PushMsg(url, msg);

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

                List<IcgooDutiablePriceJson> model = JsonConvert.DeserializeObject<List<IcgooDutiablePriceJson>>(result);
                bool isAllSuccess = true;

                foreach (var item in model)
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

        protected override void PushIcgooTariffDiff(OrderItem cResult, string supplier)
        {
            throw new NotImplementedException();
        }
    }
}
