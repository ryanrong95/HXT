using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Classify;

namespace WebApp.PvData_Classify
{
    public partial class SecondList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名
            this.Model.Admin = new
            {
                ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                UserName = Needs.Wl.Admin.Plat.AdminPlat.Current.UserName,
                RealName = byName,
                Role = adminRole == null ? DeclarantRole.SeniorDeclarant.GetHashCode() : adminRole.DeclarantRole.GetHashCode()
            }.Json();

            var pvdataApi = new PvDataApiSetting();
            var wladminApi = new WlAdminApiSetting();

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.SubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.GetNext,
            }.Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string model = Request.QueryString["Model"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string FirstOperator = Request.QueryString["FirstOperator"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_ClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.First;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                Expression<Func<PD_ClassifyProduct, bool>> lambda1 = item => item.OrderID.Contains(orderID.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                Expression<Func<PD_ClassifyProduct, bool>> lambda1 = item => item.Model.Contains(model.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.Name.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(FirstOperator))
            {
                lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.ClassifyFirstOperator != null &&
                                                                                item.Category.ClassifyFirstOperator.RealName == FirstOperator.Trim()));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.UpdateDate < dt));
                }
            }
            if (!string.IsNullOrEmpty(strIsShowLocked))
            {
                bool.TryParse(strIsShowLocked, out isShowLocked);
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
            lamdasOrderByAscDateTime.Add((Expression<Func<PD_ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_ClassifyProductsAll.GetPageList(
                page,
                rows,
                expression,
                lamdasOrderByAscDateTime.ToArray(),
                null,
                Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                isShowLocked,
                lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                        item => new
                        {
                            item.ID,
                            ItemID = item.ID,
                            item.OrderID,
                            MainID = item.OrderID,
                            item.MainOrderID,
                            OrderedDate = item.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            ClientCode = item.Client.ClientCode,
                            ClientName = Needs.Wl.Admin.Plat.AdminPlat.Current.ID == "Admin00002" ? "" : item.Client.Company.Name,

                            PartNumber = item.Model,
                            Manufacturer = item.Manufacturer,
                            CustomName = item.Name,
                            Origin = item.Origin,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            Currency = item.Currency,
                            TotalPrice = item.TotalPrice.ToString("0.0000"),
                            ClassifyStatus = item.ClassifyStatus.GetDescription(),
                            ClassifyFirstOperatorName = item.Category.ClassifyFirstOperator?.RealName ?? "--",
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                            HSCode = item.Category.HSCode,
                            TariffName = item.Category.Name,
                            ImportPreferentialTaxRate = item.ImportTax.Rate.ToString("0.0000"),
                            VATRate = item.AddedValueTax.Rate.ToString("0.0000"),
                            ExciseTaxRate = item.ExciseTax?.Rate.ToString("0.0000") ?? "0.0000",
                            TaxCode = item.Category.TaxCode,
                            TaxName = item.Category.TaxName,
                            LegalUnit1 = item.Category.Unit1,
                            LegalUnit2 = item.Category.Unit2,
                            CIQCode = item.Category.CIQCode,
                            Elements = item.Category.Elements,

                            OriginATRate = "",
                            CIQ = (item.Category.Type & ItemCategoryType.Inspection) > 0,
                            CIQprice = item.InspectionFee.GetValueOrDefault(),
                            Ccc = (item.Category.Type & ItemCategoryType.CCC) > 0,
                            Embargo = (item.Category.Type & ItemCategoryType.Forbid) > 0,
                            HkControl = (item.Category.Type & ItemCategoryType.HKForbid) > 0,
                            IsHighPrice = (item.Category.Type & ItemCategoryType.HighValue) > 0,
                            Coo = (item.Category.Type & ItemCategoryType.OriginProof) > 0,

                            //特殊类型
                            SpecialType = item.Category.GetSpecialTypeForClassify(),

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.ByName ?? "--",
                            LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
                        }
                     ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 一键归类
        /// </summary>
        protected void QuickClassify()
        {
            try
            {
                string[] ids = Request.Form["itemIds"].Split(',');
                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_ClassifyProduct, DateTime>>)(t => t.CreateDate));
                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_ClassifyProductsAll.GetTop(ids.Length, i => ids.Contains(i.ID), lamdasOrderByAscDateTime.ToArray(), null);

                #region 调用中心数据接口批量获取型号的管控信息

                var apisetting = new PvDataApiSetting();
                var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetMultiSysControls;
                var partNumbers = items.Select(item => item.Model).Distinct().ToList();
                var controls = new List<Control>();
                var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JSingle<string>>(url, new
                {
                    PartNumbers = partNumbers
                });
                if (result.code == 300)
                {
                    throw new Exception($"获取型号管控信息接口调用异常 - {result.data}");
                }
                if (result.code == 200)
                {
                    controls = result.data.JsonTo<List<Control>>();
                }

                #endregion

                foreach (var item in items)
                {
                    //管控信息
                    item.IsCCC = (item.Category.Type & ItemCategoryType.CCC) > 0;
                    item.IsOriginProof = (item.Category.Type & ItemCategoryType.OriginProof) > 0;
                    item.IsInsp = (item.Category.Type & ItemCategoryType.Inspection) > 0;
                    item.IsHighValue = (item.Category.Type & ItemCategoryType.HighValue) > 0;
                    item.IsForbid = (item.Category.Type & ItemCategoryType.Forbid) > 0;

                    item.IsSysForbid = controls.FirstOrDefault(c => c.PartNumber == item.Model)?.IsSysEmbargo ?? false;
                    item.IsSysCCC = controls.FirstOrDefault(c => c.PartNumber == item.Model)?.IsSysCcc ?? false;

                    if (item.IsSysForbid)
                    {
                        item.Category.Type |= ItemCategoryType.Forbid;
                    }

                    item.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    var classify = PD_ClassifyFactory.Create(ClassifyStep.Step2, item);
                    classify.QuickClassify();
                }

                var classifiedResults = GetClassifiedResults(items);
                Response.Write((new { success = true, message = "产品归类成功", data = classifiedResults }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "产品归类失败：" + ex.Message }).Json());
            }
        }

        private string GetClassifiedResults(PD_ClassifyProduct[] classifyProducts)
        {
            var orderIds = classifyProducts.Select(item => item.OrderID).Distinct().ToArray();
            var mainOrderIds = classifyProducts.Select(item => item.MainOrderID).Distinct().ToArray();
            var orderFiles = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MainOrderFiles.Where(item => mainOrderIds.Contains(item.MainOrderID) && item.FileType == FileType.OriginalInvoice).ToList();
            var orderVoyages = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(item => orderIds.Contains(item.Order.ID)).ToList();
            var pvdataApi = new PvDataApiSetting();
            var wladminApi = new WlAdminApiSetting();
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);

            return classifyProducts.Select(item => new
            {
                ItemID = item.ID,
                MainID = item.OrderID,
                OrderedDate = item.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ClientCode = item.Client.ClientCode,
                ClientName = item.Client.Company.Name,

                PartNumber = item.Model,
                Manufacturer = item.Manufacturer,
                Origin = item.Origin,
                UnitPrice = item.UnitPrice.ToString("0.0000"),
                Quantity = item.Quantity,
                Unit = item.Unit,
                Currency = item.Currency,
                TotalPrice = item.TotalPrice.ToString("0.0000"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                HSCode = item.Category?.HSCode,
                TariffName = item.Category?.Name,
                ImportPreferentialTaxRate = item.ImportTax?.Rate.ToString("0.0000"),//华芯通没有区分优惠税率和加征税率, 将进口关税赋给优惠税率
                VATRate = item.AddedValueTax?.Rate.ToString("0.0000"),
                ExciseTaxRate = item.ExciseTax?.Rate.ToString("0.0000") ?? "0.0000",
                TaxCode = item.Category?.TaxCode,
                TaxName = item.Category?.TaxName,
                LegalUnit1 = item.Category?.Unit1,
                LegalUnit2 = item.Category?.Unit2,
                CIQCode = item.Category?.CIQCode,
                Elements = item.Category?.Elements,

                OriginATRate = 0,//华芯通没有区分优惠税率和加征税率, 进口关税已经赋给优惠税率, 加征税率为0
                CIQ = (item.Category?.Type & ItemCategoryType.Inspection) > 0,
                CIQprice = item.InspectionFee.GetValueOrDefault(),
                Ccc = (item.Category?.Type & ItemCategoryType.CCC) > 0,
                Embargo = (item.Category?.Type & ItemCategoryType.Forbid) > 0,
                HkControl = (item.Category?.Type & ItemCategoryType.HKForbid) > 0,
                IsHighPrice = (item.Category?.Type & ItemCategoryType.HighValue) > 0,
                Coo = (item.Category?.Type & ItemCategoryType.OriginProof) > 0,

                PIs = GetPIs(orderFiles.Where(pi => pi.MainOrderID == item.MainOrderID)),
                SpecialType = GetSpecialType(orderVoyages.Where(ov => ov.Order.ID == item.OrderID)),
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.SubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.GetNext,
                Step = ClassifyStep.Step2.GetHashCode(),
                CreatorID = admin.ID,
                CreatorName = byName,
                Role = adminRole.DeclarantRole.GetHashCode()
            }).Json();
        }

        /// <summary>
        /// 验证是否可以归类
        /// </summary>
        /// <returns></returns>
        protected object ValidateClassify()
        {
            try
            {
                string itemId = Request.Form["itemId"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_ClassifyProductsAll[itemId];
                if (classifyProduct.ClassifyStatus == ClassifyStatus.First)
                {
                    return new
                    {
                        IsCanClassify = true,
                    };
                }
                else
                {
                    var declarant = classifyProduct.Category.ClassifySecondOperator?.ByName;
                    return new
                    {
                        IsCanClassify = false,
                        Message = "该产品型号已由报关员【" + declarant + "】完成了预处理二。"
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    IsCanClassify = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取订单信息：合同发票、特殊类型
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string orderId = Request.Form["orderId"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderId];
            var orderFiles = order.MainOrderFiles.Where(item => item.FileType == FileType.OriginalInvoice).ToList();
            var orderVoyages = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == orderId).ToList();

            return new
            {
                PIs = GetPIs(orderFiles),
                SpecialType = GetSpecialType(orderVoyages),
            };
        }

        /// <summary>
        /// 获取合同发票
        /// </summary>
        /// <param name="orderFiles"></param>
        /// <returns></returns>
        private string GetPIs(IEnumerable<MainOrderFile> orderFiles)
        {
            var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
            return orderFiles.Select(item => new
            {
                item.ID,
                FileName = item.Name,
                item.FileFormat,
                //  Url = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                Url = DateTime.Compare(item.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                    FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
            }).Json();
        }

        /// <summary>
        /// 获取特殊类型
        /// </summary>
        /// <param name="orderVoyages"></param>
        /// <returns></returns>
        private string GetSpecialType(IEnumerable<OrderVoyage> orderVoyages)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var st in orderVoyages)
            {
                sb.Append(st.Type.GetDescription() + "|");
            }

            return sb.Length > 0 ? sb.ToString().TrimEnd('|') : "--";
        }
    }

    public class Control
    {
        public string PartNumber;
        public bool IsSysCcc;
        public bool IsSysEmbargo;
    }
}
