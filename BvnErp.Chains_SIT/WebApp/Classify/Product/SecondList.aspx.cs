using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
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
    public partial class SecondList : Uc.PageBase
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
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];
            string strIsShowLocked = Request.QueryString["IsShowLocked"];
            bool isShowLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<ClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.First;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                Expression<Func<ClassifyProduct, bool>> lambda1 = item => item.OrderID.Contains(orderID.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                //var productIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Products.Where(c => c.Model.Contains(model.Trim())).Select(c => c.ID).ToArray();
                //Expression<Func<ClassifyProduct, bool>> lambda1 = item => productIds.Contains(item.ProductID);
                var orderItemIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(item => item.Model.Contains(model.Trim())).Select(item => item.ID).ToArray();
                Expression<Func<ClassifyProduct, bool>> lambda1 = item => orderItemIDs.Contains(item.ID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.Name.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.UpdateDate < dt));
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
            lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetPageList(
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
                            item.OrderID,
                            item.Client.ClientCode,
                            ClientName = item.Client.Company.Name,
                            item.Category.HSCode,
                            Name = item.Category.Name,
                            item.Category.Elements,
                            item.Model,
                            item.Manufacturer,
                            item.Origin,
                            item.Quantity,
                            item.Unit,
                            item.Category.Unit1,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            item.Currency,
                            TariffRate = item.ImportTax.Rate.ToString("0.0000"),
                            ValueAddRate = item.AddedValueTax.Rate.ToString("0.0000"),
                            item.Category.CIQCode,
                            ClassifyStatus = item.ClassifyStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString().Replace("T", " ") ?? "--",

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.RealName ?? "--",
                            LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
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
                string[] ids = Request.Form["IDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetTop(50, i => ids.Contains(i.ID), lamdasOrderByAscDateTime.ToArray(), null);

                foreach (var item in items)
                {
                    //管控信息
                    item.IsCCC = (item.Category.Type & ItemCategoryType.CCC) > 0;
                    item.IsOriginProof = (item.Category.Type & ItemCategoryType.OriginProof) > 0;
                    item.IsInsp = (item.Category.Type & ItemCategoryType.Inspection) > 0;
                    item.IsHighValue = (item.Category.Type & ItemCategoryType.HighValue) > 0;
                    item.IsSysForbid = (item.ControlType(item.Model) & ItemCategoryType.Forbid) > 0;

                    item.IsSysCCC = (item.ControlType(item.Model) & ItemCategoryType.CCC) > 0;

                    item.IsForbid = (item.Category.Type & ItemCategoryType.Forbid) > 0;

                    item.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    var classify = ClassifyFactory.Create(ClassifyStep.Step2, item);
                    classify.QuickClassify();
                }

                Response.Write((new { success = true, message = "产品归类成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "产品归类失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 批量锁定
        /// </summary>
        protected void BatchLock()
        {
            try
            {
                string[] ids = Request.Form["IDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');

                List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetTop(50, i => ids.Contains(i.ID), lamdasOrderByAscDateTime.ToArray(), null);

                bool isNeedRemindLocked = false;
                StringBuilder sbLockedRemind = new StringBuilder();

                foreach (var item in items)
                {
                    if (item.IsLocked)//已经被锁定
                    {
                        if (item.Locker != null && item.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                        {
                            isNeedRemindLocked = true;
                            //锁定人不是当前登录者,提示已经被他人锁定
                            sbLockedRemind.Append("【" + item.Model + "】、");
                        }
                    }
                    else//未被锁定,进行锁定
                    {
                        item.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.Step2, item);
                        classify.Lock();
                    }
                }

                if (isNeedRemindLocked)
                {
                    Response.Write((new { success = true, message = "以下型号已被他人锁定：\r\n" + sbLockedRemind.ToString().Trim(new char[] { '、' }), }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "批量锁定成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "批量锁定失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 归类锁定
        /// </summary>
        protected void Lock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                if (classifyProduct.IsLocked && classifyProduct.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                {
                    Response.Write((new { success = false, message = "当前产品归类已被锁定，锁定人【" + classifyProduct.Locker.RealName + "】，锁定时间【" + classifyProduct.LockDate + "】" }).Json());
                }
                else
                {
                    if (!classifyProduct.IsLocked)
                    {
                        classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.Step2, classifyProduct);
                        classify.Lock();
                    }
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 解除归类锁定
        /// </summary>
        protected void UnLock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var classify = ClassifyFactory.Create(ClassifyStep.Step2, classifyProduct);
                classify.UnLock();

                Response.Write((new { success = true, message = "已解除产品归类锁定" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }
    }
}