using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify.PreProduct
{
    /// <summary>
    /// 归类编辑界面
    /// </summary>
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
            };
            var item = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];

            //系统判断是否属于禁运产品
            bool isSysForbid = (item.ControlType(item.Model) & ItemCategoryType.Forbid) > 0;
            //系统判断是否需要CCC认证
            bool isSysCCC = (item.ControlType(item.Model) & ItemCategoryType.CCC) > 0;

            //自动归类历史记录
            var categoryDefault = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ProductCategoriesDefaults.Where(pcd => pcd.Model == item.Model).FirstOrDefault();
            bool isDefaultCCC = categoryDefault == null ? isSysCCC : ((categoryDefault.Type & ItemCategoryType.CCC) > 0);
            bool isDefaultOriginProof = categoryDefault == null ? false : ((categoryDefault.Type & ItemCategoryType.OriginProof) > 0);
            bool isDefaultInsp = categoryDefault == null ? false : ((categoryDefault.Type & ItemCategoryType.Inspection) > 0);
            bool isDefaultHighValue = categoryDefault == null ? false : ((categoryDefault.Type & ItemCategoryType.HighValue) > 0);
            bool isDefaultHKForbidden = categoryDefault == null ? false : ((categoryDefault.Type & ItemCategoryType.HKForbid) > 0);

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
                ClientID = item.PreProduct.Client.ID,
                ClientCode = item.PreProduct.Client.ClientCode,
                ClientName = item.PreProduct.Client.Company.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Qty = item.PreProduct.Qty,
                UnitPrice = item.PreProduct.Price,
                AvgUnitPrice = avgUnitPrice,
                Currency = item.PreProduct.Currency,
                HSCode = item.HSCode ?? categoryDefault?.HSCode,
                TariffName = item.ProductName ?? categoryDefault?.ProductName,

                InspectionFee = item.InspectionFee ?? categoryDefault?.InspectionFee,
                TaxCode = item.TaxCode ?? categoryDefault?.TaxCode,
                TaxName = item.TaxName ?? categoryDefault?.TaxName,
                TariffRate = item.TariffRate ?? categoryDefault?.TariffRate / 100,
                ValueAddTaxRate = item.AddedValueRate ?? categoryDefault?.AddedValueRate / 100,
                Unit1 = item.Unit1 ?? categoryDefault?.Unit1,
                Unit2 = item.Unit2 ?? categoryDefault?.Unit2,
                CIQCode = item.CIQCode ?? categoryDefault?.CIQCode,
                Elements = item.Elements ?? categoryDefault?.Elements,
                Summary = item.Summary,
                ProductUnionCode = item.PreProduct.ProductUnionCode,

                IsSysForbid = isSysForbid,
                IsSysCCC = isSysCCC,

                IsCCC = item.Type == null ? isDefaultCCC : (item.Type & ItemCategoryType.CCC) > 0,
                IsOriginProof = item.Type == null ? isDefaultOriginProof : (item.Type & ItemCategoryType.OriginProof) > 0,
                IsInsp = item.Type == null ? isDefaultInsp : (item.Type & ItemCategoryType.Inspection) > 0,
                IsHighValue = item.Type == null ? isDefaultHighValue : (item.Type & ItemCategoryType.HighValue) > 0,
                IsForbidden = item.Type == null ? isSysForbid : (item.Type & ItemCategoryType.Forbid) > 0,
                IsHKForbidden = item.Type == null ? isDefaultHKForbidden : (item.Type & ItemCategoryType.HKForbid) > 0,
            }.Json();

            this.Model.CurrentSc = currentSc.Json();
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
                PreClassifyProduct item = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];

                var orderItemsOrigin = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin();
                int countUnionCodeCountInOrderItems = orderItemsOrigin
                    .Where(t => t.ProductUniqueCode == item.ProductUnionCode
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .Count();

                //如果从产品预归类已完成列表进入编辑页面，归类操作点击时，接口已经下单，则解锁，然后退回到列表
                if (step == ClassifyStep.PreDoneEdit && countUnionCodeCountInOrderItems > 0)
                {
                    item.UnLock();
                    Response.Write((new { success = false, needReturn = true, message = "接口已经下单！", }).Json());
                    return;
                }

                var declarant = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //归类类型
                item.Admin = declarant;
                item.TaxCode = model.TaxCode;
                item.TaxName = model.TaxName;
                item.HSCode = model.HSCode;
                item.ProductName = model.TariffNameText;
                item.Elements = model.Elements;
                item.Summary = model.Summary;
                item.Manufacturer = model.Manufacturer;
                item.Unit1 = model.Unit1;
                if (model.Unit2 != null && model.Unit2 != "")
                {
                    item.Unit2 = model.Unit2;
                }
                item.CIQCode = model.CIQCodeText;
                if (model.Summary != null && model.Summary != "")
                {
                    item.Summary = model.Summary;
                }
                item.Type = ItemCategoryType.Normal;
                if ((bool)model.IsInsp)
                {
                    item.Type |= ItemCategoryType.Inspection;
                }
                if ((bool)model.IsCCC)
                {
                    item.Type |= ItemCategoryType.CCC;
                }
                if ((bool)model.IsOriginProof)
                {
                    item.Type |= ItemCategoryType.OriginProof;
                }
                if ((bool)model.IsSysForbid)
                {
                    item.Type |= ItemCategoryType.Forbid;
                }
                if ((bool)model.IsHighValue)
                {
                    item.Type |= ItemCategoryType.HighValue;
                }
                if ((bool)model.IsForbidden)
                {
                    item.Type |= ItemCategoryType.Forbid;
                }
                if ((bool)model.IsHKForbidden)
                {
                    item.Type |= ItemCategoryType.HKForbid;
                }

                //关税率
                item.TariffRate = model.TariffRateText;

                //增值税率
                item.AddedValueRate = model.ValueAddTaxRateText;

                //归类管控
                item.IsCCC = model.IsCCC;
                item.IsOriginProof = model.IsOriginProof;
                item.IsSysForbid = model.IsSysForbid;
                item.IsSysCCC = model.IsSysCCC;
                item.IsHighValue = model.IsHighValue;
                item.IsForbid = model.IsForbidden;
                item.IsHKForbid = model.IsHKForbidden;

                //商检
                bool isInsp = model.IsInsp;
                if (isInsp)
                {
                    item.IsInsp = true;
                    item.InspectionFee = model.InspFeeText == "" ? 0M : model.InspFeeText;
                }
                else
                {
                    item.IsInsp = false;
                }
                item.Status = Status.Normal;
                var classify = ClassifyFactory.Create(step, item);
                classify.DoClassify();

                Response.Write((new { success = true, message = "产品归类成功！" }).Json());


            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "产品归类失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 产品归类成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Product_ClassifySuccess(object sender, SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "产品归类成功！" }).Json());
        }

        /// <summary>
        /// 异常处理成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Product_ClassifyAnomalySuccess(object sender, ProductClassifiedEventArgs e)
        {
            Response.Write((new { success = true, message = "归类异常处理成功！" }).Json());
        }

        /// <summary>
        /// 根据产品型号查找海关归类历史纪录
        /// </summary>
        /// <returns></returns>
        protected object GetProductCategories()
        {
            var model = Request.Form["Model"];
            ProductControlsView view = new ProductControlsView(model);
            var productControls = view.ToList();
            return new
            {
                isSysForbid = productControls.Where(c => c.Type == ProductControlType.Forbid).Count() > 0,
                isSysCCC = productControls.Where(c => c.Type == ProductControlType.CCC).Count() > 0,
                data = new ProductCategoriesView(model: model, isModelLike: false).Select(item => new
                {
                    item.ID,
                    item.Model,
                    item.Name,
                    item.HSCode,
                    item.Elements
                })
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
                Name = name,
                item.TaxCode,
                item.TaxName,
                item.CreateDate
            }).Where(item => item.CreateDate >= date).OrderByDescending(item => item.CreateDate).Take(5); ;
        }

        /// <summary>
        /// 计算关税
        /// 公式: Round(报关总价 * 海关汇率 * 1.002, 0) * 关税率
        /// </summary>
        /// <returns></returns>
        [Obsolete("归类时不再计算关税，改为报价时计算，该方法保留以防以后业务变更")]
        protected decimal CalcTariff()
        {
            var totalPrice = Convert.ToDecimal(Request.Form["TotalPrice"]);
            var currency = Request.Form["Currency"];
            var tariffRate = Convert.ToDecimal(Request.Form["TariffRate"]);

            //如果是人民币，则海关汇率为1
            if (currency == MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.Currency>(Needs.Ccs.Services.Enums.Currency.CNY))
            {
                return Rounding(Rounding(totalPrice * 1 * ConstConfig.TransPremiumInsurance, 0) * tariffRate, 2);
            }
            else
            {
                //获取币种的海关汇率
                var customRate = Needs.Wl.Admin.Plat.AdminPlat.CustomRates.Where(item => item.Code == currency).SingleOrDefault();
                if (customRate != null)
                {
                    return Rounding(Rounding(totalPrice * customRate.Rate * ConstConfig.TransPremiumInsurance, 0) * tariffRate, 2);
                }
                else
                {
                    //没有币种对应的海关汇率，关税计算异常，返回-1；
                    return -1;
                }
            }
        }

        /// <summary>
        /// 计算增值税
        /// 公式: Round(报关总价 * 海关汇率 * 1.002 + 关税, 0) * 增值税率
        /// </summary>
        /// <returns></returns>
        [Obsolete("归类时不再计算增值税，改为报价时计算，该方法保留以防以后业务变更")]
        protected decimal CalcValueAddedTax()
        {
            var totalPrice = Convert.ToDecimal(Request.Form["TotalPrice"]);
            var currency = Request.Form["Currency"];
            var tariffValue = Convert.ToDecimal(Request.Form["TariffValue"]);
            var valueAddTaxRate = Convert.ToDecimal(Request.Form["ValueAddTaxValue"]);

            //如果是人民币，则海关汇率为1
            if (currency == "CNY")
            {
                return Rounding(totalPrice * 1 * ConstConfig.TransPremiumInsurance + tariffValue, 0) * valueAddTaxRate;
            }
            else
            {
                //获取币种的海关汇率
                var customRate = Needs.Wl.Admin.Plat.AdminPlat.CustomRates.Where(item => item.Code == currency).SingleOrDefault();
                if (customRate != null)
                {
                    return Rounding(totalPrice * customRate.Rate * ConstConfig.TransPremiumInsurance + tariffValue, 0) * valueAddTaxRate;
                }
                else
                {
                    //没有币种对应的海关汇率，增值税计算异常，返回-1；
                    return -1;
                }
            }
        }

        /// <summary>
        /// 四舍五入  IEEE规范
        /// </summary>
        /// <param name="Value">值</param>
        /// <param name="Digit">保留位数</param>
        /// <returns></returns>
        private decimal Rounding(decimal? Value, int Digit)
        {
            return Math.Round((decimal)Value, Digit, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 根据产品型号查找历史纪录
        /// </summary>
        /// <returns></returns>
        protected object GetHistoryCategories()
        {
            var model = Request.Form["Model"];

            var data = new ProductCategoriesView(model).AsEnumerable().Select(item => new
            {
                item.ID,
                item.Model,
                item.Name,
                item.TariffRate,
                item.AddedValueRate,
                item.UnitPrice,
                InspectionFee = item.InspectionFee == null ? string.Empty : Convert.ToString(item.InspectionFee),
                item.Qty,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                IsInspection = item.InspectionFee == null ? "否" : "是",
            }).OrderByDescending(item => item.CreateDate).Take(5).ToList();

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
        /// 验证当前型号的归类结果与之前的归类历史记录是否一致
        /// 如果不一致返回信息提醒报关员
        /// </summary>
        protected void ClassifyCheck()
        {
            try
            {
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

                    bool isDefaultCCC = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.CCC) > 0);
                    bool isDefaultOriginProof = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.OriginProof) > 0);
                    bool isDefaultInsp = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.Inspection) > 0);
                    bool isDefaultForbidden = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.Forbid) > 0);
                    bool isDefaultHKForbidden = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.HKForbid) > 0);
                    bool isDefaultHighValue = (historyCategory.Type == null ? false : (historyCategory.Type & ItemCategoryType.HighValue) > 0);

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
                    if ((bool)entity.IsForbidden != isDefaultForbidden)
                    {
                        isPassed = false;
                        msg += "管控：当前归类<label style=\"color:green\">【" + ((bool)entity.IsForbidden ? "是" : "否") + "】</label>，" +
                                        " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.Forbid) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }
                    if ((bool)entity.IsHKForbidden != isDefaultHKForbidden)
                    {
                        isPassed = false;
                        msg += "香港管制：当前归类<label style=\"color:green\">【" + ((bool)entity.IsHKForbidden ? "是" : "否") + "】</label>，" +
                                        " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.HKForbid) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }

                    if ((bool)entity.IsInsp != isDefaultInsp)
                    {
                        isPassed = false;
                        msg += "是否商检：当前归类<label style=\"color:green\">【" + ((bool)entity.IsInsp ? "是" : "否") + "】</label>，" +
                                      " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.Inspection) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }

                    if ((bool)entity.IsHighValue != isDefaultHighValue)
                    {
                        isPassed = false;
                        msg += "高价值产品：当前归类<label style=\"color:green\">【" + ((bool)entity.IsHighValue ? "是" : "否") + "】</label>，" +
                                        " 历史纪录<label style=\"color:red\">【" + ((historyCategory.Type & ItemCategoryType.HighValue) > 0 ? "是" : "否") + "】</label><br/><br/>";
                    }

                    if ((bool)entity.IsInsp)
                    {
                        if (entity.InspFeeText == "" && historyCategory.InspectionFee.GetValueOrDefault() == 0)
                        {

                        }
                        else if (entity.InspFeeText == "" || (decimal)entity.InspFeeText != historyCategory.InspectionFee.GetValueOrDefault())
                        {
                            isPassed = false;
                            msg += "商检费：当前归类<label style=\"color:green\">【" + entity.InspFeeText + "】</label>，" +
                                          " 历史纪录<label style=\"color:red\">【" + historyCategory.InspectionFee + "】</label><br/><br/>";
                        }
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
        /// 产品归类日志
        /// </summary>
        /// <returns></returns>
        protected object GetProductClassifyLogs()
        {
            string preProductID = Request.Form["PreProductID"];
            var data = new ProductClassifyLogsView().Where(log => log.ClassifyProductID == preProductID)
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
                                                    .Select(log => new
                                                    {
                                                        log.ID,
                                                        log.CreateDate,
                                                        log.Summary
                                                    });



            return data;
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
            if (step == ClassifyStep.PreStep1)
            {
                targetClassifyStatus = ClassifyStatus.Unclassified;
            }
            else if (step == ClassifyStep.PreStep2)
            {
                targetClassifyStatus = ClassifyStatus.First;
            }

            Expression<Func<PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == targetClassifyStatus;

            List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
            lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.CreateDate));
            lamdasOrderByAscDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate?? DateTime.MaxValue));

            var product = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll.GetTop(
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