using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify.Product
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = Convert.ToInt32(Request.QueryString["PageNumber"]),
                PageSize = Convert.ToInt32(Request.QueryString["PageSize"]),
                IsShowLocked = Convert.ToBoolean(Request.QueryString["IsShowLocked"]),
                Model = Convert.ToString(Request.QueryString["Model"]) ?? string.Empty,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ProductName = Convert.ToString(Request.QueryString["ProductName"]) ?? string.Empty,
                HSCode = Convert.ToString(Request.QueryString["HSCode"]) ?? string.Empty,
                LastClassifyTimeBegin = Convert.ToString(Request.QueryString["LastClassifyTimeBegin"]) ?? string.Empty,
                LastClassifyTimeEnd = Convert.ToString(Request.QueryString["LastClassifyTimeEnd"]) ?? string.Empty,

                ClientCode = Convert.ToString(Request.QueryString["ClientCode"]) ?? string.Empty,
                ProductChangeAddTimeBegin = Convert.ToString(Request.QueryString["ProductChangeAddTimeBegin"]) ?? string.Empty,
                ProductChangeAddTimeEnd = Convert.ToString(Request.QueryString["ProductChangeAddTimeEnd"]) ?? string.Empty,
            };
            var item = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];

            //系统判断是否属于禁运产品
            bool isSysForbid = (item.ControlType(item.Model) & ItemCategoryType.Forbid) > 0;
            //系统判断是否需要CCC认证
            bool isSysCCC = (item.ControlType(item.Model) & ItemCategoryType.CCC) > 0;

            //自动归类历史记录
            var categoryDefault = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ProductCategoriesDefaults.Where(pcd => pcd.Model == item.Model).FirstOrDefault();
            bool isDefaultCCC = categoryDefault == null ? isSysCCC : (categoryDefault.Type == null ? (categoryDefault.ClassifyType == IcgooClassifyTypeEnums.CCC) :
                                                                                                  (categoryDefault.Type & ItemCategoryType.CCC) > 0);
            bool isDefaultForbid = categoryDefault == null ? isSysForbid : (categoryDefault.Type == null ? (categoryDefault.ClassifyType == IcgooClassifyTypeEnums.Embargo) :
                                                                                                  (categoryDefault.Type & ItemCategoryType.Forbid) > 0);
            bool isDefaultOriginProof = categoryDefault == null ? false : (categoryDefault.Type == null ? false : (categoryDefault.Type & ItemCategoryType.OriginProof) > 0);
            bool isDefaultInsp = categoryDefault == null ? false : (categoryDefault.Type == null ? (categoryDefault.ClassifyType == IcgooClassifyTypeEnums.Inspection) :
                                                                                                   (categoryDefault.Type & ItemCategoryType.Inspection) > 0);
            bool isDefaultHighValue = categoryDefault == null ? false : (categoryDefault.Type == null ? false : (categoryDefault.Type & ItemCategoryType.HighValue) > 0);

            //var OrderIsCharterBus = WlPlot.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID && t.Type == OrderSpecialType.CharterBus)
            //    .OrderByDescending(t => t.CreateTime).FirstOrDefault();
            //var OrderIsHighValue = WlPlot.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID && t.Type == OrderSpecialType.HighValue)
            //    .OrderByDescending(t => t.CreateTime).FirstOrDefault();
            //var OrderIsInspection = WlPlot.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID && t.Type == OrderSpecialType.Inspection)
            //    .OrderByDescending(t => t.CreateTime).FirstOrDefault();
            //var OrderIsQuarantine = WlPlot.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID && t.Type == OrderSpecialType.Quarantine)
            //    .OrderByDescending(t => t.CreateTime).FirstOrDefault();
            //var OrderIsCCC = WlPlot.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID && t.Type == OrderSpecialType.CCC)
            //    .OrderByDescending(t => t.CreateTime).FirstOrDefault();


            var orderSpecialTypeDatas = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID).ToList();
            List<Enum> orderSpecialTypeEnumsList = new List<Enum>();
            foreach (var oneOrderSpecialType in new OrderSpecialType[] {
                OrderSpecialType.CharterBus, OrderSpecialType.HighValue, OrderSpecialType.Inspection, OrderSpecialType.Quarantine, OrderSpecialType.CCC, })
            {
                var oneData = orderSpecialTypeDatas.Where(t => t.Type == oneOrderSpecialType).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                if (oneData != null)
                {
                    orderSpecialTypeEnumsList.Add(oneOrderSpecialType);
                }
            }
            Enum[] orderSpecialTypeEnumsArray = orderSpecialTypeEnumsList.ToArray();



            //var rate = WlPlot.RealTimeRates.GetIQueryable(item.Currency).FirstOrDefault();

            //计算历史单价的平均值 Begin
            var data = new ProductCategoriesView(item.Model).AsEnumerable().Select(t => new
            {
                t.ID,
                t.Model,
                t.UnitPrice,
                t.CreateDate,
            }).OrderByDescending(t => t.CreateDate).Take(5).ToList();

            decimal avgUnitPrice = 0;
            if (data != null && data.Any())
            {
                avgUnitPrice = data.Average(t => t.UnitPrice.Value);
            }
            //计算历史单价的平均值 End

            //初始化归类产品数据
            this.Model.OrderItem = new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ClientID = item.Client.ID,
                ClientName = item.Client.Company.Name,
                ClientCode = item.Client.ClientCode,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Origin = item.Origin,
                Currency = item.Currency,
                TaxCode = item.Category?.TaxCode ?? categoryDefault?.TaxCode,
                TaxName = item.Category?.TaxName ?? categoryDefault?.TaxName,
                HSCode = item.Category?.HSCode ?? categoryDefault?.HSCode,
                TariffName = item.Category?.Name ?? categoryDefault?.ProductName,
                TariffRate = item.ImportTax?.Rate ?? categoryDefault?.TariffRate / 100,
                ValueAddTaxRate = item.AddedValueTax?.Rate ?? categoryDefault?.AddedValueRate / 100,
                Qty = item.Quantity,
                Unit = item.Unit,
                UnitPrice = item.UnitPrice.ToString("0.0000"),
                AvgUnitPrice = avgUnitPrice,
                TotalPrice = item.TotalPrice.ToString("0.00"),
                //Rate = rate?.Rate,
                Unit1 = item.Category?.Unit1 ?? categoryDefault?.Unit1,
                Unit2 = item.Category?.Unit2 ?? categoryDefault?.Unit2,
                CIQCode = item.Category?.CIQCode ?? categoryDefault?.CIQCode,
                Elements = item.Category?.Elements ?? categoryDefault?.Elements,
                Summary = item.Category?.Summary,

                IsCCC = item.Category == null ? isDefaultCCC : (item.Category.Type & ItemCategoryType.CCC) > 0,
                IsForbid = item.Category == null ? isDefaultForbid : (item.Category.Type & ItemCategoryType.Forbid) > 0,
                IsOriginProof = item.Category == null ? isDefaultOriginProof : (item.Category.Type & ItemCategoryType.OriginProof) > 0,
                IsInsp = item.Category == null ? isDefaultInsp : (item.Category.Type & ItemCategoryType.Inspection) > 0,
                InspFee = item.InspectionFee == null ? categoryDefault?.InspectionFee : item.InspectionFee,
                IsSysForbid = isSysForbid,
                IsSysCCC = isSysCCC,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                IsHighValue = item.Category == null ? isDefaultHighValue : (item.Category.Type & ItemCategoryType.HighValue) > 0, //orderVoyage != null ? true : false,

                //OrderIsCharterBus = OrderIsCharterBus != null,
                //OrderIsHighValue = OrderIsHighValue != null,
                //OrderIsInspection = OrderIsInspection != null,
                //OrderIsQuarantine = OrderIsQuarantine != null,
                //OrderIsCCC = OrderIsCCC != null,

                OrderSpecialTypeDisplay = orderSpecialTypeEnumsArray.GetEnumsDescriptions("|"),
            }.Json();

            var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), Request.QueryString["From"]);
            this.Model.IsReClassify = step == ClassifyStep.ReClassify;
            if (step == ClassifyStep.ReClassify)
            {
                var changeLogs = item.GetChangeLogs().ToList();  //item.GetChangeNotices().ToList();
                var mfrChangeArray = changeLogs.Where(n => n.Type == OrderItemChangeType.BrandChange).OrderByDescending(t => t.CreateDate).ToArray();
                var modelChangeArray = changeLogs.Where(n => n.Type == OrderItemChangeType.ProductModelChange).OrderByDescending(t => t.CreateDate).ToArray();
                var originChangeArray = changeLogs.Where(n => n.Type == OrderItemChangeType.OriginChange).OrderByDescending(t => t.CreateDate).ToArray();
                var hsCodeChangeArray = changeLogs.Where(n => n.Type == OrderItemChangeType.HSCodeChange).OrderByDescending(t => t.CreateDate).ToArray();
                var tariffNameChangeArray = changeLogs.Where(n => n.Type == OrderItemChangeType.TariffNameChange).OrderByDescending(t => t.CreateDate).ToArray();

                this.Model.MfrChange = (mfrChangeArray != null && mfrChangeArray.Length > 0) ? mfrChangeArray : null;
                this.Model.ModelChange = (modelChangeArray != null && modelChangeArray.Length > 0) ? modelChangeArray : null;
                this.Model.OriginChange = (originChangeArray != null && originChangeArray.Length > 0) ? originChangeArray : null;
                this.Model.HsCodeChange = (hsCodeChangeArray != null && hsCodeChangeArray.Length > 0) ? hsCodeChangeArray : null;
                this.Model.TariffNameChange = (tariffNameChangeArray != null && tariffNameChangeArray.Length > 0) ? tariffNameChangeArray : null;


                this.Model.MfrChangeSummary = (mfrChangeArray != null && mfrChangeArray.Length > 0) ? GetChangeSummaryFromArray(mfrChangeArray) : string.Empty;
                this.Model.ModelChangeSummary = (modelChangeArray != null && modelChangeArray.Length > 0) ? GetChangeSummaryFromArray(modelChangeArray) : string.Empty;
                this.Model.OriginChangeSummary = (originChangeArray != null && originChangeArray.Length > 0) ? GetChangeSummaryFromArray(originChangeArray) : string.Empty;
                this.Model.HsCodeChangeSummary = (hsCodeChangeArray != null && hsCodeChangeArray.Length > 0) ? GetChangeSummaryFromArray(hsCodeChangeArray) : string.Empty;
                this.Model.TariffNameChangeSummary = (tariffNameChangeArray != null && tariffNameChangeArray.Length > 0) ? GetChangeSummaryFromArray(tariffNameChangeArray) : string.Empty;


                this.Model.MfrChangeTextJson = GetChangeTextFromArray(mfrChangeArray).Json();
                this.Model.ModelChangeTextJson = GetChangeTextFromArray(modelChangeArray).Json();
                this.Model.OriginChangeTextJson = GetChangeTextFromArray(originChangeArray).Json();
                this.Model.HsCodeChangeTextJson = GetChangeTextFromArray(hsCodeChangeArray).Json();
                this.Model.TariffNameChangeTextJson = GetChangeTextFromArray(tariffNameChangeArray).Json();
            }
            else
            {
                this.Model.MfrChangeTextJson = new object[] { }.Json();
                this.Model.ModelChangeTextJson = new object[] { }.Json();
                this.Model.OriginChangeTextJson = new object[] { }.Json();
                this.Model.HsCodeChangeTextJson = new object[] { }.Json();
                this.Model.TariffNameChangeTextJson = new object[] { }.Json();
            }

            this.Model.CurrentSc = currentSc.Json();
        }

        ///// <summary>
        ///// 从 OrderItemChangeNotice 数组中得出 显示型号信息日志的 List
        ///// </summary>
        ///// <param name="orderItemChangeNotices"></param>
        ///// <param name="strChangeObject"></param>
        ///// <returns></returns>
        //private List<object> GetChangeTextFromArray(OrderItemChangeNotice[] orderItemChangeNotices, string strChangeObject)
        //{
        //    List<object> list = new List<object>();

        //    foreach (var orderItemChangeNotice in orderItemChangeNotices)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendFormat("{0}被 {1} 从【{2}】变更为【{3}】", 
        //            strChangeObject, 
        //            orderItemChangeNotice.TriggerSource.GetDescription(),
        //            orderItemChangeNotice.OldValue,
        //            orderItemChangeNotice.NewValue);
        //        list.Add(new
        //        {
        //            OrderItemChangeNoticeID = orderItemChangeNotice.ID,
        //            ShowText = sb.ToString(),
        //            UpdateDate = orderItemChangeNotice.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //        });
        //    }

        //    return list;
        //}

        private List<object> GetChangeTextFromArray(OrderItemChangeLog[] orderItemChangeLogs)
        {
            List<object> list = new List<object>();

            foreach (var orderItemChangeLog in orderItemChangeLogs)
            {
                list.Add(new
                {
                    OrderItemChangeLogID = orderItemChangeLog.ID,
                    ShowText = orderItemChangeLog.Summary,
                    UpdateDate = orderItemChangeLog.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }

            return list;
        }

        private string GetChangeSummaryFromArray(OrderItemChangeLog[] orderItemChangeLogs)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < orderItemChangeLogs.Length; i++)
            {
                sb.Append("● " + orderItemChangeLogs[i].CreateDate + "，" + orderItemChangeLogs[i].Summary + "。<br>");
            }

            return sb.ToString().Trim();
        }

        // --------------------------------------------------------- 页面加载初始化 就请求的数据 Begin ---------------------------------------------------------

        /// <summary>
        /// 初始化订单附件
        /// </summary>
        protected void dataFiles()
        {
            string id = Request.QueryString["ID"];
            var orderitem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[id];
            if (orderitem != null)
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderitem.OrderID];
                var files = order.Files.Where(file => file.FileType == FileType.OriginalInvoice);
                Func<Needs.Ccs.Services.Models.OrderFile, object> convert = orderFile => new
                {
                    orderFile.ID,
                    orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    orderFile.FileFormat,
                    Url = FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl()
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new { }.Json());
            }
        }

        /// <summary>
        /// 根据产品型号查找海关归类历史纪录
        /// </summary>
        /// <returns></returns>
        protected object GetProductCategories()
        {
            var model = Request.Form["Model"];
            var date = DateTime.Now.AddMonths(-2);
            return new
            {
                data = new ProductCategoriesView(model: model, isModelLike: false).Select(item => new
                {
                    item.ID,
                    item.Model,
                    item.Name,
                    item.HSCode,
                    item.Elements,
                    item.CreateDate
                }).Where(item => item.CreateDate >= date).OrderByDescending(item => item.CreateDate).Take(5)
            };
        }

        /// <summary>
        /// 根据报关品名查找税务归类纪录
        /// </summary>
        protected object GetProductTaxCategories()
        {
            var clientID = Request.Form["ClientID"];
            var name = Request.Form["Name"];
            var date = DateTime.Now.AddMonths(-2);

            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return new MyTaxCategoriesView(clientID, name).Select(item => new
            {
                item.ID,
                item.Name,
                item.TaxCode,
                item.TaxName,
                item.CreateDate
            }).Where(item => item.CreateDate >= date).OrderByDescending(item => item.CreateDate).Take(5);
        }

        /// <summary>
        /// 产品归类日志
        /// </summary>
        /// <returns></returns>
        protected object GetProductClassifyLogs()
        {
            string id = Request.Form["ID"];
            var data = new ProductClassifyLogsView().Where(log => log.ClassifyProductID == id)
                                                    .OrderByDescending(item => item.CreateDate)
                                                     .Select(log => new
                                                     {
                                                         log.ID,
                                                         log.CreateDate,
                                                         Summary = log.OperationLog
                                                     });

            return data;
        }

        /// <summary>
        /// 产品归类变更日志
        /// </summary>
        /// <returns></returns>
        protected object GetProductClassifyChangeLogs()
        {
            string model = Request.Form["Model"];
            var data = new ProductClassifyChangeLogsView().Where(log => log.Model == model)
                                                          .OrderByDescending(item => item.CreateDate)
                                                            .Select(log => new
                                                            {
                                                                log.ID,
                                                                log.CreateDate,
                                                                log.Summary
                                                            });

            return data;
        }

        /// <summary>
        /// 获取申报要素默认值
        /// </summary>
        /// <returns></returns>
        protected string GetElements()
        {
            string hsCode = Request.Form["HScode"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(item => item.HSCode == hsCode.Trim()).FirstOrDefault();

            if (tariff != null)
            {
                return tariff.Elements;
            }
            return null;
        }

        /// <summary>
        /// 获取海关编码
        /// </summary>
        /// <returns></returns>
        protected object GetHSCodes()
        {
            string hsCode = Request.Form["HSCode"];
            string origin = Request.Form["Origin"];

            if (string.IsNullOrWhiteSpace(hsCode))
            {
                return null;
            }

            var tariffs = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(item => item.HSCode.StartsWith(hsCode)).Take(10).ToList();

            List<CustomsOriginTariff> customsOriginTariffs = new List<CustomsOriginTariff>();

            //查询原产地税率
            if (tariffs != null && tariffs.Any())
            {
                var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());

                string[] ids = tariffs.Select(t => t.ID).Distinct().ToArray();

                using (var customsOriginTariffsView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView())
                {
                    customsOriginTariffs = customsOriginTariffsView
                        .Where(item => item.Type == CustomsRateType.ImportTax
                                    && item.Origin == origin
                                    && item.StartDate <= curDate && (item.EndDate == null || item.EndDate >= curDate)
                                    && item.Status == Needs.Ccs.Services.Enums.Status.Normal
                                    && ids.Contains(item.CustomsTariffID))
                        .ToList();
                }
            }

            List<CustomsOriginTariff> resultCustomsOriginTariffs = new List<CustomsOriginTariff>();

            foreach (var tariff in tariffs)
            {
                var currentCustomsOriginTariff = customsOriginTariffs.Where(t => t.CustomsTariffID == tariff.ID).FirstOrDefault();

                if (currentCustomsOriginTariff != null)
                {
                    resultCustomsOriginTariffs.Add(currentCustomsOriginTariff);
                }
            }

            var linq = from tariff in tariffs
                       join resultCustomsOriginTariff in resultCustomsOriginTariffs
                       on tariff.ID equals resultCustomsOriginTariff.CustomsTariffID into resultCustomsOriginTariffs2
                       from resultCustomsOriginTariff in resultCustomsOriginTariffs2.DefaultIfEmpty()
                       select new
                       {
                           ID = tariff.HSCode,
                           HSCode = tariff.HSCode,
                           Name = tariff.Name,
                           TariffRate = (resultCustomsOriginTariff == null || string.IsNullOrEmpty(resultCustomsOriginTariff.CustomsTariffID))
                                            ? tariff.MFN / 100 : (resultCustomsOriginTariff.Rate + tariff.MFN) / 100,
                           ValueAddTaxRate = tariff.AddedValue / 100,
                           Unit1 = tariff.Unit1,
                           Unit2 = tariff.Unit2,
                       };

            return linq;
        }

        // --------------------------------------------------------- 页面加载初始化 就请求的数据 End ---------------------------------------------------------

        /// <summary>
        /// 验证当前型号的归类结果与之前的归类历史记录是否一致
        /// 如果不一致返回信息提醒报关员
        /// </summary>
        protected void ClassifyCheck()
        {
            try
            {
                //TODO：代码待优化
                string Entity = HttpUtility.UrlDecode(Request.Form["Model"]).Replace("&quot;", "\'").Replace("amp;", "");
                dynamic entity = Entity.JsonTo<dynamic>();
                string model = (string)(entity.ModelText);
                var historyCategory = new ProductCategoriesDefaultsView().Where(c => c.Model == model).FirstOrDefault();

                if (historyCategory == null)
                {
                    Response.Write((new { pass = true, message = "验证成功" }).Json());
                }
                else
                {
                    string msg = "";
                    bool isPassed = true;

                    #region 验证归类结果

                    if (entity.Manufacturer != historyCategory.Manufacturer)
                    {
                        isPassed = false;
                        msg += "品牌：当前归类<label style=\"color:green\">【" + entity.Manufacturer + "】</label>，" +
                                    "历史纪录<label style=\"color:red\">【" + historyCategory.Manufacturer + "】</label><br/><br/>";
                    }
                    if (entity.HSCode != historyCategory.HSCode)
                    {
                        isPassed = false;
                        msg += "海关编码：当前归类<label style=\"color:green\">【" + entity.HSCode + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.HSCode + "】</label><br/><br/>";
                    }
                    if (entity.TariffNameText != historyCategory.ProductName)
                    {
                        isPassed = false;
                        msg += "报关品名：当前归类<label style=\"color:green\">【" + entity.TariffNameText + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.ProductName + "】</label><br/><br/>";
                    }
                    if (entity.TaxCode != historyCategory.TaxCode)
                    {
                        isPassed = false;
                        msg += "税务编码：当前归类<label style=\"color:green\">【" + entity.TaxCode + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.TaxCode + "】</label><br/><br/>";
                    }
                    if (entity.TaxName != historyCategory.TaxName)
                    {
                        isPassed = false;
                        msg += "税务名称：当前归类<label style=\"color:green\">【" + entity.TaxName + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.TaxName + "】</label><br/><br/>";
                    }
                    if ((decimal)entity.TariffRateText != historyCategory.TariffRate / 100)
                    {
                        isPassed = false;
                        msg += "关税率：当前归类<label style=\"color:green\">【" + entity.TariffRateText + "】</label>，" +
                                     " 历史纪录<label style=\"color:red\">【" + historyCategory.TariffRate / 100 + "】</label><br/><br/>";
                    }
                    if ((decimal)entity.ValueAddTaxRateText != historyCategory.AddedValueRate / 100)
                    {
                        isPassed = false;
                        msg += "增值税率：当前归类<label style=\"color:green\">【" + entity.ValueAddTaxRateText + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.AddedValueRate / 100 + "】</label><br/><br/>";
                    }
                    if (entity.Unit1 != historyCategory.Unit1)
                    {
                        isPassed = false;
                        msg += "法定第一单位：当前归类<label style=\"color:green\">【" + entity.Unit1 + "】</label>，" +
                                          " 历史纪录<label style=\"color:red\">【" + historyCategory.Unit1 + "】</label><br/><br/>";
                    }
                    if ((string)entity.Unit2 != (historyCategory.Unit2 ?? ""))
                    {
                        isPassed = false;
                        msg += "法定第二单位：当前归类<label style=\"color:green\">【" + entity.Unit2 + "】</label>，" +
                                          " 历史纪录<label style=\"color:red\">【" + historyCategory.Unit2 + "】</label><br/><br/>";
                    }
                    if (entity.CIQCodeText != historyCategory.CIQCode)
                    {
                        isPassed = false;
                        msg += "检验检疫编码：当前归类<label style=\"color:green\">【" + entity.CIQCodeText + "】</label>，" +
                                          " 历史纪录<label style=\"color:red\">【" + historyCategory.CIQCode + "】</label><br/><br/>";
                    }
                    if (entity.Elements != historyCategory.Elements)
                    {
                        isPassed = false;
                        msg += "申报要素：当前归类<label style=\"color:green\">【" + entity.Elements + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + historyCategory.Elements + "】</label><br/><br/>";
                    }

                    bool isDefaultCCC = (historyCategory.Type == null ? (historyCategory.ClassifyType == IcgooClassifyTypeEnums.CCC) :
                                                                        (historyCategory.Type & ItemCategoryType.CCC) > 0);
                    bool isDefaultOriginProof = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.OriginProof) > 0);
                    bool isDefaultInsp = (historyCategory.Type == null ? (historyCategory.ClassifyType == IcgooClassifyTypeEnums.Inspection) :
                                                                         (historyCategory.Type & ItemCategoryType.Inspection) > 0);
                    bool isDefaultHighValue = historyCategory.Type == null ? (historyCategory.ClassifyType == IcgooClassifyTypeEnums.HighValue) :
                                                                             (historyCategory.Type & ItemCategoryType.HighValue) > 0;
                    bool isDefaultForbid = historyCategory.Type == null ? (historyCategory.ClassifyType == IcgooClassifyTypeEnums.Embargo) :
                                                                          (historyCategory.Type & ItemCategoryType.Forbid) > 0;

                    if ((bool)entity.IsCCC != isDefaultCCC)
                    {
                        isPassed = false;
                        msg += "CCC认证：当前归类<label style=\"color:green\">【" + ((bool)entity.IsCCC ? "是" : "否") + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.CCC) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }
                    if ((bool)entity.IsOriginProof != isDefaultOriginProof)
                    {
                        isPassed = false;
                        msg += "原产地证明：当前归类<label style=\"color:green\">【" + ((bool)entity.IsOriginProof ? "是" : "否") + "】</label>，" +
                                        " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.OriginProof) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }
                    if ((bool)entity.IsInsp != isDefaultInsp)
                    {
                        isPassed = false;
                        msg += "是否商检：当前归类<label style=\"color:green\">【" + ((bool)entity.IsInsp ? "是" : "否") + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.Inspection) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }

                    if ((bool)entity.IsInsp)
                    {
                        if ((decimal)entity.InspFeeText != historyCategory.InspectionFee.GetValueOrDefault())
                        {
                            isPassed = false;
                            msg += "商检费：当前归类<label style=\"color:green\">【" + entity.InspFeeText + "】</label>，" +
                                          " 历史纪录<label style=\"color:red\">【" + historyCategory.InspectionFee + "】</label><br/><br/>";
                        }
                    }

                    if ((bool)entity.IsHighValue != isDefaultHighValue)
                    {
                        isPassed = false;
                        msg += "高价值产品：当前归类<label style=\"color:green\">【" + ((bool)entity.IsHighValue ? "是" : "否") + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.HighValue) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }
                    if ((bool)entity.IsForbid != isDefaultForbid)
                    {
                        isPassed = false;
                        msg += "禁运：当前归类<label style=\"color:green\">【" + ((bool)entity.IsForbid ? "是" : "否") + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.Forbid) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }

                    #endregion

                    if (!isPassed)
                    {
                        msg = "该型号的以下归类结果与历史纪录不一致，请仔细核对！<br/>" +
                              "点击“<label style=\"color:green\">确定</label>”完成归类，点击“<label style=\"color:red\">取消</label>”继续修改<br/><br/>" + msg;
                    }

                    Response.Write((new { pass = isPassed, message = msg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { pass = false, message = "验证失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 一个订单(报关单)中同一产品型号的海关编码必须一致
        /// </summary>
        protected void ModelAndHSCodeCheck()
        {
            string id = Request.Form["OrderItemID"];
            string orderID = Request.Form["OrderID"];
            string model = Request.Form["Model"];
            string hsCode = Request.Form["HSCode"];

            List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
            lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var view = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetTop(50, t => t.OrderID == orderID, lamdasOrderByAscDateTime.ToArray(), null);
            var entity = view.FirstOrDefault(p => p.ID != id && p.Model == model && (p.Category != null && p.Category.HSCode != hsCode));

            if (entity == null)
            {
                Response.Write((new { matched = true, message = "验证成功" }).Json());
            }
            else
            {
                Response.Write((new
                {
                    matched = false,
                    message = "同一订单中相同产品型号的海关编码归类结果必须一致，请仔细核对！<br/><br/>" +
                    "该型号【" + model + "】的海关编码已经被归类为<label style=\"color:green\">【" + entity.Category.HSCode + "】</label>，不能再修改为<label style=\"color:red\">【" + hsCode + "】</label>！"
                }).Json());
            }
        }

        /// <summary>
        /// 根据产品型号查找历史纪录
        /// </summary>
        /// <returns></returns>
        protected object GetHistoryCategories()
        {
            var model = Request.Form["Model"];
            var date = DateTime.Now.AddMonths(-2);

            var data = new ProductCategoriesView(model).AsEnumerable().Select(item => new
            {
                item.ID,
                item.Model,
                item.Name,
                item.TariffRate,
                item.AddedValueRate,
                item.UnitPrice,
                item.InspectionFee,
                item.Qty,
                CreateDateValue = item.CreateDate,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                IsInspection = item.InspectionFee == null ? "否" : "是",
            }).Where(item => item.CreateDateValue >= date).OrderByDescending(item => item.CreateDate).Take(5).ToList();

            decimal avgUnitPrice = 0;
            if (data != null && data.Any())
            {
                avgUnitPrice = data.Average(t => t.UnitPrice.Value);
            }

            return new
            {
                total = data.Count,
                rows = data,
                avgUnitPrice = avgUnitPrice,
            };
        }

        /// <summary>
        /// 归类
        /// </summary>
        protected void Classify()
        {
            try
            {
                string Model = HttpUtility.UrlDecode(Request.Form["Model"]).Replace("&quot;", "\'").Replace("amp;", "");
                dynamic model = Model.JsonTo<dynamic>();

                //初始化
                string id = model.ID;
                ClassifyStep step = (ClassifyStep)model.From;
                ClassifyProduct item = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];

                //如果从已完成列表进入编辑页面，归类操作点击时订单已被报价，则解锁，然后退回到列表
                if (step == ClassifyStep.DoneEdit && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted)
                {
                    item.UnLock();
                    Response.Write((new { success = false, needReturn = true, message = "该订单已报价！", }).Json());
                    return;
                }

                var declarant = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

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

                //归类类型
                item.Category.OrderItemID = item.ID;
                item.Category.Declarant = declarant;
                item.Category.TaxCode = model.TaxCode;
                item.Category.TaxName = model.TaxName;
                item.Category.HSCode = model.HSCode;
                item.Category.Name = model.TariffNameText;
                item.Category.Elements = model.Elements;
                item.Category.Unit1 = model.Unit1;
                if (model.Unit2 != null && model.Unit2 != "")
                {
                    item.Category.Unit2 = model.Unit2;
                }
                item.Category.CIQCode = model.CIQCodeText;
                if (model.Summary != null && model.Summary != "")
                {
                    item.Category.Summary = model.Summary;
                }
                item.Category.Type = ItemCategoryType.Normal;
                if ((bool)model.IsInsp)
                {
                    item.Category.Type |= ItemCategoryType.Inspection;
                }
                if ((bool)model.IsCCC)
                {
                    item.Category.Type |= ItemCategoryType.CCC;
                }
                if ((bool)model.IsOriginProof)
                {
                    item.Category.Type |= ItemCategoryType.OriginProof;
                }
                if ((bool)model.IsSysForbid || (bool)model.IsForbid)
                {
                    item.Category.Type |= ItemCategoryType.Forbid;
                }
                if ((bool)model.IsHighValue)
                {
                    item.Category.Type |= ItemCategoryType.HighValue;
                }
                //如果来自疫区，则为归类类型添加“检疫”
                if (model.Origin != null && model.Origin != "")
                {
                    string origin = model.Origin;
                    var quarantines = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsQuarantines;
                    var quarantine = quarantines.Where(cq => cq.Origin == origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                    if (quarantine != null)
                    {
                        item.Category.Type |= ItemCategoryType.Quarantine;
                    }
                }

                //类型：产品变更  
                //税费变化
                if (step == ClassifyStep.ReClassify && (item.ImportTax.Rate != (decimal)model.TariffRateText || item.AddedValueTax.Rate != (decimal)model.ValueAddTaxRateText))
                {
                    item.Classified += InsertOrderChangeNotice;
                }

                //关税率
                item.ImportTax.OrderItemID = item.ID;
                item.ImportTax.Type = CustomsRateType.ImportTax;
                item.ImportTax.Rate = model.TariffRateText;
                item.ImportTax.ReceiptRate = item.ImportTax.Rate; //ReceiptRate 这个字段也要保存关税率

                //增值税率
                item.AddedValueTax.OrderItemID = item.ID;
                item.AddedValueTax.Type = CustomsRateType.AddedValueTax;
                item.AddedValueTax.Rate = model.ValueAddTaxRateText;
                item.AddedValueTax.ReceiptRate = item.AddedValueTax.Rate; //ReceiptRate 这个字段也要保存增值税率

                //归类管控
                item.IsCCC = model.IsCCC;
                item.IsForbid = model.IsForbid;
                item.IsOriginProof = model.IsOriginProof;
                item.IsSysForbid = model.IsSysForbid;
                item.IsSysCCC = model.IsSysCCC;
                item.IsHighValue = model.IsHighValue;

                //商检
                bool isInsp = model.IsInsp;
                if (isInsp)
                {
                    item.IsInsp = true;
                    item.InspectionFee = model.InspFeeText;
                }
                else
                {
                    item.IsInsp = false;
                    item.InspectionFee = null;
                }

                //品牌、型号变更
                item.Manufacturer = model.Manufacturer;
                item.Model = model.ModelText;
                item.Unit = model.Unit;

                item.Admin = declarant;
                var classify = ClassifyFactory.Create(step, item);
                classify.DoClassify();

                Response.Write((new { success = true, message = "产品归类成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "产品归类失败：" + ex.Message }).Json());
            }
        }

        private void InsertOrderChangeNotice(object sender, ProductClassifiedEventArgs e)
        {
            var classifyProduct = (ClassifyProduct)e.Product;
            var orderChangeNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderChanges.GetTop(1, item => item.OrderID == classifyProduct.OrderID).FirstOrDefault();
            if (orderChangeNotice == null)
            {
                orderChangeNotice = new OrderChangeNotice() { ID = ChainsGuid.NewGuidUp(), OrderID = classifyProduct.OrderID, Type = OrderChangeType.ProductChange, processState = ProcessState.Processing };
                orderChangeNotice.Enter();
            }
        }

        /// <summary>
        /// 继续进入归类详情页面（选取下一个，未被人锁定或自己锁定的）
        /// </summary>
        protected void ContinueEdit()
        {
            string strFromValue = Request.Form["From"];
            int intFromValue = int.Parse(strFromValue);

            ClassifyStep step = (ClassifyStep)Enum.ToObject(typeof(ClassifyStep), intFromValue);

            //找出该归类类型中，下一个，未被人锁定或自己锁定的

            ClassifyStatus targetClassifyStatus = ClassifyStatus.Unclassified;
            if (step == ClassifyStep.Step1)
            {
                targetClassifyStatus = ClassifyStatus.Unclassified;
            }
            else if (step == ClassifyStep.Step2)
            {
                targetClassifyStatus = ClassifyStatus.First;
            }

            Expression<Func<ClassifyProduct, bool>> expression = item => item.ClassifyStatus == targetClassifyStatus;

            List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
            lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var product = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetTop(
                top: 1,
                expression: expression,
                orderByAscDateTimeExpressions: lamdasOrderByAscDateTime.ToArray(),
                orderByDescDateTimeExpressions: null,
                currentAdminID: Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                isShowLocked: false).FirstOrDefault();

            if (product == null)
            {
                Response.Write((new { success = false, message = "已没有 未被他人锁定或被您锁定的型号" }).Json());
            }
            else
            {
                Response.Write((new { success = true, message = "", OrderItemID = product.ID, }).Json());
            }
        }

    }
}