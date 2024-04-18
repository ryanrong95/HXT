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
    /// 预处理一查询界面
    /// </summary>
    public partial class FirstList : Uc.PageBase
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
            string CallBackUrl2 = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified;

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreGetNext,
            }.Json();
        }

        /// <summary>
        /// 初始化预归类产品数据
        /// </summary>
        protected void data()
        {
            string Model = Request.QueryString["Model"];
            string DueDateBegin = Request.QueryString["DueDateBegin"];
            string DueDateEnd = Request.QueryString["DueDateEnd"];
            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Unclassified && item.PreProduct.UseType == PreProductUserType.Pre;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Expression<Func<PD_PreClassifyProduct, bool>> lambda1 = item => item.Model.Contains(Model);
                lamdas.Add(lambda1);
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

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_PreClassifyProductsStep1.GetPageList(
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

                           Name = item.ProductName,
                           PartNumber = item.Model,
                           Manufacturer = item.Manufacturer,
                           Origin = string.Empty,
                           UnitPrice = item.PreProduct.Price.ToString("0.0000"),
                           Quantity = item.PreProduct.Qty,
                           Currency = item.PreProduct.Currency,
                           ClassifyStatus = item.ClassifyStatus.GetDescription(),
                           PreProductID = item.PreProductID,
                           DueDate = item.PreProduct.DueDate?.ToString("yyyy-MM-dd") ?? "--",
                           CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                           //产品归类锁定
                           LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                           Locker = item.Locker?.ByName ?? "--",
                           LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                           IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                           IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,

                           Source = item.Source,
                           MerchandiserName = item.PreProduct.Client.Merchandiser.RealName
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
                    ClassifyStep = ClassifyStep.PreStep1,
                    Summary = reason,
                };
                var classify = PD_ClassifyFactory.Create(ClassifyStep.PreStep1, product);
                classify.Return();

                Response.Write((new { success = true, message = "退回成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = true, message = "退回失败: " + ex.Message }).Json());
            }
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
                if (preClassifyProduct.ClassifyStatus == ClassifyStatus.Unclassified)
                {
                    return new
                    {
                        IsCanClassify = true,
                    };
                }
                else
                {
                    var declarant = preClassifyProduct.ClassifyFirstOperatorName;
                    return new
                    {
                        IsCanClassify = false,
                        Message = "该产品型号已由报关员【" + declarant + "】完成了预处理一。"
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
}
