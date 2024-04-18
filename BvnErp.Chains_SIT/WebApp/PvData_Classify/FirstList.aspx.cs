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
    public partial class FirstList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int page, rows;
            int.TryParse(Request.QueryString["PageNumber"], out page);
            int.TryParse(Request.QueryString["PageSize"], out rows);

            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = page,
                PageSize = rows,
                IsShowLocked = Convert.ToBoolean(Request.QueryString["IsShowLocked"]),
                Model = Convert.ToString(Request.QueryString["Model"]) ?? string.Empty,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ProductName = Convert.ToString(Request.QueryString["ProductName"]) ?? string.Empty,
                HSCode = Convert.ToString(Request.QueryString["HSCode"]) ?? string.Empty,
                LastClassifyTimeBegin = Convert.ToString(Request.QueryString["LastClassifyTimeBegin"]) ?? string.Empty,
                LastClassifyTimeEnd = Convert.ToString(Request.QueryString["LastClassifyTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();

            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);
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
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_ClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Unclassified;

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
                            ClientName = item.Client.Company.Name,

                            PartNumber = item.Model,
                            Manufacturer = item.Manufacturer,
                            CustomName = item.Name,
                            TariffName = item.Name,
                            Origin = item.Origin,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            Currency = item.Currency,
                            TotalPrice = item.TotalPrice.ToString("0.0000"),
                            ClassifyStatus = item.ClassifyStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.ByName ?? "--",
                            LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,

                            MerchandiserName = item.Client.Merchandiser.RealName
                        }
                     ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
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
                if (classifyProduct.ClassifyStatus == ClassifyStatus.Unclassified)
                {
                    return new
                    {
                        IsCanClassify = true,
                    };
                }
                else
                {
                    var declarant = classifyProduct.Category.ClassifyFirstOperator?.ByName;
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

        /// <summary>
        /// 获取订单信息：合同发票、特殊类型
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string orderId = Request.Form["orderId"];

            #region 合同发票
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderId];
            var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
            // var t2 = order.CreateDate;
            var pis = order.MainOrderFiles.Where(item => item.FileType == FileType.OriginalInvoice)
                .ToList().Select(item => new
                {
                    item.ID,
                    FileName = item.Name,
                    item.FileFormat,
                    Url = DateTime.Compare(item.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                    // Url = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                }).Json();
            #endregion

            #region 特殊类型
            var specialTypes = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == orderId).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var st in specialTypes)
            {
                sb.Append(st.Type.GetDescription() + "|");
            }
            #endregion

            return new
            {
                PIs = pis,
                SpecialType = sb.Length > 0 ? sb.ToString().TrimEnd('|') : "--",
            };
        }
    }
}
