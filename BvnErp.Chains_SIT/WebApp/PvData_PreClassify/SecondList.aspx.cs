using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
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

namespace WebApp.PvData_PreClassify
{
    /// <summary>
    /// 预处理二查询界面
    /// </summary>
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
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreGetNext,
            }.Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string Model = Request.QueryString["Model"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string FirstOperator = Request.QueryString["FirstOperator"];
            string DueDateBegin = Request.QueryString["DueDateBegin"];
            string DueDateEnd = Request.QueryString["DueDateEnd"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.First && item.PreProduct.UseType == PreProductUserType.Pre;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Expression<Func<PD_PreClassifyProduct, bool>> lambda1 = item => item.Model.Contains(Model);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.ProductName.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(FirstOperator))
            {
                lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.ClassifyFirstOperatorName == FirstOperator.Trim()));
            }
            if (!string.IsNullOrEmpty(DueDateBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(DueDateBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.PreProduct.DueDate != null && item.PreProduct.DueDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(DueDateEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(DueDateEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.PreProduct.DueDate != null && item.PreProduct.DueDate < dt));
                }
            }
            if (!string.IsNullOrEmpty(CreateDateBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(CreateDateBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.CreateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(CreateDateEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(CreateDateEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.CreateDate < dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.UpdateDate < dt));
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
            lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(t => t.CreateDate));
            lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate ?? DateTime.MaxValue));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_PreClassifyProductsStep2.GetPageList(
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
                           MainID = item.ID,
                           ClientCode = item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,
                           ProductUnionCode = item.ProductUnionCode,

                           PartNumber = item.Model,
                           Manufacturer = item.Manufacturer,
                           Origin = string.Empty,
                           UnitPrice = item.PreProduct.Price.ToString("0.0000"),
                           Quantity = item.PreProduct.Qty,
                           Currency = item.PreProduct.Currency,
                           ClassifyStatus = item.ClassifyStatus.GetDescription(),
                           ClassifyFirstOperatorName = item.ClassifyFirstOperatorName,
                           DueDate = item.PreProduct.DueDate?.ToString("yyyy-MM-dd") ?? "--",
                           CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           CompanyType = item.PreProduct.CompanyType.GetDescription(),

                           HSCode = item.HSCode,
                           TariffName = item.ProductName,
                           ImportPreferentialTaxRate = item.TariffRate?.ToString("0.0000"),
                           VATRate = item.AddedValueRate?.ToString("0.0000"),
                           ExciseTaxRate = item.ExciseTaxRate?.ToString("0.0000") ?? "0.0000",
                           TaxCode = item.TaxCode,
                           TaxName = item.TaxName,
                           LegalUnit1 = item.Unit1,
                           LegalUnit2 = item.Unit2,
                           CIQCode = item.CIQCode,
                           Elements = item.Elements,

                           OriginATRate = "",
                           CIQ = (item.Type & ItemCategoryType.Inspection) > 0,
                           CIQprice = item.InspectionFee.GetValueOrDefault(),
                           Ccc = (item.Type & ItemCategoryType.CCC) > 0,
                           Embargo = (item.Type & ItemCategoryType.Forbid) > 0,
                           HkControl = (item.Type & ItemCategoryType.HKForbid) > 0,
                           IsHighPrice = (item.Type & ItemCategoryType.HighValue) > 0,
                           Coo = (item.Type & ItemCategoryType.OriginProof) > 0,

                           //特殊类型
                           SpecialType = item.GetSpecialType(),

                           //产品归类锁定
                           LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                           Locker = item.Locker?.ByName ?? "--",
                           LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                           IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                           IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,

                           Source = item.Source
                       }
                    ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <returns></returns>
        protected void Return()
        {
            try
            {
                string id = Request.Form["mainId"];
                string reason = Request.Form["Reason"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var product = new PD_PreClassifyProduct()
                {
                    ID = id,
                    Admin = admin,
                    ClassifyStep = ClassifyStep.PreStep2,
                    Summary = reason,
                };
                var classify = PD_ClassifyFactory.Create(ClassifyStep.PreStep2, product);
                classify.Return();

                Response.Write((new { success = true, message = "退回成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = true, message = "退回失败: " + ex.Message }).Json());
            }
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
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(t => t.CreateDate));
                lamdasOrderByAscDateTime.Add((Expression<Func<PD_PreClassifyProduct, DateTime>>)(t => t.PreProduct.DueDate ?? DateTime.MaxValue));
                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_PreClassifyProductsAll.GetTop(ids.Length, i => ids.Contains(i.ID), lamdasOrderByAscDateTime.ToArray(), null);
                var classifiedResults = GetClassifiedResults(items);

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
                    item.IsOriginProof = (item.Type & ItemCategoryType.OriginProof) > 0;
                    item.IsCCC = (item.Type & ItemCategoryType.CCC) > 0;
                    item.IsHighValue = (item.Type & ItemCategoryType.HighValue) > 0;
                    item.IsInsp = (item.Type & ItemCategoryType.Inspection) > 0;
                    item.IsHKForbid = (item.Type & ItemCategoryType.HKForbid) > 0;
                    item.IsForbid = (item.Type & ItemCategoryType.Forbid) > 0;

                    item.IsSysForbid = controls.FirstOrDefault(c => c.PartNumber == item.Model)?.IsSysEmbargo ?? false;
                    item.IsSysCCC = controls.FirstOrDefault(c => c.PartNumber == item.Model)?.IsSysCcc ?? false;

                    item.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    var classify = PD_ClassifyFactory.Create(ClassifyStep.PreStep2, item);
                    classify.QuickClassify();
                }

                Response.Write((new { success = true, message = "产品归类成功", data = classifiedResults }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "产品归类失败：" + ex.Message }).Json());
            }
        }

        private string GetClassifiedResults(PD_PreClassifyProduct[] classifyProducts)
        {
            var pvdataApi = new PvDataApiSetting();
            var wladminApi = new WlAdminApiSetting();
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);

            return classifyProducts.Select(item => new
            {
                MainID = item.ID,
                ClientCode = item.PreProduct.Client.ClientCode,
                ClientName = item.PreProduct.Client.Company.Name,

                PartNumber = item.Model,
                Manufacturer = item.Manufacturer,
                Origin = item.PreProduct.AreaOfProduction,
                UnitPrice = item.PreProduct.Price.ToString("0.0000"),
                Currency = item.PreProduct.Currency,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                HSCode = item.HSCode,
                TariffName = item.ProductName,
                ImportPreferentialTaxRate = item.TariffRate?.ToString("0.0000"),//华芯通没有区分优惠税率和加征税率, 将进口关税赋给优惠税率
                VATRate = item.AddedValueRate?.ToString("0.0000"),
                ExciseTaxRate = item.ExciseTaxRate?.ToString("0.0000") ?? "0.0000",
                TaxCode = item.TaxCode,
                TaxName = item.TaxName,
                LegalUnit1 = item.Unit1,
                LegalUnit2 = item.Unit2,
                CIQCode = item.CIQCode,
                Elements = item.Elements,

                OriginATRate = 0,//华芯通没有区分优惠税率和加征税率, 进口关税已经赋给优惠税率, 加征税率为0
                CIQ = (item.Type & ItemCategoryType.Inspection) > 0,
                CIQprice = item.InspectionFee.GetValueOrDefault(),
                Ccc = (item.Type & ItemCategoryType.CCC) > 0,
                Embargo = (item.Type & ItemCategoryType.Forbid) > 0,
                HkControl = (item.Type & ItemCategoryType.HKForbid) > 0,
                IsHighPrice = (item.Type & ItemCategoryType.HighValue) > 0,
                Coo = (item.Type & ItemCategoryType.OriginProof) > 0,

                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreGetNext,
                Step = ClassifyStep.PreStep2.GetHashCode(),
                CreatorID = admin.ID,
                CreatorName = admin.RealName,
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
                var preClassifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_PreClassifyProductsAll[itemId];
                if (preClassifyProduct.ClassifyStatus == ClassifyStatus.First)
                {
                    return new
                    {
                        IsCanClassify = true,
                    };
                }
                else
                {
                    var declarant = preClassifyProduct.ClassifySecondOperatorName;
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

        protected void Delete()
        {
            try
            {
                string id = Request.Form["mainId"];
                var product = new PD_PreClassifyProduct()
                {
                    ID = id,
                    ClassifyStep = ClassifyStep.PreDoneEdit,
                    PreProduct = new PreProduct
                    {
                        ProductUnionCode = id
                    }
                };
                var classify = PD_ClassifyFactory.Create(ClassifyStep.PreDoneEdit, product);
                classify.Delete();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = true, message = "删除失败: " + ex.Message }).Json());
            }
        }

        protected void DeleteCheck()
        {
            string id = Request.Form["mainId"];
            var product = new Needs.Ccs.Services.Views.OrderItemsView().Where(t => t.ProductUniqueCode == id).FirstOrDefault();
            if (product != null)
            {
                Response.Write((new { success = false, message = "该产品已生成订单，不能删除!" }).Json());
            }
            else
            {
                Response.Write((new { success = true, message = "可以删除" }).Json());
            }
        }
    }

    public class Control
    {
        public string PartNumber;
        public bool IsSysCcc;
        public bool IsSysEmbargo;
    }
}
